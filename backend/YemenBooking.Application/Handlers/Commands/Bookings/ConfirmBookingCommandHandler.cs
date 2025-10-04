using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System.Linq;
using YemenBooking.Core.Notifications;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر تأكيد الحجز
/// Confirm booking command handler
/// </summary>
public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, ResultDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ConfirmBookingCommandHandler> _logger;
    private readonly IAvailabilityService _availabilityService;
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly IMediator _mediator;
    private readonly IIndexingService _indexingService;

    public ConfirmBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        INotificationService notificationService,
        ILogger<ConfirmBookingCommandHandler> logger,
        IAvailabilityService availabilityService,
        IUnitAvailabilityRepository availabilityRepository,
        IMediator mediator,
        IIndexingService indexingService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _notificationService = notificationService;
        _logger = logger;
        _availabilityService = availabilityService;
        _availabilityRepository = availabilityRepository;
        _mediator = mediator;
        _indexingService = indexingService;
    }

    public async Task<ResultDto<bool>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر تأكيد الحجز {BookingId}", request.BookingId);

            // التحقق من وجود الحجز
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                return ResultDto<bool>.Failed("الحجز غير موجود");
            }

            // التحقق من الصلاحيات
            var authorizationValidation = await ValidateAuthorizationAsync(request, booking, cancellationToken);
            if (!authorizationValidation.IsSuccess)
            {
                return authorizationValidation;
            }

            // التحقق من حالة الكيان
            var stateValidation = ValidateBookingState(booking);
            if (!stateValidation.IsSuccess)
            {
                return stateValidation;
            }

            // التحقق من قواعد العمل
            var businessRulesValidation = await ValidateBusinessRulesAsync(booking, cancellationToken);
            if (!businessRulesValidation.IsSuccess)
            {
                return businessRulesValidation;
            }

            // التحقق من توفر الوحدة
            if (!await _availabilityService.CheckAvailabilityAsync(booking.UnitId, booking.CheckIn, booking.CheckOut))
            {
                _logger.LogWarning("Unit {UnitId} is no longer available for booking {BookingId}.", booking.UnitId, request.BookingId);
                return ResultDto<bool>.Failure("Unit is no longer available for the selected dates.");
            }

            // تأكيد الحجز
            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            // حفظ التغييرات
            await _unitOfWork.Repository<Booking>().UpdateAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogAsync(
                "ConfirmBooking",
                booking.Id.ToString(),
                "تم تأكيد الحجز",
                _currentUserService.UserId,
                cancellationToken);

            // إرسال حدث تأكيد الحجز
            await _eventPublisher.PublishAsync(new BookingConfirmedEvent
            {
                BookingId = booking.Id,
                ConfirmedBookingId = booking.Id,
                UserId = booking.UserId,
                UnitId = booking.UnitId,
                PropertyId = booking.Unit.PropertyId,
                ConfirmedAt = DateTime.UtcNow,
                ConfirmedBy = _currentUserService.UserId,
                ConfirmedAmount = booking.TotalPrice.Amount,
                CheckInDate = booking.CheckIn,
                CheckOutDate = booking.CheckOut,
                ConfirmationNotes = "",
                OccurredAt = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                EventType = nameof(BookingConfirmedEvent),
                Version = 1,
                CorrelationId = booking.Id.ToString()
            }, cancellationToken);

            // تحديث مباشر لفهرس الإتاحة
            try
            {
                var from = DateTime.UtcNow.Date;
                var to = from.AddMonths(6);
                var periods = await _availabilityRepository.GetByDateRangeAsync(booking.UnitId, from, to);
                var availableRanges = periods
                    .Where(a => a.Status == "Available")
                    .Select(p => (p.StartDate, p.EndDate))
                    .ToList();

                await _indexingService.OnAvailabilityChangedAsync(booking.UnitId, booking.Unit.PropertyId, availableRanges, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "تعذرت الفهرسة المباشرة للإتاحة بعد تأكيد الحجز {BookingId}", booking.Id);
            }

            // إرسال إشعار للضيف
            await _notificationService.SendAsync(new NotificationRequest
            {
                UserId = booking.UserId,
                Type = NotificationType.BookingConfirmed,
                Title = "تم تأكيد الحجز / Booking Confirmed",
                Message = $"تم تأكيد حجزك بنجاح للحجز رقم {booking.Id} / Your booking {booking.Id} has been confirmed successfully",
                Data = new { BookingId = booking.Id }
            }, cancellationToken);

            // إرسال إشعار لمالك الكيان
            var unitEntity = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(booking.UnitId, cancellationToken);
            if (unitEntity != null)
            {
                var propertyEntity = await _unitOfWork.Repository<Property>().GetByIdAsync(unitEntity.PropertyId, cancellationToken);
                if (propertyEntity != null && propertyEntity.OwnerId != Guid.Empty)
                {
                    await _notificationService.SendAsync(new NotificationRequest
                    {
                        UserId = propertyEntity.OwnerId,
                        Type = NotificationType.BookingConfirmed,
                        Title = "تم تأكيد حجز / Booking Confirmed",
                        Message = $"تم تأكيد حجز في وحدتك للحجز رقم {booking.Id} / A booking in your unit has been confirmed for booking {booking.Id}",
                        Data = new { BookingId = booking.Id, UnitId = unitEntity.Id }
                    }, cancellationToken);
                }
            }

            _logger.LogInformation("تم تأكيد الحجز {BookingId} بنجاح", booking.Id);

            return ResultDto<bool>.Succeeded(true, "تم تأكيد الحجز بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تأكيد الحجز {BookingId}", request.BookingId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء تأكيد الحجز");
        }
    }

    /// <summary>
    /// التحقق من الصلاحيات
    /// Authorization validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(ConfirmBookingCommand request, Booking booking, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var errors = new List<string>();

        // التحقق من صحة الطلب
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors.Select(e => e.Message));
        }

        // التحقق من أن المستخدم لديه الصلاحية لتأكيد الحجز
        if (_currentUserService.Role != "Admin" && _currentUserService.Role != "Staff")
        {
            errors.Add("ليس لديك الصلاحية لتأكيد هذا الحجز / You do not have permission to confirm this booking");
        }

        // التحقق من أن المستخدم هو موظف في الكيان
        if (_currentUserService.Role != "Admin" && !_currentUserService.IsStaffInProperty(booking.Unit.PropertyId))
        {
            errors.Add("لست موظفًا في هذا الكيان / You are not a staff member of this property");
        }

        if (errors.Any())
        {
            return ResultDto<bool>.Failed(string.Join("\n", errors));
        }

        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من حالة الحجز
    /// Booking state validation
    /// </summary>
    private ResultDto<bool> ValidateBookingState(Booking booking)
    {
        // يمكن تأكيد الحجوزات المعلقة فقط
        if (booking.Status != BookingStatus.Pending)
        {
            return ResultDto<bool>.Failed($"لا يمكن تأكيد الحجز في الحالة الحالية: {booking.Status}");
        }

        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Business rules validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Booking booking, CancellationToken cancellationToken)
    {
        // التحقق من اكتمال الدفع إذا كان مطلوباً
        var payments = await _unitOfWork.Repository<Payment>()
            .FindAsync(p => p.BookingId == booking.Id && p.Status == PaymentStatus.Successful, cancellationToken);

        var totalPaid = payments.Sum(p => p.Amount.Amount);
        var requiredAmount = booking.TotalPrice.Amount;

        // التحقق من سياسة الدفع للكيان
        var unit = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(booking.UnitId, cancellationToken);
        var property = await _unitOfWork.Repository<Property>().GetByIdAsync(unit.PropertyId, cancellationToken);
        var paymentPolicy = await GetPropertyPaymentPolicyAsync(property.Id, cancellationToken);

        if (paymentPolicy != null && paymentPolicy.RequireFullPaymentBeforeConfirmation)
        {
            if (totalPaid < requiredAmount)
            {
                return ResultDto<bool>.Failed($"يجب دفع المبلغ كاملاً ({requiredAmount} {booking.TotalPrice.Currency}) قبل التأكيد");
            }
        }
        else if (paymentPolicy != null && paymentPolicy.MinimumDepositPercentage > 0)
        {
            var minimumRequired = requiredAmount * (paymentPolicy.MinimumDepositPercentage / 100);
            if (totalPaid < minimumRequired)
            {
                return ResultDto<bool>.Failed($"يجب دفع مبلغ لا يقل عن {minimumRequired} {booking.TotalPrice.Currency} قبل التأكيد");
            }
        }

        // التحقق من أن تاريخ الوصول لم يمضي
        if (booking.CheckIn.Date < DateTime.Today)
        {
            return ResultDto<bool>.Failed("لا يمكن تأكيد حجز انتهت فترة الوصول المحددة له");
        }

        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// الحصول على سياسة الدفع للكيان
    /// Get property payment policy
    /// </summary>
    private async Task<PropertyPolicy> GetPropertyPaymentPolicyAsync(Guid propertyId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<PropertyPolicy>()
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.Type == PolicyType.Payment, cancellationToken);
    }
}

/// <summary>
/// حدث تأكيد الحجز
/// Booking confirmed event
/// </summary>
public class BookingConfirmedEvent : IBookingConfirmedEvent
{
    public Guid BookingId { get; set; }
    public Guid ConfirmedBookingId { get; set; }
    public Guid? UserId { get; set; }
    public Guid UnitId { get; set; }
    public Guid PropertyId { get; set; }
    public DateTime ConfirmedAt { get; set; }
    public Guid? ConfirmedBy { get; set; }
    public decimal ConfirmedAmount { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string ConfirmationNotes { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(BookingConfirmedEvent);
    public int Version { get; set; } = 1;
    public string CorrelationId { get; set; } = string.Empty;
}
