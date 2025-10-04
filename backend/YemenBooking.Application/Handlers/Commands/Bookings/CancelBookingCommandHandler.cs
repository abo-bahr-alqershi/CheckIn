using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Core.Interfaces.Repositories;
using System.Linq;
using YemenBooking.Core.Notifications;
using YemenBooking.Core.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using System.Linq;
using YemenBooking.Application.Events;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر إلغاء الحجز
/// Cancel booking command handler
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ResultDto<bool>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitAvailabilityRepository _unitAvailabilityRepository;
    private readonly ILogger<CancelBookingCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly IIndexingService _indexingService;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository,
        IPaymentRepository paymentRepository,
        IValidationService validationService,
        IAuditService auditService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUserService,
        IUnitAvailabilityRepository unitAvailabilityRepository,
        ILogger<CancelBookingCommandHandler> logger,
        IMediator mediator,
        IUnitAvailabilityRepository availabilityRepository,
        IIndexingService indexingService)
    {
        _bookingRepository = bookingRepository;
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
        _paymentRepository = paymentRepository;
        _validationService = validationService;
        _auditService = auditService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
        _unitAvailabilityRepository = unitAvailabilityRepository;
        _logger = logger;
        _mediator = mediator;
        _availabilityRepository = availabilityRepository;
        _indexingService = indexingService;
    }

    public async Task<ResultDto<bool>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر إلغاء الحجز {BookingId} / Starting cancel booking processing for booking: {BookingId}", request.BookingId);

            // الخطوة 1: التحقق من صحة البيانات المدخلة
            // Step 1: Input data validation
            var inputValidationResult = await ValidateInputAsync(request, cancellationToken);
            if (!inputValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات المدخلة: {Errors} / Input validation failed: {Errors}", string.Join(", ", inputValidationResult.Errors));
                return ResultDto<bool>.Failed(inputValidationResult.Errors);
            }

            // الخطوة 2: التحقق من وجود الحجز
            // Step 2: Booking existence validation
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("Booking not found for ID: {BookingId}", request.BookingId);
                return ResultDto<bool>.Failure("Booking not found.");
            }

            // الخطوة 3: التحقق من صلاحيات المستخدم
            // Step 3: User authorization validation
            var authorizationResult = await ValidateAuthorizationAsync(booking, cancellationToken);
            if (!authorizationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من الصلاحيات للمستخدم: {UserId} / Authorization failed for user: {UserId}", _currentUserService.UserId);
                return ResultDto<bool>.Failed(authorizationResult.Errors);
            }

            // الخطوة 4: التحقق من قواعد الأعمال
            // Step 4: Business rules validation
            var businessRulesResult = await ValidateBusinessRulesAsync(booking, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من قواعد الأعمال: {Errors} / Business rules validation failed: {Errors}", string.Join(", ", businessRulesResult.Errors));
                return ResultDto<bool>.Failed(businessRulesResult.Errors);
            }

            // الخطوة 5: التحقق من حالة الحجز
            // Step 5: Booking state validation
            var stateValidationResult = await ValidateBookingStateAsync(booking, cancellationToken);
            if (!stateValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من حالة الحجز: {Errors} / Booking state validation failed: {Errors}", string.Join(", ", stateValidationResult.Errors));
                return ResultDto<bool>.Failed(stateValidationResult.Errors);
            }

            // الخطوة 6: التحقق من سياسة الإلغاء
            // Step 6: Cancellation policy validation
            var cancellationPolicyResult = await ValidateCancellationPolicyAsync(booking, cancellationToken);
            if (!cancellationPolicyResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من سياسة الإلغاء: {Errors} / Cancellation policy validation failed: {Errors}", string.Join(", ", cancellationPolicyResult.Errors));
                return ResultDto<bool>.Failed(cancellationPolicyResult.Errors);
            }

            // الخطوة 7: معالجة الإلغاء
            // Step 7: Process cancellation
            // تسجيل سبب الإلغاء على الكيان
            booking.CancellationReason = request.CancellationReason;
            var cancellationResult = await ProcessCancellationAsync(booking, cancellationToken);
            if (!cancellationResult.IsSuccess)
            {
                _logger.LogError("فشل في معالجة إلغاء الحجز: {Errors} / Cancellation processing failed: {Errors}", string.Join(", ", cancellationResult.Errors));
                return ResultDto<bool>.Failed(cancellationResult.Errors);
            }

            // الخطوة 8: تسجيل العملية ونشر الأحداث
            // Step 8: Audit logging and event publishing
            await LogAuditAndPublishEventsAsync(booking, cancellationToken);

            // تحديث مباشر لفهرس الإتاحة
            try
            {
                var bookingToUpdate = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
                if (bookingToUpdate != null)
                {
                    var from = DateTime.UtcNow.Date;
                    var to = from.AddMonths(6);
                    var periods = await _availabilityRepository.GetByDateRangeAsync(bookingToUpdate.UnitId, from, to);
                    var availableRanges = periods
                        .Where(p => p.Status == "Available")
                        .Select(p => (p.StartDate, p.EndDate))
                        .ToList();

                    var unit = await _unitRepository.GetByIdAsync(bookingToUpdate.UnitId, cancellationToken);
                    var propertyId = unit?.PropertyId ?? Guid.Empty;

                    await _indexingService.OnAvailabilityChangedAsync(bookingToUpdate.UnitId, propertyId, availableRanges, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "تعذرت الفهرسة المباشرة للإتاحة بعد إلغاء الحجز {BookingId}", request.BookingId);
            }

            _logger.LogInformation("تم إلغاء الحجز بنجاح: {BookingId} / Booking cancelled successfully: {BookingId}", booking.Id);
            return ResultDto<bool>.Succeeded(true, "تم إلغاء الحجز بنجاح / Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة أمر إلغاء الحجز: {BookingId} / Error processing cancel booking command: {BookingId}", request.BookingId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء إلغاء الحجز / An error occurred while cancelling booking");
        }
    }

    private async Task<ResultDto<bool>> ValidateInputAsync(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الحجز
        // Validate booking ID
        if (request.BookingId == Guid.Empty)
        {
            errors.Add("معرف الحجز مطلوب / Booking ID is required");
        }

        // التحقق من سبب الإلغاء
        // Validate cancellation reason
        if (string.IsNullOrWhiteSpace(request.CancellationReason))
        {
            errors.Add("سبب الإلغاء مطلوب / Cancellation reason is required");
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

    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Booking booking, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        // التحقق من أن المستخدم مسؤول
        if (_currentUserService.Role == "Admin")
        {
            return ResultDto<bool>.Succeeded(true);
        }

        // التحقق من كون المستخدم مالك الكيان
        if (_currentUserService.Role == "Owner")
        {
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit != null)
            {
                var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
                if (property != null && property.OwnerId == currentUserId)
                    return ResultDto<bool>.Succeeded(true);
            }
        }

        // التحقق من أن الضيف يحجز بنفسه
        if (booking.UserId == currentUserId)
        {
            return ResultDto<bool>.Succeeded(true);
        }

        return ResultDto<bool>.Failed("ليس لديك صلاحية لإلغاء هذا الحجز / You don't have permission to cancel this booking");
    }

    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Booking booking, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الحجز لم يتم إلغاؤه مسبقًا
        // Check that booking has not been cancelled already
        if (booking.Status == BookingStatus.Cancelled)
        {
            errors.Add("الحجز تم إلغاؤه مسبقًا / Booking has already been cancelled");
        }

        // التحقق من أن الحجز لم ينته بعد
        // Check that booking has not ended yet
        if (booking.CheckOut < DateTime.UtcNow)
        {
            errors.Add("لا يمكن إلغاء حجز منتهي / Cannot cancel an ended booking");
        }

        // التحقق من الدفعات المرتبطة
        // Check associated payments
        var totalPaid = await _paymentRepository.GetTotalPaidAmountAsync(booking.Id, cancellationToken);
        if (totalPaid > 0)
        {
            errors.Add($"لا يمكن إلغاء حجز تم الدفع له. المبلغ المدفوع: {totalPaid} / Cannot cancel a booking with payments. Paid amount: {totalPaid}");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    private async Task<ResultDto<bool>> ValidateBookingStateAsync(Booking booking, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الحجز في حالة تسمح بالإلغاء
        // Check that booking is in a state that allows cancellation
        var validStatuses = new[] { BookingStatus.Pending, BookingStatus.Confirmed };
        if (!validStatuses.Contains(booking.Status))
        {
            errors.Add($"لا يمكن إلغاء الحجز في الحالة الحالية: {booking.Status} / Cannot cancel booking in current status: {booking.Status}");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    private async Task<ResultDto<bool>> ValidateCancellationPolicyAsync(Booking booking, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من سياسة الإلغاء: أولوية للوحدة ثم سياسة الكيان
        var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
        if (unit != null)
        {
            // إذا كانت الوحدة لا تسمح بالإلغاء على الإطلاق
            if (!unit.AllowsCancellation)
            {
                errors.Add("هذه الوحدة لا تسمح بإلغاء الحجز / This unit does not allow cancellations");
            }
            else
            {
                // إذا حددت الوحدة نافذة إلغاء، استخدمها، وإلا استخدم سياسة العقار
                int? windowDays = unit.CancellationWindowDays;
                if (!windowDays.HasValue)
                {
                    var propertyPolicy = await _propertyRepository.GetCancellationPolicyAsync(unit.PropertyId, cancellationToken);
                    windowDays = propertyPolicy?.CancellationWindowDays;
                }
                if (windowDays.HasValue)
                {
                    var daysBeforeCheckIn = (booking.CheckIn - DateTime.UtcNow).TotalDays;
                    if (daysBeforeCheckIn < windowDays.Value)
                    {
                        errors.Add($"لا يمكن الإلغاء. يجب الإلغاء قبل {windowDays.Value} أيام من تاريخ الوصول / Cannot cancel. Must cancel {windowDays.Value} days before check-in date");
                    }
                }
            }
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    private async Task<ResultDto<bool>> ProcessCancellationAsync(Booking booking, CancellationToken cancellationToken)
    {
        try
        {
            // تحديث حالة الحجز إلى ملغى
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            // إزالة سجلات الإتاحة المرتبطة بهذا الحجز
            await _unitAvailabilityRepository.DeleteRangeAsync(u => u.BookingId == booking.Id, cancellationToken);

            // تحرير الوحدة من الحجز
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit != null)
            {
                unit.IsAvailable = true;
                unit.UpdatedAt = DateTime.UtcNow;
                await _unitRepository.UpdateAsync(unit, cancellationToken);
            }

            // حفظ التغييرات في قاعدة البيانات
            await _bookingRepository.UpdateAsync(booking, cancellationToken);
            await _bookingRepository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Succeeded(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة إلغاء الحجز {BookingId} / Error processing cancellation for booking {BookingId}", booking.Id);
            return ResultDto<bool>.Failed("حدث خطأ أثناء إلغاء الحجز / An error occurred while cancelling booking");
        }
    }

    private async Task LogAuditAndPublishEventsAsync(Booking booking, CancellationToken cancellationToken)
    {
        // تسجيل العملية
        // Audit logging
        await _auditService.LogAsync(
            "CancelBooking",
            booking.Id.ToString(),
            $"تم إلغاء الحجز / Booking cancelled",
            _currentUserService.UserId,
            cancellationToken);

        // نشر حدث إلغاء الحجز
        // Publish booking cancelled event
        await _eventPublisher.PublishAsync(new BookingCancelledEvent
        {
            BookingId = booking.Id,
            CancellationReason = booking.CancellationReason,
            CancellationType = "User",
            RefundProcessed = false,
            RefundAmount = Money.Zero(booking.TotalPrice.Currency),
            CancellationFee = Money.Zero(booking.TotalPrice.Currency),
            CancelledAt = DateTime.UtcNow,
            CancelledBy = _currentUserService.UserId,
            UnitId = booking.UnitId
        }, cancellationToken);

        // إرسال إشعار للضيف
        // Send notification to guest
        await _notificationService.SendAsync(new NotificationRequest
        {
            UserId = booking.UserId,
            Type = NotificationType.BookingCancelled,
            Title = "تم إلغاء الحجز / Booking Cancelled",
            Message = $"تم إلغاء حجزك بنجاح / Your booking has been cancelled successfully",
            Data = new { BookingId = booking.Id }
        }, cancellationToken);

        // إرسال إشعار لمالك الكيان
        // Send notification to property owner
        var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
        if (unit != null)
        {
            var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
            if (property != null && property.OwnerId != Guid.Empty)
            {
                await _notificationService.SendAsync(new NotificationRequest
                {
                    UserId = property.OwnerId,
                    Type = NotificationType.BookingCancelled,
                    Title = "تم إلغاء حجز / Booking Cancelled",
                    Message = $"تم إلغاء حجز في وحدتك / A booking in your unit has been cancelled",
                    Data = new { BookingId = booking.Id, UnitId = unit.Id }
                }, cancellationToken);
            }
        }
    }
}

/// <summary>
/// نتيجة حساب الإلغاء
/// Cancellation calculation result
/// </summary>
public class CancellationCalculation
{
    public Money RefundAmount { get; set; }
    public Money CancellationFee { get; set; }
}

/// <summary>
/// حدث إلغاء الحجز
/// Booking cancelled event
/// </summary>
public class BookingCancelledEvent : IBookingCancelledEvent
{
    public Guid BookingId { get; set; }
    public Guid UnitId { get; set; }
    public Guid? UserId { get; set; }
    public string CancellationReason { get; set; }
    public Money CancellationFee { get; set; }
    public Money RefundAmount { get; set; }
    public DateTime CancelledAt { get; set; }
    public Guid CancelledBy { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(BookingCancelledEvent);
    public int Version { get; set; } = 1;
    public string CorrelationId { get; set; }
    public string CancellationType { get; set; }
    public bool RefundProcessed { get; set; }
}
