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

            var result = new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
            if (pageNumber == 1)
            {
                var successful = await queryable.CountAsync(p => p.Status == PaymentStatus.Successful, cancellationToken);
                var pending = await queryable.CountAsync(p => p.Status == PaymentStatus.Pending, cancellationToken);
                var failed = await queryable.CountAsync(p => p.Status == PaymentStatus.Failed, cancellationToken);
                var refunded = await queryable.CountAsync(p => p.Status == PaymentStatus.Refunded || p.Status == PaymentStatus.PartiallyRefunded, cancellationToken);
                var totalAmount = await queryable.SumAsync(p => p.Amount.Amount, cancellationToken);

                // Compute trend percentages only when a valid date range is provided
                double? totalPaymentsTrendPct = null;
                double? totalAmountTrendPct = null;
                double? successfulPaymentsTrendPct = null;
                double? refundedPaymentsTrendPct = null;

                if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate.Value > request.StartDate.Value)
                {
                    var currentStart = request.StartDate!.Value;
                    var currentEnd = request.EndDate!.Value;
                    var period = currentEnd - currentStart;
                    var previousStart = currentStart - period;
                    var previousEnd = currentStart;

                    var currentPeriod = _paymentRepository.GetQueryable()
                        .AsNoTracking()
                        .Where(p => p.PaymentDate >= currentStart && p.PaymentDate <= currentEnd);

                    // Apply other filters identically to previous/current periods
                    if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PaymentStatus>(request.Status, true, out var s))
                        currentPeriod = currentPeriod.Where(p => p.Status == s);
                    if (!string.IsNullOrWhiteSpace(request.Method))
                        currentPeriod = currentPeriod.Where(p => p.Method.Name == request.Method);
                    if (request.BookingId.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.BookingId == request.BookingId.Value);
                    if (request.UserId.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.Booking.UserId == request.UserId.Value);
                    if (request.PropertyId.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.Booking.Unit.PropertyId == request.PropertyId.Value);
                    if (request.UnitId.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.Booking.UnitId == request.UnitId.Value);
                    if (request.MinAmount.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.Amount.Amount >= request.MinAmount.Value);
                    if (request.MaxAmount.HasValue)
                        currentPeriod = currentPeriod.Where(p => p.Amount.Amount <= request.MaxAmount.Value);

                    var previousPeriod = _paymentRepository.GetQueryable()
                        .AsNoTracking()
                        .Where(p => p.PaymentDate >= previousStart && p.PaymentDate <= previousEnd);

                    if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PaymentStatus>(request.Status, true, out var ps))
                        previousPeriod = previousPeriod.Where(p => p.Status == ps);
                    if (!string.IsNullOrWhiteSpace(request.Method))
                        previousPeriod = previousPeriod.Where(p => p.Method.Name == request.Method);
                    if (request.BookingId.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.BookingId == request.BookingId.Value);
                    if (request.UserId.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.Booking.UserId == request.UserId.Value);
                    if (request.PropertyId.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.Booking.Unit.PropertyId == request.PropertyId.Value);
                    if (request.UnitId.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.Booking.UnitId == request.UnitId.Value);
                    if (request.MinAmount.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.Amount.Amount >= request.MinAmount.Value);
                    if (request.MaxAmount.HasValue)
                        previousPeriod = previousPeriod.Where(p => p.Amount.Amount <= request.MaxAmount.Value);

                    var currTotal = await currentPeriod.CountAsync(cancellationToken);
                    var prevTotal = await previousPeriod.CountAsync(cancellationToken);
                    var currAmount = await currentPeriod.SumAsync(p => p.Amount.Amount, cancellationToken);
                    var prevAmount = await previousPeriod.SumAsync(p => p.Amount.Amount, cancellationToken);
                    var currSuccessful = await currentPeriod.CountAsync(p => p.Status == PaymentStatus.Successful, cancellationToken);
                    var prevSuccessful = await previousPeriod.CountAsync(p => p.Status == PaymentStatus.Successful, cancellationToken);
                    var currRefunded = await currentPeriod.CountAsync(p => p.Status == PaymentStatus.Refunded || p.Status == PaymentStatus.PartiallyRefunded, cancellationToken);
                    var prevRefunded = await previousPeriod.CountAsync(p => p.Status == PaymentStatus.Refunded || p.Status == PaymentStatus.PartiallyRefunded, cancellationToken);

                    static double? Trend(double current, double previous)
                    {
                        if (previous == 0)
                        {
                            return current == 0 ? 0.0 : (double?)null; // No baseline -> no trend
                        }
                        var pct = ((current - previous) / previous) * 100.0;
                        return Math.Round(pct, 1);
                    }

                    totalPaymentsTrendPct = Trend(currTotal, prevTotal);
                    totalAmountTrendPct = Trend((double)currAmount, (double)prevAmount);
                    successfulPaymentsTrendPct = Trend(currSuccessful, prevSuccessful);
                    refundedPaymentsTrendPct = Trend(currRefunded, prevRefunded);
                }

                result.Metadata = new
                {
                    totalPayments = totalCount,
                    totalAmount,
                    successfulPayments = successful,
                    pendingPayments = pending,
                    failedPayments = failed,
                    refundedPayments = refunded,
                    totalPaymentsTrendPct,
                    totalAmountTrendPct,
                    successfulPaymentsTrendPct,
                    refundedPaymentsTrendPct
                };
            }
            return result;
        }
    }
} 