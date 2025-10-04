using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs.PropertySearch
{
    /// <summary>
    /// إحصائيات البحث
    /// </summary>
    public class SearchStatisticsDto
    {
        /// <summary>
        /// مدة البحث بالملي ثانية
        /// </summary>
        public long SearchDurationMs { get; set; }

        /// <summary>
        /// عدد الفلاتر المطبقة
        /// </summary>
        public int AppliedFiltersCount { get; set; }

        /// <summary>
        /// عدد النتائج قبل التصفح
        /// </summary>
        public int TotalResultsBeforePaging { get; set; }

        /// <summary>
        /// اقتراحات البحث
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// النطاق السعري للنتائج
        /// </summary>
        public PriceRangeDto? PriceRange { get; set; }
    }
} 