using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Payment;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.PaymentManagement;

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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGatewayService _paymentGatewayService;
    private readonly IValidationService _validationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IPaymentGatewayService paymentGatewayService,
        IValidationService validationService,
        IAuditService auditService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUserService,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _paymentGatewayService = paymentGatewayService;
        _validationService = validationService;
        _auditService = auditService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
        _logger = logger;
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
            var inputValidationResult = await ValidateInputAsync(request);
            if (!inputValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات المدخلة: {Errors} / Input validation failed: {Errors}", string.Join(", ", inputValidationResult.Errors));
                return ResultDto<Guid>.Failed(inputValidationResult.Errors);
            }

            // الخطوة 2: التحقق من وجود الحجز
            // Step 2: Booking existence validation
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            if (booking == null)
            {
                _logger.LogWarning("الحجز غير موجود: {BookingId} / Booking not found: {BookingId}", request.BookingId);
                return ResultDto<Guid>.Failed("الحجز غير موجود / Booking not found");
            }

            // الخطوة 3: التحقق من صلاحيات المستخدم
            // Step 3: User authorization validation
            var authorizationResult = await ValidateAuthorizationAsync(booking);
            if (!authorizationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من الصلاحيات للمستخدم: {UserId} / Authorization failed for user: {UserId}", _currentUserService.UserId);
                return ResultDto<Guid>.Failed(authorizationResult.Errors);
            }

            // الخطوة 4: التحقق من قواعد الأعمال
            // Step 4: Business rules validation
            var businessRulesResult = await ValidateBusinessRulesAsync(booking, request);
            if (!businessRulesResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من قواعد الأعمال: {Errors} / Business rules validation failed: {Errors}", string.Join(", ", businessRulesResult.Errors));
                return ResultDto<Guid>.Failed(businessRulesResult.Errors);
            }

            // الخطوة 5: التحقق من حالة الحجز
            // Step 5: Booking state validation
            var stateValidationResult = await ValidateBookingStateAsync(booking);
            if (!stateValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من حالة الحجز: {Errors} / Booking state validation failed: {Errors}", string.Join(", ", stateValidationResult.Errors));
                return ResultDto<Guid>.Failed(stateValidationResult.Errors);
            }

            // الخطوة 6: معالجة الدفع
            // Step 6: Process payment
            var paymentResult = await ProcessPaymentAsync(booking, request);
            if (!paymentResult.IsSuccess)
            {
                _logger.LogError("فشل في معالجة الدفع: {Errors} / Payment processing failed: {Errors}", string.Join(", ", paymentResult.Errors));
                return ResultDto<Guid>.Failed(paymentResult.Errors);
            }

            // الخطوة 7: تسجيل العملية ونشر الأحداث
            // Step 7: Audit logging and event publishing
            await LogAuditAndPublishEventsAsync(booking, paymentResult.Data);

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
    private async Task<ResultDto<bool>> ValidateInputAsync(ProcessPaymentCommand request)
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
        var validationResult = await _validationService.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من صلاحيات المستخدم
    /// Validate user authorization
    /// </summary>
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Booking booking)
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
            var unit = await _bookingRepository.GetUnitByBookingIdAsync(booking.Id);
            if (unit?.Property?.OwnerId == currentUserId)
            {
                return ResultDto<bool>.Succeeded(true);
            }
        }

        // الضيف يمكنه معالجة دفعات حجوزاته فقط
        // Guest can process payments for their own bookings only
        if (userRoles.Contains("Guest") && booking.GuestId == currentUserId)
        {
            return ResultDto<bool>.Succeeded(true);
        }

        return ResultDto<bool>.Failed("ليس لديك صلاحية لمعالجة هذا الدفع / You don't have permission to process this payment");
    }

    /// <summary>
    /// التحقق من قواعد الأعمال
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Booking booking, ProcessPaymentCommand request)
    {
        var errors = new List<string>();

        // التحقق من عدم وجود دفعة ناجحة بنفس معرف المعاملة
        // Check for duplicate transaction ID
        var existingPayment = await _paymentRepository.GetByTransactionIdAsync(request.TransactionId);
        if (existingPayment != null && existingPayment.Status == PaymentStatus.Completed)
        {
            errors.Add("معرف المعاملة مستخدم مسبقاً / Transaction ID already exists");
        }

        // التحقق من أن المبلغ لا يتجاوز المبلغ المطلوب للحجز
        // Check that amount doesn't exceed required booking amount
        var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id);
        var remainingAmount = booking.TotalAmount.Amount - totalPaid;
        if (request.Amount.Amount > remainingAmount)
        {
            errors.Add($"المبلغ المدفوع يتجاوز المبلغ المطلوب. المبلغ المتبقي: {remainingAmount} / Payment amount exceeds required amount. Remaining: {remainingAmount}");
        }

        // التحقق من طريقة الدفع المسموحة
        // Validate allowed payment method
        var allowedMethods = await _paymentGatewayService.GetAllowedPaymentMethodsAsync(booking.Unit.PropertyId);
        if (!allowedMethods.Contains(request.PaymentMethod))
        {
            errors.Add("طريقة الدفع غير مسموحة لهذا الكيان / Payment method not allowed for this property");
        }

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
        var validStatuses = new[] { BookingStatus.Pending, BookingStatus.PartiallyPaid, BookingStatus.Confirmed };
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
        if (booking.CheckOutDate < DateTime.UtcNow)
        {
            errors.Add("لا يمكن معالجة الدفع للحجز المنتهي / Cannot process payment for expired booking");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// معالجة الدفع
    /// Process payment
    /// </summary>
    private async Task<ResultDto<Payment>> ProcessPaymentAsync(Booking booking, ProcessPaymentCommand request)
    {
        try
        {
            // معالجة الدفع عبر بوابة الدفع
            // Process payment through payment gateway
            var gatewayResult = await _paymentGatewayService.ProcessPaymentAsync(new PaymentRequest
            {
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = request.TransactionId,
                BookingId = booking.Id,
                Currency = request.Amount.Currency
            });

            if (!gatewayResult.IsSuccessful)
            {
                return ResultDto<Payment>.Failed($"فشل في معالجة الدفع: {gatewayResult.ErrorMessage} / Payment processing failed: {gatewayResult.ErrorMessage}");
            }

            // إنشاء سجل الدفع
            // Create payment record
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = request.TransactionId,
                GatewayTransactionId = gatewayResult.GatewayTransactionId,
                Status = PaymentStatus.Completed,
                ProcessedAt = DateTime.UtcNow,
                ProcessedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId
            };

            // حفظ الدفع في قاعدة البيانات
            // Save payment to database
            await _paymentRepository.AddAsync(payment);

            // تحديث حالة الحجز بناءً على الدفع
            // Update booking status based on payment
            await UpdateBookingStatusAsync(booking, payment);

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
    private async Task UpdateBookingStatusAsync(Booking booking, Payment payment)
    {
        var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id);
        
        if (totalPaid >= booking.TotalAmount.Amount)
        {
            // تم دفع المبلغ كاملاً
            // Full payment completed
            booking.Status = BookingStatus.Confirmed;
            booking.PaidAmount = new MoneyDto { Amount = totalPaid, Currency = booking.TotalAmount.Currency };
        }
        else
        {
            // دفع جزئي
            // Partial payment
            booking.Status = BookingStatus.PartiallyPaid;
            booking.PaidAmount = new MoneyDto { Amount = totalPaid, Currency = booking.TotalAmount.Currency };
        }

        booking.UpdatedAt = DateTime.UtcNow;
        booking.UpdatedBy = _currentUserService.UserId;

        await _bookingRepository.UpdateAsync(booking);
    }

    /// <summary>
    /// تسجيل العملية ونشر الأحداث
    /// Log audit and publish events
    /// </summary>
    private async Task LogAuditAndPublishEventsAsync(Booking booking, Payment payment)
    {
        // تسجيل العملية
        // Audit logging
        await _auditService.LogAsync(new AuditEntry
        {
            UserId = _currentUserService.UserId,
            Action = "ProcessPayment",
            EntityType = "Payment",
            EntityId = payment.Id.ToString(),
            Details = $"تم معالجة دفعة بمبلغ {payment.Amount.Amount} {payment.Amount.Currency} للحجز {booking.Id} / Processed payment of {payment.Amount.Amount} {payment.Amount.Currency} for booking {booking.Id}",
            Timestamp = DateTime.UtcNow
        });

        // نشر حدث معالجة الدفع
        // Publish payment processed event
        var paymentEvent = new PaymentProcessedEvent
        {
            PaymentId = payment.Id,
            BookingId = booking.Id,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            ProcessedAt = payment.ProcessedAt,
            ProcessedBy = payment.ProcessedBy
        };

        await _eventPublisher.PublishAsync(paymentEvent);

        // إرسال إشعار للضيف
        // Send notification to guest
        await _notificationService.SendAsync(new NotificationRequest
        {
            UserId = booking.GuestId,
            Type = NotificationType.PaymentProcessed,
            Title = "تم معالجة الدفع / Payment Processed",
            Message = $"تم معالجة دفعتك بمبلغ {payment.Amount.Amount} {payment.Amount.Currency} بنجاح / Your payment of {payment.Amount.Amount} {payment.Amount.Currency} has been processed successfully",
            Data = new { PaymentId = payment.Id, BookingId = booking.Id }
        });
    }
}
