using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Bookings;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System.Linq;

namespace YemenBooking.Application.Handlers.Queries.Bookings
{
    /// <summary>
    /// معالج استعلام الحصول على تفاصيل حجز معين
    /// Query handler for GetBookingByIdQuery
    /// </summary>
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, ResultDto<BookingDetailsDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookingByIdQueryHandler> _logger;
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;

        public GetBookingByIdQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetBookingByIdQueryHandler> logger,
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<ResultDto<BookingDetailsDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("جاري معالجة استعلام تفاصيل الحجز: {BookingId}", request.BookingId);

                // التحقق من صحة المعرف
                if (request.BookingId == Guid.Empty)
                {
                    return ResultDto<BookingDetailsDto>.Failure("معرف الحجز غير صالح");
                }

                // جلب الحجز مع المدفوعات
                var booking = await _bookingRepository.GetBookingWithPaymentsAsync(request.BookingId, cancellationToken);
                if (booking == null)
                {
                    return ResultDto<BookingDetailsDto>.Failure($"الحجز بالمعرف {request.BookingId} غير موجود");
                }

                // جلب الخدمات المرتبطة
                var bookingWithServices = await _bookingRepository.GetBookingWithServicesAsync(request.BookingId, cancellationToken);
                if (bookingWithServices != null)
                {
                    booking.BookingServices = bookingWithServices.BookingServices;
                }

                // التحقق من الصلاحيات: المالك أو المشرف
                var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (user == null)
                {
                    return ResultDto<BookingDetailsDto>.Failure("يجب تسجيل الدخول لعرض تفاصيل الحجز");
                }
                var roles = _currentUserService.UserRoles;
                if (booking.UserId != _currentUserService.UserId && !roles.Contains("Admin"))
                {
                    return ResultDto<BookingDetailsDto>.Failure("ليس لديك صلاحية لعرض هذا الحجز");
                }

                // التحويل إلى DTO
                var detailsDto = _mapper.Map<BookingDetailsDto>(booking);

                // enrich with unit and property information for admin details page
                var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
                if (unit != null)
                {
                    detailsDto.UnitId = unit.Id;
                    detailsDto.UnitName = unit.Name ?? string.Empty;
                    detailsDto.UnitImages = unit.Images?.Select(i => i.Url).ToList() ?? new System.Collections.Generic.List<string>();

                    var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
                    if (property != null)
                    {
                        detailsDto.PropertyId = property.Id;
                        detailsDto.PropertyName = property.Name ?? string.Empty;
                        detailsDto.PropertyAddress = property.Address ?? string.Empty;
                    }
                }

                _logger.LogInformation("تم جلب تفاصيل الحجز بنجاح: {BookingId}", request.BookingId);
                return ResultDto<BookingDetailsDto>.Ok(detailsDto, "تم جلب تفاصيل الحجز بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة استعلام تفاصيل الحجز: {BookingId}", request.BookingId);
                return ResultDto<BookingDetailsDto>.Failure("حدث خطأ أثناء جلب تفاصيل الحجز");
            }
        }
    }
} 