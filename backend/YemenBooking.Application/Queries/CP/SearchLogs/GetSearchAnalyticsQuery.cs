using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchLogs
{
    /// <summary>
    /// استعلام للحصول على تحليلات البحث
    /// Query to get search analytics
    /// </summary>
    public class GetSearchAnalyticsQuery : IRequest<ResultDto<SearchAnalyticsDto>>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
} 