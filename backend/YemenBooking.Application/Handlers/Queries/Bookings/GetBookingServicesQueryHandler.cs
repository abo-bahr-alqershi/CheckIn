using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Bookings;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Bookings
{
    /// <summary>
    /// معالج استعلام الحصول على خدمات الحجز
    /// Query handler for GetBookingServicesQuery
    /// </summary>
    public class GetBookingServicesQueryHandler : IRequestHandler<GetBookingServicesQuery, ResultDto<IEnumerable<ServiceDto>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookingServicesQueryHandler> _logger;

        public GetBookingServicesQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetBookingServicesQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<ServiceDto>>> Handle(GetBookingServicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام خدمات الحجز: {BookingId}", request.BookingId);

            if (request.BookingId == Guid.Empty)
                return ResultDto<IEnumerable<ServiceDto>>.Failure("معرف الحجز غير صالح");

            var booking = await _bookingRepository.GetBookingWithServicesAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return ResultDto<IEnumerable<ServiceDto>>.Failure($"الحجز بالمعرف {request.BookingId} غير موجود");

            var roles = _currentUserService.UserRoles;
            var currentUserId = _currentUserService.UserId;
            if (booking.UserId != currentUserId && !roles.Contains("Admin"))
                return ResultDto<IEnumerable<ServiceDto>>.Failure("ليس لديك صلاحية لعرض خدمات هذا الحجز");

            var services = booking.BookingServices.Select(bs => bs.Service).ToList();
            var dtos = services.Select(s => _mapper.Map<ServiceDto>(s)).ToList();

            _logger.LogInformation("تم جلب {Count} خدمة للحجز: {BookingId}", dtos.Count, request.BookingId);
            return ResultDto<IEnumerable<ServiceDto>>.Ok(dtos, "تم جلب خدمات الحجز بنجاح");
        }
    }
} 