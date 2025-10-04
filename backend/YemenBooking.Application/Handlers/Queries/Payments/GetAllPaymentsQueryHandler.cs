using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Payments;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities; // added to reference Payment entity

namespace YemenBooking.Application.Handlers.Queries.Payments
{
    /// <summary>
    /// معالج استعلام جلب جميع المدفوعات مع دعم الفلاتر
    /// </summary>
    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, PaginatedResult<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllPaymentsQueryHandler> _logger;

        public GetAllPaymentsQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetAllPaymentsQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetAllPaymentsQuery with filters: {@Request}", request);

            // Authorization: Admin only
            if (!await _currentUserService.IsInRoleAsync("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض المدفوعات");

            // Build query
            IQueryable<Payment> queryable = _paymentRepository.GetQueryable()
                .AsNoTracking()
                .Include(p => p.Booking).ThenInclude(b => b.Unit);

            if (!string.IsNullOrWhiteSpace(request.Status)
                && Enum.TryParse<PaymentStatus>(request.Status, true, out var statusEnum))
            {
                queryable = queryable.Where(p => p.Status == statusEnum);
            }
            if (!string.IsNullOrWhiteSpace(request.Method))
            {
                queryable = queryable.Where(p => p.Method.Name == request.Method);
            }
            if (request.BookingId.HasValue)
                queryable = queryable.Where(p => p.BookingId == request.BookingId.Value);
            if (request.UserId.HasValue)
                queryable = queryable.Where(p => p.Booking.UserId == request.UserId.Value);
            if (request.PropertyId.HasValue)
                queryable = queryable.Where(p => p.Booking.Unit.PropertyId == request.PropertyId.Value);
            if (request.UnitId.HasValue)
                queryable = queryable.Where(p => p.Booking.UnitId == request.UnitId.Value);
            if (request.MinAmount.HasValue)
                queryable = queryable.Where(p => p.Amount.Amount >= request.MinAmount.Value);
            if (request.MaxAmount.HasValue)
                queryable = queryable.Where(p => p.Amount.Amount <= request.MaxAmount.Value);
            if (request.StartDate.HasValue)
                queryable = queryable.Where(p => p.PaymentDate >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                queryable = queryable.Where(p => p.PaymentDate <= request.EndDate.Value);

            // Sort by payment date desc
            queryable = queryable.OrderByDescending(p => p.PaymentDate);

            // Pagination
            var totalCount = await queryable.CountAsync(cancellationToken);
            var pageNumber = Math.Max(request.PageNumber, 1);
            var pageSize = Math.Max(request.PageSize, 1);
            var items = await queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<PaymentDto>(p))
                .ToListAsync(cancellationToken);

            return new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 