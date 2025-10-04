using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام اتجاهات الحجوزات
    /// Handler for GetBookingTrendsQuery
    /// </summary>
    public class GetBookingTrendsQueryHandler : IRequestHandler<GetBookingTrendsQuery, IEnumerable<TimeSeriesDataDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetBookingTrendsQueryHandler> _logger;

        public GetBookingTrendsQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            ILogger<GetBookingTrendsQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<TimeSeriesDataDto>> Handle(GetBookingTrendsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Booking Trends Query for Property {PropertyId} Range {Start} - {End}",
                request.PropertyId, request.Range.StartDate, request.Range.EndDate);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new UnauthorizedException("يجب تسجيل الدخول للوصول إلى اتجاهات الحجوزات");
            var role = _currentUserService.Role;
            if (request.PropertyId.HasValue)
            {
                if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
                    throw new ForbiddenException("ليس لديك صلاحية لعرض اتجاهات الحجوزات لهذا الكيان");
            }
            else if (role != "Admin")
            {
                throw new ForbiddenException("ليس لديك صلاحية لعرض اتجاهات الحجوزات العامة");
            }

            IEnumerable<Booking> bookings;
            if (request.PropertyId.HasValue)
            {
                bookings = await _bookingRepository.GetBookingsByPropertyAsync(
                    request.PropertyId.Value,
                    request.Range.StartDate,
                    request.Range.EndDate,
                    cancellationToken);
            }
            else
            {
                bookings = await _bookingRepository.GetBookingsByDateRangeAsync(
                    request.Range.StartDate,
                    request.Range.EndDate,
                    cancellationToken);
            }

            var trends = bookings
                .GroupBy(b => b.BookedAt.Date)
                .Select(g => new TimeSeriesDataDto
                {
                    Date = g.Key,
                    Value = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToList();

            return trends;
        }
    }
} 