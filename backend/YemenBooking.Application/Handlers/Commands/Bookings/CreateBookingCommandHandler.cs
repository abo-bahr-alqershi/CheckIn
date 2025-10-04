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
using System.Linq;
using YemenBooking.Application.Interfaces;
using YemenBooking.Core.Notifications;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر إنشاء حجز جديد
/// Create booking command handler
/// </summary>
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ResultDto<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAvailabilityService _availabilityService;
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly IPricingService _pricingService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateBookingCommandHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyServiceRepository _propertyServiceRepository;
    private readonly IMediator _mediator;
    private readonly IIndexingService _indexingService;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitRepository unitRepository,
        IPropertyServiceRepository propertyServiceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAvailabilityService availabilityService,
        IUnitAvailabilityRepository availabilityRepository,
        IPricingService pricingService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<CreateBookingCommandHandler> logger,
        INotificationService notificationService,
        IMediator mediator,
        IIndexingService indexingService)
    {
        _bookingRepository = bookingRepository;
        _unitRepository = unitRepository;
        _propertyServiceRepository = propertyServiceRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _availabilityService = availabilityService;
        _availabilityRepository = availabilityRepository;
        _pricingService = pricingService;
        _validationService = validationService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _notificationService = notificationService;
        _mediator = mediator;
        _indexingService = indexingService;
    }

    public async Task<ResultDto<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر إنشاء حجز جديد للمستخدم {UserId} والوحدة {UnitId}", 
                request.UserId, request.UnitId);

            // التحقق من صحة المدخلات
            var validationResult = await ValidateInputAsync(request, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود المستخدم والوحدة
            var existenceValidation = await ValidateExistenceAsync(request, cancellationToken);
            if (!existenceValidation.IsSuccess)
            {
                return existenceValidation;
            }

            // التحقق من الصلاحيات
            var authorizationValidation = await ValidateAuthorizationAsync(request, cancellationToken);
            if (!authorizationValidation.IsSuccess)
            {
                return authorizationValidation;
            }

            // التحقق من قواعد العمل
            var businessRulesValidation = await ValidateBusinessRulesAsync(request, cancellationToken);
            if (!businessRulesValidation.IsSuccess)
            {
                return businessRulesValidation;
            }

            // التحقق من توافر الوحدة للفترة المطلوبة
            var isAvailable = await _availabilityService.CheckAvailabilityAsync(
                request.UnitId,
                request.CheckIn,
                request.CheckOut);
            if (!isAvailable)
            {
                return ResultDto<Guid>.Failed("لا يمكن إنشاء الحجز؛ الوحدة غير متاحة للفترة المحددة");
            }

            // حساب السعر الإجمالي
            var priceAmount = await _pricingService.CalculatePriceAsync(
                request.UnitId,
                request.CheckIn,
                request.CheckOut);
            // إنشاء كائن Money بالعملة الافتراضية (YER)
            var totalPrice = Money.Yer(priceAmount);

            // إنشاء كيان الحجز
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                UnitId = request.UnitId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                GuestsCount = request.GuestsCount,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // حفظ الحجز
            await _unitOfWork.Repository<Booking>().AddAsync(booking, cancellationToken);

            // إنشاء سجل توافر لحجز الوحدة
            var availability = new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = booking.UnitId,
                StartDate = booking.CheckIn,
                EndDate = booking.CheckOut,
                Status = "unavailable",
                Reason = "booking",
                Notes = $"Block for booking {booking.Id}",
                BookingId = booking.Id
            };
            await _unitOfWork.Repository<UnitAvailability>().AddAsync(availability, cancellationToken);

            // حفظ كل التغييرات في معاملة واحدة
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // تسجيل تنفيذ كتلة الإتاحة في سجل التدقيق
            await _auditService.LogAsync(
                "BlockAvailability",
                availability.Id.ToString(),
                $"تم حجز الوحدة {booking.UnitId} للحجز {booking.Id}",
                _currentUserService.UserId,
                cancellationToken);

            // تحديث مباشر لفهرس الإتاحة
            try
            {
                var from = DateTime.UtcNow.Date;
                var to = from.AddMonths(6);
                var periods = await _availabilityRepository.GetByDateRangeAsync(booking.UnitId, from, to);
                var availableRanges = periods
                    .Where(p => p.Status == "Available")
                    .Select(p => (p.StartDate, p.EndDate))
                    .ToList();

                var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
                var propertyId = unit?.PropertyId ?? Guid.Empty;

                await _indexingService.OnAvailabilityChangedAsync(booking.UnitId, propertyId, availableRanges, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "تعذرت الفهرسة المباشرة للإتاحة بعد إنشاء الحجز {BookingId}", booking.Id);
            }

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogAsync(
                "CreateBooking",
                booking.Id.ToString(),
                $"تم إنشاء حجز جديد للمستخدم {request.UserId}",
                _currentUserService.UserId,
                cancellationToken);

            // إرسال حدث إنشاء الحجز
            await _eventPublisher.PublishAsync(new BookingCreatedEvent
            {
                BookingId = booking.Id,
                UserId = request.UserId,
                UnitId = request.UnitId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                TotalPrice = totalPrice,
                GuestsCount = request.GuestsCount,
                Status = booking.Status.ToString(),
                BookedAt = booking.CreatedAt
            }, cancellationToken);

            // إرسال إشعار للضيف بتأكيد الحجز المبدئي
            await _notificationService.SendAsync(new NotificationRequest
            {
                UserId = request.UserId,
                Type = NotificationType.BookingCreated,
                Title = "تم إنشاء حجزك / Your booking has been created",
                Message = $"تم إنشاء حجزك رقم {booking.Id} بنجاح / Your booking {booking.Id} has been created successfully",
                Data = new { BookingId = booking.Id }
            }, cancellationToken);

            _logger.LogInformation("تم إنشاء الحجز بنجاح بالمعرف {BookingId}", booking.Id);

            return ResultDto<Guid>.Succeeded(booking.Id, "تم إنشاء الحجز بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء الحجز للمستخدم {UserId}", request.UserId);
            return ResultDto<Guid>.Failed("حدث خطأ أثناء إنشاء الحجز");
        }
    }

    /// <summary>
    /// التحقق من صحة المدخلات
    /// Input validation
    /// </summary>
    private async Task<ResultDto<Guid>> ValidateInputAsync(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من صحة المعرفات
        if (request.UserId == Guid.Empty)
            errors.Add("معرف المستخدم مطلوب");

        if (request.UnitId == Guid.Empty)
            errors.Add("معرف الوحدة مطلوب");

        // التحقق من صحة التواريخ
        if (request.CheckIn >= request.CheckOut)
            errors.Add("تاريخ المغادرة يجب أن يكون بعد تاريخ الوصول");

        if (request.CheckIn.Date < DateTime.Today)
            errors.Add("تاريخ الوصول يجب أن يكون في المستقبل");

        // التحقق من عدد الضيوف
        if (request.GuestsCount <= 0)
            errors.Add("عدد الضيوف يجب أن يكون أكبر من صفر");

        // التحقق من صحة الطلب
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors.Select(e => e.Message));
        }

        if (errors.Any())
        {
            return ResultDto<Guid>.Failed(errors, "بيانات غير صحيحة");
        }

        return ResultDto<Guid>.Succeeded(Guid.Empty);
    }

    /// <summary>
    /// التحقق من وجود الكيانات
    /// Existence validation
    /// </summary>
    private async Task<ResultDto<Guid>> ValidateExistenceAsync(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // التحقق من وجود المستخدم
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return ResultDto<Guid>.Failed("المستخدم غير موجود أو غير نشط");
        }

        // التحقق من وجود الوحدة
        var unit = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(request.UnitId, cancellationToken);
        if (unit == null || !unit.IsActive)
        {
            return ResultDto<Guid>.Failed("الوحدة غير موجودة أو غير نشطة");
        }

        return ResultDto<Guid>.Succeeded(Guid.Empty);
    }

    /// <summary>
    /// التحقق من الصلاحيات
    /// Authorization validation
    /// </summary>
    private async Task<ResultDto<Guid>> ValidateAuthorizationAsync(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        // العثور على الوحدة للتحقق من الصلاحيات
        var unit = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);

        // التحقق من الصلاحيات: حجز بنفسه أو مسؤول أو موظف
            if (currentUserId != request.UserId)
            {
                if (_currentUserService.Role != "Admin" && _currentUserService.Role != "Staff" && _currentUserService.UserId != request.UserId)
                {
                    return ResultDto<Guid>.Failed("ليس لديك الصلاحية لإنشاء حجز لمستخدم آخر");
                }
            }

        // التحقق من أن المستخدم هو موظف في الكيان إذا لم يكن المالك أو المسؤول
        if (_currentUserService.Role != "Admin" && _currentUserService.Role != "Staff" && unit != null && !_currentUserService.IsStaffInProperty(unit.PropertyId))
        {
            return ResultDto<Guid>.Failed("لست موظفًا في هذا الكيان");
        }

        return ResultDto<Guid>.Succeeded(Guid.Empty);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Business rules validation
    /// </summary>
    private async Task<ResultDto<Guid>> ValidateBusinessRulesAsync(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // التحقق من سعة الوحدة
        var unit = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(request.UnitId, cancellationToken);
        if (request.GuestsCount > unit.MaxCapacity)
        {
            return ResultDto<Guid>.Failed($"عدد الضيوف يتجاوز السعة القصوى للوحدة ({unit.MaxCapacity})");
        }

        // التحقق من عدم وجود حجز متداخل للمستخدم
        var overlappingBookings = await _unitOfWork.Repository<Booking>().FindAsync(
            b => b.UserId == request.UserId && b.CheckIn < request.CheckOut && b.CheckOut > request.CheckIn,
            cancellationToken);
        if (overlappingBookings.Any())
        {
            return ResultDto<Guid>.Failed("لديك حجز آخر في نفس الفترة الزمنية");
        }

        if (request.Services != null && request.Services.Any())
        {
            var unitEntity = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);
            if (unitEntity != null)
            {
                var propertyServices = await _propertyServiceRepository.GetPropertyServicesAsync(unitEntity.PropertyId, cancellationToken);
                var availableServiceIds = propertyServices.Select(ps => ps.Id);
                var invalidServiceIds = request.Services.Except(availableServiceIds).ToList();
                if (invalidServiceIds.Any())
                {
                    _logger.LogWarning("Invalid services requested: {InvalidServiceIds}", invalidServiceIds);
                    return ResultDto<Guid>.Failure($"Invalid services requested: {string.Join(", ", invalidServiceIds)}");
                }
            }
        }

        return ResultDto<Guid>.Succeeded(Guid.Empty);
    }
}

/// <summary>
/// حدث إنشاء الحجز
/// Booking created event
/// </summary>
public class BookingCreatedEvent : IBookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public Guid? UserId { get; set; }
    public Guid UnitId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public Money TotalPrice { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(BookingCreatedEvent);
    public int Version { get; set; } = 1;
    public string CorrelationId { get; set; }
    public int GuestsCount { get; set; }
    public string Status { get; set; }
    public DateTime BookedAt { get; set; }
}
