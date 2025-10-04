using MediatR;
using System;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Images
{
    /// <summary>
    /// استعلام لإحصائيات الصور (إجمالي، حجم، حسب فئة، حسب حالة)
    /// Query to get image statistics (total, size, by category, by status, average, primary)
    /// </summary>
    public class GetImageStatisticsQuery : IRequest<ResultDto<ImageStatisticsDto>>
    {
        /// <summary>
        /// معرف الكيان (اختياري)
        /// Property ID (optional)
        /// </summary>
        public Guid? PropertyId { get; set; }

        /// <summary>
        /// معرف الوحدة (اختياري)
        /// Unit ID (optional)
        /// </summary>
        public Guid? UnitId { get; set; }
    }
} 