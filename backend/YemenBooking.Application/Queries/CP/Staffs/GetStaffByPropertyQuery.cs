using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Staffs
{
    /// <summary>
    /// استعلام للحصول على موظفي الكيان
    /// Query to get staff by property
    /// </summary>
    public class GetStaffByPropertyQuery : IRequest<PaginatedResult<StaffDto>>
    {
        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

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