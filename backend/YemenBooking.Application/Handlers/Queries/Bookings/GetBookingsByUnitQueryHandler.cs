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
    /// معالج استعلام الحصول على حجوزات وحدة معينة
    /// Query handler for GetBookingsByUnitQuery
    /// </summary>
    public class GetBookingsByUnitQueryHandler : IRequestHandler<GetBookingsByUnitQuery, PaginatedResult<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookingsByUnitQueryHandler> _logger;

        public GetBookingsByUnitQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetBookingsByUnitQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<BookingDto>> Handle(GetBookingsByUnitQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام حجوزات الوحدة: {UnitId}, الصفحة: {PageNumber}, الحجم: {PageSize}", request.UnitId, request.PageNumber, request.PageSize);

            if (request.UnitId == Guid.Empty)
                return PaginatedResult<BookingDto>.Empty(request.PageNumber, request.PageSize);

            var roles = _currentUserService.UserRoles;
            var propertyId = _currentUserService.PropertyId;
            if (!roles.Contains("Admin") && propertyId.HasValue)
            {
                // التحقق من صلاحية عرض الحجوزات لهذه الوحدة
                // نفترض أن الCurrentUserService.PropertyId يحدد الكيان المملوك
            }

            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _bookingRepository.GetQueryable()
                .AsNoTracking()
                .Include(b => b.User)
                .Where(b => b.UnitId == request.UnitId);

            if (request.StartDate.HasValue)
                query = query.Where(b => b.CheckIn >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(b => b.CheckOut <= request.EndDate.Value);

            query = query.OrderByDescending(b => b.CheckIn);

            var totalCount = await query.CountAsync(cancellationToken);
            var bookings = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = bookings.Select(b => _mapper.Map<BookingDto>(b)).ToList();

            _logger.LogInformation("تم جلب {Count} حجز من إجمالي {TotalCount} لوحدة: {UnitId}", dtos.Count, totalCount, request.UnitId);
            return new PaginatedResult<BookingDto>(dtos, pageNumber, pageSize, totalCount);
        }
    }
} 