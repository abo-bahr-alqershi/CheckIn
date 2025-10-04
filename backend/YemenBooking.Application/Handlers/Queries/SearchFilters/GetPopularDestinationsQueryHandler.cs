using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Application.Interfaces.Services;
using System.Collections.Generic;
using System;

namespace YemenBooking.Application.Handlers.Queries.SearchFilters
{
    /// <summary>
    /// معالج استعلام الحصول على الوجهات الشائعة
    /// Query handler for GetPopularDestinationsQuery
    /// </summary>
    public class GetPopularDestinationsQueryHandler : IRequestHandler<GetPopularDestinationsQuery, PaginatedResult<DestinationDto>>
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<GetPopularDestinationsQueryHandler> _logger;

        public GetPopularDestinationsQueryHandler(ISearchService searchService, ILogger<GetPopularDestinationsQueryHandler> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        public async Task<PaginatedResult<DestinationDto>> Handle(GetPopularDestinationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الوجهات الشائعة بحد أقصى: {Count}", request.Count);

            var results = await _searchService.GetPopularDestinationsAsync(request.Count, cancellationToken);
            var dtos = new List<DestinationDto>();

            foreach (var item in results)
            {
                var cityProp = item.GetType().GetProperty("City")?.GetValue(item)?.ToString();
                var viewCountProp = item.GetType().GetProperty("ViewCount")?.GetValue(item);
                int viewCount = viewCountProp is int i ? i : 0;
                dtos.Add(new DestinationDto { City = cityProp ?? string.Empty, ViewCount = viewCount });
            }

            // filter by PropertyType if specified
            if (!string.IsNullOrWhiteSpace(request.PropertyType))
            {
                dtos = dtos.Where(x => x.City.Equals(request.PropertyType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return new PaginatedResult<DestinationDto>
            {
                Items = dtos,
                PageNumber = 1,
                PageSize = request.Count,
                TotalCount = dtos.Count
            };
        }
    }
} 