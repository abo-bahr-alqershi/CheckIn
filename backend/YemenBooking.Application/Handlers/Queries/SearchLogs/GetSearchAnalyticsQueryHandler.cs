using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.SearchLogs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.SearchLogs
{
    /// <summary>
    /// معالج استعلام تحليلات البحث
    /// Query handler for search analytics
    /// </summary>
    public class GetSearchAnalyticsQueryHandler : IRequestHandler<GetSearchAnalyticsQuery, ResultDto<SearchAnalyticsDto>>
    {
        private readonly ISearchLogRepository _searchLogRepository;
        private readonly ILogger<GetSearchAnalyticsQueryHandler> _logger;

        public GetSearchAnalyticsQueryHandler(ISearchLogRepository searchLogRepository, ILogger<GetSearchAnalyticsQueryHandler> logger)
        {
            _searchLogRepository = searchLogRepository;
            _logger = logger;
        }

        public async Task<ResultDto<SearchAnalyticsDto>> Handle(GetSearchAnalyticsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري جلب تحليلات البحث من {From} إلى {To}", request.From, request.To);

            var query = _searchLogRepository.GetQueryable();

            if (request.From.HasValue)
                query = query.Where(x => x.CreatedAt >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(x => x.CreatedAt <= request.To.Value);

            var totalSearches = await query.CountAsync(cancellationToken);
            var propertySearches = await query.CountAsync(x => x.SearchType == "Property", cancellationToken);
            var unitSearches = await query.CountAsync(x => x.SearchType == "Unit", cancellationToken);

            var byDay = await query
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new SearchCountByDayDto { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync(cancellationToken);

            var analyticsDto = new SearchAnalyticsDto
            {
                TotalSearches = totalSearches,
                PropertySearches = propertySearches,
                UnitSearches = unitSearches,
                SearchesByDay = byDay
            };

            return ResultDto<SearchAnalyticsDto>.Succeeded(analyticsDto);
        }
    }
} 