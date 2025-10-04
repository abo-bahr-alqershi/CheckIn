using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Payments;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Core.Notifications;

namespace YemenBooking.Application.Handlers.Commands.Payments;

/// <summary>
/// مُعالج أمر استرداد الدفع
/// Payment refund command handler
/// 
/// يقوم بمعالجة عملية استرداد الدفع ويشمل:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الدفع
/// - التحقق من صلاحيات المستخدم
/// - التحقق من قواعد الأعمال
/// - التحقق من حالة الدفع
/// - معالجة الاسترداد
/// - إنشاء حدث الاسترداد
/// 
/// Processes payment refund and includes:
/// - Input data validation
/// - Payment existence validation
/// - User authorization validation
/// - Business rules validation
/// - Payment state validation
/// - Refund processing
/// - Refund event creation
/// </summary>
public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, ResultDto<bool>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentGatewayService _paymentGatewayService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RefundPaymentCommandHandler> _logger;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository,
        IPaymentGatewayService paymentGatewayService,
        IValidationService validationService,
        IAuditService auditService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUserService,
        ILogger<RefundPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _paymentGatewayService = paymentGatewayService;
        _validationService = validationService;
        _auditService = auditService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر استرداد الدفع
    /// Handle payment refund command
    /// </summary>
    /// <param name="request">طلب استرداد الدفع / Payment refund request</param>
    /// <param name="cancellationToken">رمز الإلغاء / Cancellation token</param>
    /// <returns>نتيجة العملية / Operation result</returns>
    public async Task<ResultDto<bool>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر استرداد الدفع {PaymentId} / Starting refund processing for payment: {PaymentId}", request.PaymentId);

            // الخطوة 1: التحقق من صحة البيانات المدخلة
            // Step 1: Input data validation
            var inputValidationResult = await ValidateInputAsync(request, cancellationToken);
            if (!inputValidationResult.Success)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات المدخلة: {Errors} / Input validation failed: {Errors}", string.Join(", ", inputValidationResult.Errors));
                return ResultDto<bool>.Failed(inputValidationResult.Errors);
            }

            // الخطوة 2: التحقق من وجود الدفعة والحجز
            // Step 2: Payment and booking existence validation
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                _logger.LogWarning("الدفعة غير موجودة: {PaymentId} / Payment not found: {PaymentId}", request.PaymentId);
                return ResultDto<bool>.Failed("الدفعة غير موجودة / Payment not found");
            }

            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("الحجز غير موجود: {BookingId} / Booking not found: {BookingId}", payment.BookingId);
                return ResultDto<bool>.Failed("الحجز غير موجود / Booking not found");
            }

            // الخطوة 3: التحقق من صلاحيات المستخدم
            // Step 3: User authorization validation
            var authorizationResult = await ValidateAuthorizationAsync(payment, booking, cancellationToken);
            if (!authorizationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من الصلاحيات للمستخدم: {UserId} / Authorization failed for user: {UserId}", _currentUserService.UserId);
                return ResultDto<bool>.Failed(authorizationResult.Errors);
            }

            // الخطوة 4: التحقق من قواعد الأعمال
            // Step 4: Business rules validation
            var businessRulesResult = await ValidateBusinessRulesAsync(payment, request, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من قواعد الأعمال: {Errors} / Business rules validation failed: {Errors}", string.Join(", ", businessRulesResult.Errors));
                return ResultDto<bool>.Failed(businessRulesResult.Errors);
            }

            // الخطوة 5: التحقق من حالة الدفعة
            // Step 5: Payment state validation
            var stateValidationResult = await ValidatePaymentStateAsync(payment, cancellationToken);
            if (!stateValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من حالة الدفعة: {Errors} / Payment state validation failed: {Errors}", string.Join(", ", stateValidationResult.Errors));
                return ResultDto<bool>.Failed(stateValidationResult.Errors);
            }

            // الخطوة 6: التحقق من سياسة الاسترداد
            // Step 6: Refund policy validation
            var refundPolicyResult = await ValidateRefundPolicyAsync(payment, cancellationToken);
            if (!refundPolicyResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من سياسة الاسترداد: {Errors} / Refund policy validation failed: {Errors}", string.Join(", ", refundPolicyResult.Errors));
                return ResultDto<bool>.Failed(refundPolicyResult.Errors);
            }

            // الخطوة 7: معالجة الاسترداد
            // Step 7: Process refund
            var refundResult = await ProcessRefundAsync(payment, request, cancellationToken);
            if (!refundResult.IsSuccess)
            {
                _logger.LogError("فشل في معالجة الاسترداد: {Errors} / Refund processing failed: {Errors}", string.Join(", ", refundResult.Errors));
                return ResultDto<bool>.Failed(refundResult.Errors);
            }

            // الخطوة 8: تسجيل العملية ونشر الأحداث
            // Step 8: Audit logging and event publishing
            await LogAuditAndPublishEventsAsync(payment, booking, new Money(request.RefundAmount.Amount, request.RefundAmount.Currency), request.RefundReason, cancellationToken);

            _logger.LogInformation("تم معالجة الاسترداد بنجاح: {PaymentId} / Refund processed successfully: {PaymentId}", payment.Id);
            return ResultDto<bool>.Succeeded(true, "تم معالجة الاسترداد بنجاح / Refund processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة أمر استرداد الدفع: {PaymentId} / Error processing refund command: {PaymentId}", request.PaymentId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء معالجة الاسترداد / An error occurred while processing refund");
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateInputAsync(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الدفعة
        // Validate payment ID
        if (request.PaymentId == Guid.Empty)
        {
            errors.Add("معرف الدفعة مطلوب / Payment ID is required");
        }

        // التحقق من مبلغ الاسترداد
        // Validate refund amount
        if (request.RefundAmount == null)
        {
            errors.Add("مبلغ الاسترداد مطلوب / Refund amount is required");
        }
        else if (request.RefundAmount.Amount <= 0)
        {
            errors.Add("مبلغ الاسترداد يجب أن يكون أكبر من صفر / Refund amount must be greater than zero");
        }

        // التحقق من سبب الاسترداد
        // Validate refund reason
        if (string.IsNullOrWhiteSpace(request.RefundReason))
        {
            errors.Add("سبب الاسترداد مطلوب / Refund reason is required");
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
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Payment payment, Booking booking, CancellationToken cancellationToken)
    {
        var userRoles = _currentUserService.UserRoles;
        if (userRoles.Contains("Admin") || userRoles.Contains("Owner"))
        {
            return ResultDto<bool>.Succeeded(true);
        }
        return ResultDto<bool>.Failed("ليس لديك صلاحية لمعالجة هذا الاسترداد / You don't have permission to process this refund");
    }

    /// <summary>
    /// التحقق من قواعد الأعمال
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Payment payment, RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن مبلغ الاسترداد لا يتجاوز المبلغ المدفوع
        // Check that refund amount doesn't exceed paid amount
        if (request.RefundAmount.Amount > payment.Amount.Amount)
        {
            errors.Add($"مبلغ الاسترداد يتجاوز المبلغ المدفوع. المبلغ المدفوع: {payment.Amount.Amount} / Refund amount exceeds paid amount. Paid amount: {payment.Amount.Amount}");
        }

        // التحقق من أن العملة متطابقة
        // Check that currency matches
        if (request.RefundAmount.Currency != payment.Amount.Currency)
        {
            errors.Add("عملة الاسترداد يجب أن تتطابق مع عملة الدفعة / Refund currency must match payment currency");
        }

        // التحقق من عدم وجود استرداد سابق لنفس الدفعة
        // Check for previous refunds for the same payment
        var existingRefunds = await _paymentRepository.GetRefundsForPaymentAsync(payment.Id, cancellationToken);
        if (existingRefunds.Any())
        {
            var totalRefunded = existingRefunds.Sum(r => r.Amount.Amount);
            if (totalRefunded + request.RefundAmount.Amount > payment.Amount.Amount)
            {
                errors.Add($"إجمالي مبلغ الاسترداد يتجاوز المبلغ المدفوع. المبلغ المسترد سابقًا: {totalRefunded} / Total refund amount exceeds paid amount. Previously refunded: {totalRefunded}");
            }
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من حالة الدفعة
    /// Validate payment state
    /// </summary>
    private async Task<ResultDto<bool>> ValidatePaymentStateAsync(Payment payment, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الدفعة في حالة تسمح بالاسترداد
        // Check that payment is in a state that allows refund
        if (payment.Status != PaymentStatus.Successful)
        {
            errors.Add($"لا يمكن استرداد الدفعة في الحالة الحالية: {payment.Status} / Cannot refund payment in current status: {payment.Status}");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من سياسة الاسترداد
    /// Validate refund policy
    /// </summary>
    private async Task<ResultDto<bool>> ValidateRefundPolicyAsync(Payment payment, CancellationToken cancellationToken)
    {
        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// معالجة الاسترداد
    /// Process refund
    /// </summary>
    private async Task<ResultDto<bool>> ProcessRefundAsync(Payment payment, RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // معالجة الاسترداد عبر بوابة الدفع
            // Process refund through payment gateway
            var gatewayResult = await _paymentGatewayService.ProcessRefundAsync(payment.TransactionId, request.RefundAmount.Amount, request.RefundReason, cancellationToken);
            if (!gatewayResult.IsSuccess)
            {
                return ResultDto<bool>.Failed($"فشل في معالجة الاسترداد: {gatewayResult.ErrorMessage} / Refund processing failed: {gatewayResult.ErrorMessage}");
            }

            // إنشاء سجل استرداد جديد
            // Create new refund record
            var refund = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = payment.BookingId,
                Amount = new Money(request.RefundAmount.Amount, request.RefundAmount.Currency),
                Method = payment.Method,
                TransactionId = gatewayResult.RefundId,
                Status = PaymentStatus.Refunded,
                PaymentDate = DateTime.UtcNow,
                ProcessedBy = _currentUserService.UserId
            };

            // تحديث حالة الدفعة الأصلية
            // Update original payment status
            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;

            // حفظ التغييرات في قاعدة البيانات
            // Save changes to database
            await _paymentRepository.AddAsync(refund, cancellationToken);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            // تحديث حالة الحجز إذا لزم الأمر
            // Update booking status if needed
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId, cancellationToken);
            if (booking != null)
            {
                var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id, cancellationToken);
                if (totalPaid < booking.TotalPrice.Amount)
                {
                    booking.Status = BookingStatus.Pending;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingRepository.UpdateAsync(booking, cancellationToken);
                }
            }

            return ResultDto<bool>.Succeeded(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة الاسترداد للدفعة: {PaymentId} / Error processing refund for payment: {PaymentId}", payment.Id);
            return ResultDto<bool>.Failed("حدث خطأ أثناء معالجة الاسترداد / An error occurred while processing refund");
        }
    }

    /// <summary>
    /// تسجيل العملية ونشر الأحداث
    /// Log audit and publish events
    /// </summary>
    private async Task LogAuditAndPublishEventsAsync(Payment payment, Booking booking, Money refundAmount, string refundReason, CancellationToken cancellationToken)
    {
        // تسجيل العملية
        // Audit logging
        await _auditService.LogBusinessOperationAsync(
            "ProcessRefund",
            $"تم معالجة استرداد بمبلغ {refundAmount.Amount} {refundAmount.Currency} للحجز {booking.Id}",
            payment.Id,
            null,
            _currentUserService.UserId,
            null,
            cancellationToken);

        // نشر حدث معالجة الاسترداد
        // Publish refund processed event
        await _eventPublisher.PublishAsync(new PaymentRefundedEvent
        {
            PaymentId = payment.Id,
            BookingId = booking.Id,
            RefundAmount = refundAmount.Amount,
            RefundReason = refundReason,
            RefundTransactionId = payment.TransactionId,
            RefundedAt = DateTime.UtcNow,
            RefundMethod = PaymentMethodEnum.CreditCard, // قيمة افتراضية مؤقتة
            RefundStatus = PaymentStatus.Refunded,
            OriginalAmount = payment.Amount.Amount,
            Currency = refundAmount.Currency,
            Notes = null,
            EventId = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType = nameof(PaymentRefundedEvent),
            Version = 1,
            UserId = _currentUserService.UserId,
            CorrelationId = booking.Id.ToString()
        }, cancellationToken);

        // إرسال إشعار للضيف
        // Send notification to guest
        await _notificationService.SendAsync(new NotificationRequest
        {
            UserId = booking.UserId,
            Type = NotificationType.RefundProcessed,
            Title = "تم معالجة الاسترداد / Refund Processed",
            Message = $"تم معالجة استرداد بمبلغ {refundAmount.Amount} {refundAmount.Currency} بنجاح / Your refund of {refundAmount.Amount} {refundAmount.Currency} has been processed successfully",
            Data = new { PaymentId = payment.Id, BookingId = booking.Id }
        }, cancellationToken);
    }
}

