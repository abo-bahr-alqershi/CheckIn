using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Bookings;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Bookings
{
    /// <summary>
    /// معالج استعلام الحصول على الحجوزات ضمن نطاق زمني
    /// Query handler for GetBookingsByDateRangeQuery
    /// </summary>
    public class GetBookingsByDateRangeQueryHandler : IRequestHandler<GetBookingsByDateRangeQuery, PaginatedResult<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookingsByDateRangeQueryHandler> _logger;

        public GetBookingsByDateRangeQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetBookingsByDateRangeQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<BookingDto>> Handle(GetBookingsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الحجوزات من {StartDate} إلى {EndDate}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.StartDate, request.EndDate, request.PageNumber, request.PageSize);

            if (request.EndDate < request.StartDate)
                return PaginatedResult<BookingDto>.Empty(request.PageNumber, request.PageSize);

            var roles = _currentUserService.UserRoles;
            var propertyId = _currentUserService.PropertyId;

            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _bookingRepository.GetQueryable()
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Unit)
                .Where(b => b.BookedAt >= request.StartDate && b.BookedAt <= request.EndDate);

            if (!roles.Contains("Admin") && propertyId.HasValue)
            {
                // للملاك أو الموظفين، عرض الحجوزات المتعلقة بالكيان فقط
                query = query.Where(b => b.Unit.PropertyId == propertyId.Value);
            }

            if (request.UserId.HasValue)
                query = query.Where(b => b.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.GuestNameOrEmail))
            {
                var term = request.GuestNameOrEmail.Trim().ToLower();
                query = query.Where(b => b.User.Name.ToLower().Contains(term)
                                      || b.User.Email.ToLower().Contains(term));
            }

            if (request.UnitId.HasValue)
                query = query.Where(b => b.UnitId == request.UnitId.Value);

            if (!string.IsNullOrWhiteSpace(request.BookingSource))
                query = query.Where(b => b.BookingSource == request.BookingSource);

            if (request.IsWalkIn.HasValue)
                query = query.Where(b => b.IsWalkIn == request.IsWalkIn.Value);

            if (request.MinTotalPrice.HasValue)
                query = query.Where(b => b.TotalPrice.Amount >= request.MinTotalPrice.Value);

            if (request.MinGuestsCount.HasValue)
                query = query.Where(b => b.GuestsCount >= request.MinGuestsCount.Value);

            query = request.SortBy?.Trim().ToLower() switch
            {
                "check_in_date" => query.OrderBy(b => b.CheckIn),
                "booking_date" => query.OrderBy(b => b.BookedAt),
                "total_price" => query.OrderBy(b => b.TotalPrice.Amount),
                _ => query.OrderByDescending(b => b.BookedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var bookings = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = bookings.Select(b => _mapper.Map<BookingDto>(b)).ToList();

            _logger.LogInformation("تم جلب {Count} حجز من إجمالي {TotalCount} ضمن النطاق الزمني", dtos.Count, totalCount);
            return new PaginatedResult<BookingDto>(dtos, pageNumber, pageSize, totalCount);
        }
    }
} 