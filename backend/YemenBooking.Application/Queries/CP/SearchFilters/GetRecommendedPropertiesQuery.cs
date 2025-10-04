using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchFilters
{
    /// <summary>
    /// استعلام للحصول على الكيانات المقترحة للمستخدم
    /// Query to get recommended properties for a user
    /// </summary>
    public class GetRecommendedPropertiesQuery : IRequest<PaginatedResult<PropertyDto>>
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// تفضيلات البحث (اختياري)
        /// </summary>
        public string? Preferences { get; set; }

        /// <summary>
        /// عدد النتائج
        /// </summary>
        public int Count { get; set; }
    }
} 