using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Booking;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Bookings;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Booking;

/// <summary>
/// معالج استعلام الحصول على تفاصيل الحجز
/// Handler for get booking details query
/// </summary>
public class GetBookingDetailsQueryHandler : IRequestHandler<GetBookingDetailsQuery, ResultDto<BookingDetailsDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingServiceRepository _bookingServiceRepository;
    private readonly ILogger<GetBookingDetailsQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام تفاصيل الحجز
    /// Constructor for get booking details query handler
    /// </summary>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="unitRepository">مستودع الوحدات</param>
    /// <param name="paymentRepository">مستودع المدفوعات</param>
    /// <param name="bookingServiceRepository">مستودع خدمات الحجز</param>
    /// <param name="logger">مسجل الأحداث</param>
    public GetBookingDetailsQueryHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IPaymentRepository paymentRepository,
        IBookingServiceRepository bookingServiceRepository,
        ILogger<GetBookingDetailsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
        _paymentRepository = paymentRepository;
        _bookingServiceRepository = bookingServiceRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على تفاصيل الحجز
    /// Handle get booking details query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>تفاصيل الحجز</returns>
    public async Task<ResultDto<BookingDetailsDto>> Handle(GetBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام تفاصيل الحجز. معرف الحجز: {BookingId}, معرف المستخدم: {UserId}", 
                request.BookingId, request.UserId);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // الحصول على الحجز
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("لم يتم العثور على الحجز: {BookingId}", request.BookingId);
                return ResultDto<BookingDetailsDto>.Failed("الحجز غير موجود", "BOOKING_NOT_FOUND");
            }

            // التحقق من صلاحية المستخدم للوصول للحجز
            if (booking.UserId != request.UserId)
            {
                _logger.LogWarning("المستخدم {UserId} لا يملك صلاحية الوصول للحجز {BookingId}", 
                    request.UserId, request.BookingId);
                return ResultDto<BookingDetailsDto>.Failed("ليس لديك صلاحية للوصول لهذا الحجز", "ACCESS_DENIED");
            }

            // جلب بيانات الوحدة والخدمات والمدفوعات بشكل تسلسلي لتجنب مشاركة DbContext بالتوازي
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit == null)
            {
                _logger.LogWarning("لم يتم العثور على الوحدة: {UnitId}", booking.UnitId);
                return ResultDto<BookingDetailsDto>.Failed("بيانات الوحدة غير متاحة", "UNIT_NOT_FOUND");
            }

            var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
            if (property == null)
            {
                _logger.LogWarning("لم يتم العثور على العقار: {PropertyId}", unit.PropertyId);
                return ResultDto<BookingDetailsDto>.Failed("بيانات العقار غير متاحة", "PROPERTY_NOT_FOUND");
            }

            var bookingServices = await _bookingServiceRepository.GetBookingServicesAsync(booking.Id, cancellationToken);
            var serviceDtos = bookingServices.Select(bs => new BookingServiceDto
            {
                Id = bs.ServiceId,
                Name = bs.Service?.Name ?? string.Empty,
                Quantity = bs.Quantity,
                TotalPrice = bs.TotalPrice.Amount,
                Currency = bs.TotalPrice.Currency ?? "YER"
            }).ToList();

            var payments = await _paymentRepository.GetPaymentsByBookingAsync(booking.Id, cancellationToken);
            var paymentDtos = payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Currency = p.Amount.Currency ?? "YER",
                Method = (PaymentMethodEnum)p.Method.Type,
                Status = p.Status,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId ?? string.Empty
            }).ToList();

            // إنشاء DTO للاستجابة
            var bookingDetailsDto = new BookingDetailsDto
            {
                Id = booking.Id,
                BookingNumber = booking.Id.ToString().Substring(0, 8),
                UserId = booking.UserId,
                UnitId = booking.UnitId,
                UnitName = unit.Name ?? string.Empty,
                PropertyId = property.Id,
                PropertyName = property.Name ?? string.Empty,
                PropertyAddress = property.Address ?? string.Empty,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestsCount = booking.GuestsCount,
                Currency = booking.TotalPrice.Currency ?? "YER",
                Status = booking.Status,
                BookedAt = booking.BookedAt,
                BookingSource = booking.BookingSource,
                CancellationReason = booking.CancellationReason,
                IsWalkIn = booking.IsWalkIn,
                PlatformCommissionAmount = booking.PlatformCommissionAmount,
                ActualCheckInDate = booking.ActualCheckInDate,
                ActualCheckOutDate = booking.ActualCheckOutDate,
                TotalAmount = booking.TotalPrice.Amount,
                CustomerRating = (int?)booking.CustomerRating,
                CompletionNotes = booking.CompletionNotes,
                Services = serviceDtos,
                Payments = paymentDtos,
                UnitImages = unit.Images?.Select(img => img.Url).ToList() ?? new List<string>()
            };

            _logger.LogInformation("تم الحصول على تفاصيل الحجز بنجاح. معرف الحجز: {BookingId}", request.BookingId);

            return ResultDto<BookingDetailsDto>.Ok(
                bookingDetailsDto, 
                "تم الحصول على تفاصيل الحجز بنجاح"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على تفاصيل الحجز. معرف الحجز: {BookingId}", request.BookingId);
            return ResultDto<BookingDetailsDto>.Failed(
                $"حدث خطأ أثناء الحصول على تفاصيل الحجز: {ex.Message}", 
                "GET_BOOKING_DETAILS_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<BookingDetailsDto> ValidateRequest(GetBookingDetailsQuery request)
    {
        if (request.BookingId == Guid.Empty)
        {
            _logger.LogWarning("معرف الحجز مطلوب");
            return ResultDto<BookingDetailsDto>.Failed("معرف الحجز مطلوب", "BOOKING_ID_REQUIRED");
        }

        if (request.UserId == Guid.Empty)
        {
            _logger.LogWarning("معرف المستخدم مطلوب");
            return ResultDto<BookingDetailsDto>.Failed("معرف المستخدم مطلوب", "USER_ID_REQUIRED");
        }

        return ResultDto<BookingDetailsDto>.Ok(null, "البيانات صحيحة");
    }
}
