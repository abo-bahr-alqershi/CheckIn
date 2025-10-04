using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Policies
{
    /// <summary>
    /// استعلام للحصول على السياسات حسب النوع
    /// Query to get policies by type
    /// </summary>
    public class GetPoliciesByTypeQuery : IRequest<PaginatedResult<PolicyDto>>
    {
        /// <summary>
        /// نوع السياسة
        /// </summary>
        public string PolicyType { get; set; }

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
} 