using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchFilters
{
    /// <summary>
    /// استعلام للحصول على الوجهات الشائعة
    /// Query to get popular destinations
    /// </summary>
    public class GetPopularDestinationsQuery : IRequest<PaginatedResult<DestinationDto>>
    {
        /// <summary>
        /// عدد النتائج
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// نوع الكيان (اختياري)
        /// </summary>
        public string? PropertyType { get; set; }
    }
} 