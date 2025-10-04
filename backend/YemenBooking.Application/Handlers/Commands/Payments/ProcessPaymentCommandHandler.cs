using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Payments;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Notifications;

namespace YemenBooking.Application.Handlers.Commands.Payments;

/// <summary>
/// مُعالج أمر معالجة الدفع
/// Payment processing command handler
/// 
/// يقوم بمعالجة عملية الدفع ويشمل:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الحجز
/// - التحقق من صلاحيات المستخدم
/// - التحقق من قواعد الأعمال
/// - التحقق من حالة الحجز
/// - معالجة الدفع
/// - إنشاء حدث الدفع
/// 
/// Processes payment and includes:
/// - Input data validation
/// - Booking existence validation
/// - User authorization validation
/// - Business rules validation
/// - Booking state validation
/// - Payment processing
/// - Payment event creation
/// </summary>
public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ResultDto<Guid>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPaymentGatewayService _paymentGatewayService;
    private readonly IValidationService _validationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentCommandHandler(
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository,
        IPaymentGatewayService paymentGatewayService,
        IValidationService validationService,
        IAuditService auditService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUserService,
        ILogger<ProcessPaymentCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
        _paymentGatewayService = paymentGatewayService;
        _validationService = validationService;
        _auditService = auditService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// معالجة أمر معالجة الدفع
    /// Handle payment processing command
    /// </summary>
    /// <param name="request">طلب معالجة الدفع / Payment processing request</param>
    /// <param name="cancellationToken">رمز الإلغاء / Cancellation token</param>
    /// <returns>نتيجة العملية مع معرف الدفع / Operation result with payment ID</returns>
    public async Task<ResultDto<Guid>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر الدفع للحجز: {BookingId} / Starting payment processing for booking: {BookingId}", request.BookingId);

            // الخطوة 1: التحقق من صحة البيانات المدخلة
            // Step 1: Input data validation
            var inputValidationResult = await ValidateInputAsync(request, cancellationToken);
            if (!inputValidationResult.Success)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات المدخلة: {Errors} / Input validation failed: {Errors}", string.Join(", ", inputValidationResult.Errors));
                return ResultDto<Guid>.Failed(inputValidationResult.Errors);
            }

            // الخطوة 2: التحقق من وجود الحجز
            // Step 2: Booking existence validation
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("الحجز غير موجود: {BookingId} / Booking not found: {BookingId}", request.BookingId);
                return ResultDto<Guid>.Failed("الحجز غير موجود / Booking not found");
            }

            // الخطوة 3: التحقق من صلاحيات المستخدم
            // Step 3: User authorization validation
            var authorizationResult = await ValidateAuthorizationAsync(booking, cancellationToken);
            if (!authorizationResult.Success)
            {
                _logger.LogWarning("فشل التحقق من الصلاحيات للمستخدم: {UserId} / Authorization failed for user: {UserId}", _currentUserService.UserId);
                return ResultDto<Guid>.Failed(authorizationResult.Errors);
            }

            // الخطوة 4: التحقق من قواعد الأعمال
            // Step 4: Business rules validation
            var businessRulesResult = await ValidateBusinessRulesAsync(booking, request, cancellationToken);
            if (!businessRulesResult.Success)
            {
                _logger.LogWarning("فشل التحقق من قواعد الأعمال: {Errors} / Business rules validation failed: {Errors}", string.Join(", ", businessRulesResult.Errors));
                return ResultDto<Guid>.Failed(businessRulesResult.Errors);
            }

            // الخطوة 5: التحقق من حالة الحجز
            // Step 5: Booking state validation
            var stateValidationResult = await ValidateBookingStateAsync(booking);
            if (!stateValidationResult.Success)
            {
                _logger.LogWarning("فشل التحقق من حالة الحجز: {Errors} / Booking state validation failed: {Errors}", string.Join(", ", stateValidationResult.Errors));
                return ResultDto<Guid>.Failed(stateValidationResult.Errors);
            }

            // الخطوة 6: معالجة الدفع
            // Step 6: Process payment
            var paymentResult = await ProcessPaymentAsync(booking, request, cancellationToken);
            if (!paymentResult.Success)
            {
                _logger.LogError("فشل في معالجة الدفع: {Errors} / Payment processing failed: {Errors}", string.Join(", ", paymentResult.Errors));
                return ResultDto<Guid>.Failed(paymentResult.Errors);
            }

            // الخطوة 7: تسجيل العملية ونشر الأحداث
            // Step 7: Audit logging and event publishing
            await LogAuditAndPublishEventsAsync(booking, paymentResult.Data, cancellationToken);

            _logger.LogInformation("تم معالجة الدفع بنجاح: {PaymentId} / Payment processed successfully: {PaymentId}", paymentResult.Data.Id);
            return ResultDto<Guid>.Succeeded(paymentResult.Data.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة أمر الدفع: {BookingId} / Error processing payment command: {BookingId}", request.BookingId);
            return ResultDto<Guid>.Failed("حدث خطأ أثناء معالجة الدفع / An error occurred while processing payment");
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateInputAsync(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الحجز
        // Validate booking ID
        if (request.BookingId == Guid.Empty)
        {
            errors.Add("معرف الحجز مطلوب / Booking ID is required");
        }

        // التحقق من المبلغ
        // Validate amount
        if (request.Amount == null)
        {
            errors.Add("مبلغ الدفع مطلوب / Payment amount is required");
        }
        else if (request.Amount.Amount <= 0)
        {
            errors.Add("مبلغ الدفع يجب أن يكون أكبر من صفر / Payment amount must be greater than zero");
        }

        // التحقق من طريقة الدفع
        // Validate payment method
        if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
        {
            errors.Add("طريقة الدفع غير صحيحة / Invalid payment method");
        }

        // التحقق من معرف المعاملة
        // Validate transaction ID
        if (string.IsNullOrWhiteSpace(request.TransactionId))
        {
            errors.Add("معرف المعاملة مطلوب / Transaction ID is required");
        }

        // التحقق من صحة البيانات باستخدام خدمة التحقق
        // Validate data using validation service
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors.Select(e => e.Message));
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من صلاحيات المستخدم
    /// Validate user authorization
    /// </summary>
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Booking booking, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var userRoles = _currentUserService.UserRoles;

        // المدير وموظف الاستقبال لديهم صلاحية كاملة
        // Admin and staff have full access
        if (userRoles.Contains("Admin") || userRoles.Contains("Staff"))
        {
            return ResultDto<bool>.Succeeded(true);
        }

        // مالك الكيان يمكنه معالجة الدفعات لوحداته
        // Property owner can process payments for their units
        if (userRoles.Contains("Owner"))
        {
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit != null)
            {
                var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
                if (property != null && property.OwnerId == currentUserId)
                    return ResultDto<bool>.Succeeded(true);
            }
        }

        // الضيف يمكنه معالجة دفعات حجوزاته فقط
        // Guest can process payments for their own bookings only
        if (userRoles.Contains("Guest") && booking.UserId == currentUserId)
        {
            return ResultDto<bool>.Succeeded(true);
        }

        return ResultDto<bool>.Failed("ليس لديك صلاحية لمعالجة هذا الدفع / You don't have permission to process this payment");
    }

    /// <summary>
    /// التحقق من قواعد الأعمال
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Booking booking, ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من وجود دفعة سابقة بنفس المعاملة
        var existingPayments = await _paymentRepository.GetPaymentsByBookingAsync(request.BookingId, cancellationToken);
        if (existingPayments.Any())
        {
            errors.Add("تم العثور على دفعة سابقة لهذا الحجز / Previous payment found for this booking");
        }

        // التحقق من أن المبلغ لا يتجاوز المبلغ المطلوب للحجز
        // Check that amount doesn't exceed required booking amount
        var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id, cancellationToken);
        var remainingAmount = booking.TotalPrice.Amount - totalPaid;
        if (request.Amount.Amount > remainingAmount)
        {
            errors.Add($"المبلغ المدفوع يتجاوز المبلغ المطلوب. المبلغ المتبقي: {remainingAmount} / Payment amount exceeds required amount. Remaining: {remainingAmount}");
        }

        // التحقق من طريقة الدفع المسموح بها
        // Note: Replace with appropriate method when available
        // var allowedMethods = await _paymentGatewayService.GetAllowedPaymentMethodsAsync(request.PropertyId, cancellationToken);
        // if (!allowedMethods.Contains(request.PaymentMethod))
        // {
        //     errors.Add("طريقة الدفع غير مسموح بها لهذا الكيان / Payment method not allowed for this property");
        // }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من حالة الحجز
    /// Validate booking state
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBookingStateAsync(Booking booking)
    {
        var errors = new List<string>();

        // التحقق من أن الحجز في حالة تسمح بالدفع
        // Check that booking is in a state that allows payment
        var validStatuses = new[] { BookingStatus.Pending, BookingStatus.Confirmed };
        if (!validStatuses.Contains(booking.Status))
        {
            errors.Add($"لا يمكن معالجة الدفع للحجز في الحالة الحالية: {booking.Status} / Cannot process payment for booking in current status: {booking.Status}");
        }

        // التحقق من أن الحجز لم يتم إلغاؤه
        // Check that booking is not cancelled
        if (booking.Status == BookingStatus.Cancelled)
        {
            errors.Add("لا يمكن معالجة الدفع للحجز الملغى / Cannot process payment for cancelled booking");
        }

        // التحقق من أن الحجز لم ينته
        // Check that booking has not expired
        if (booking.CheckOut < DateTime.UtcNow)
        {
            errors.Add("لا يمكن معالجة الدفع للحجز المنتهي / Cannot process payment for expired booking");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// معالجة الدفع
    /// Process payment
    /// </summary>
    private async Task<ResultDto<Payment>> ProcessPaymentAsync(Booking booking, ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // معالجة الدفع عبر بوابة الدفع
            // Process payment through payment gateway
            var gatewayResult = await _paymentGatewayService.ChargePaymentAsync(request.Amount.Amount, request.Amount.Currency, request.PaymentMethod.ToString(), null, cancellationToken);

            if (!gatewayResult.IsSuccess)
            {
                return ResultDto<Payment>.Failed($"فشل في معالجة الدفع: {gatewayResult.ErrorMessage} / Payment processing failed: {gatewayResult.ErrorMessage}");
            }

            // إنشاء سجل الدفع
            // Create payment record
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                Amount = new Money(request.Amount.Amount, request.Amount.Currency),
                Method = null, // سيتم تعيين قيمة افتراضية
                TransactionId = request.TransactionId,
                GatewayTransactionId = gatewayResult.TransactionId,
                Status = PaymentStatus.Successful,
                PaymentDate = DateTime.UtcNow,
                ProcessedBy = _currentUserService.UserId
            };

            // حفظ الدفع في قاعدة البيانات
            // Save payment to database
            await _paymentRepository.AddAsync(payment, cancellationToken);

            // تحديث حالة الحجز بناءً على الدفع
            // Update booking status based on payment
            await UpdateBookingStatusAsync(booking, cancellationToken);

            return ResultDto<Payment>.Succeeded(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة الدفع للحجز: {BookingId} / Error processing payment for booking: {BookingId}", booking.Id);
            return ResultDto<Payment>.Failed("حدث خطأ أثناء معالجة الدفع / An error occurred while processing payment");
        }
    }

    /// <summary>
    /// تحديث حالة الحجز بناءً على الدفع
    /// Update booking status based on payment
    /// </summary>
    private async Task UpdateBookingStatusAsync(Booking booking, CancellationToken cancellationToken)
    {
        var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id, cancellationToken);
        
        if (totalPaid >= booking.TotalPrice.Amount)
        {
            // تم دفع المبلغ كاملاً
            // Full payment completed
            booking.Status = BookingStatus.Confirmed;
        }
        else
        {
            // دفع جزئي
            // Partial payment
            booking.Status = BookingStatus.Pending;
        }

        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    /// <summary>
    /// تسجيل العملية ونشر الأحداث
    /// Log audit and publish events
    /// </summary>
    private async Task LogAuditAndPublishEventsAsync(Booking booking, Payment payment, CancellationToken cancellationToken)
    {
        // تسجيل العملية
        // Audit logging
        await _auditService.LogBusinessOperationAsync(
            "ProcessPayment",
            $"Processed payment for booking {booking.Id} with amount {payment.Amount.Amount} {payment.Amount.Currency}",
            payment.Id,
            "Payment",
            _currentUserService.UserId,
            null,
            cancellationToken);

        // نشر حدث معالجة الدفع
        // Publish payment processed event
        var paymentEvent = new PaymentProcessedEvent
        {
            PaymentId = payment.Id,
            BookingId = booking.Id,
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            TransactionId = payment.TransactionId,
            Status = payment.Status.ToString(),
            ProcessedAt = payment.PaymentDate,
            Currency = payment.Amount.Currency,
            UserId = payment.ProcessedBy
        };

        await _eventPublisher.PublishAsync(paymentEvent, cancellationToken);

        // إرسال إشعار للضيف
        // Send notification to guest
        await _notificationService.SendAsync(new NotificationRequest
        {
            UserId = booking.UserId,
            Type = NotificationType.PaymentProcessed,
            Title = "تم معالجة الدفع / Payment Processed",
            Message = $"تم معالجة دفعتك بمبلغ {payment.Amount.Amount} {payment.Amount.Currency} بنجاح / Your payment of {payment.Amount.Amount} {payment.Amount.Currency} has been processed successfully",
            Data = new { PaymentId = payment.Id, BookingId = booking.Id }
        }, cancellationToken);

        // حفظ التغييرات
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
