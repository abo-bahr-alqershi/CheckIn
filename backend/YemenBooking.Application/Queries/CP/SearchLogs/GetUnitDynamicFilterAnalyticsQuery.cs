using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchLogs
{
    /// <summary>
    /// استعلام للحصول على احصائيات فلاتر البحث الديناميكية للوحدات
    /// Query to get dynamic filter analytics for unit searches
    /// </summary>
    public class GetUnitDynamicFilterAnalyticsQuery : IRequest<ResultDto<List<FieldFilterAnalyticsDto>>>
    {
        /// <summary>
        /// تاريخ بدء الفترة (اختياري)
        /// Start date for analytics (optional)
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// تاريخ نهاية الفترة (اختياري)
        /// End date for analytics (optional)
        /// </summary>
        public DateTime? To { get; set; }
    }
} 