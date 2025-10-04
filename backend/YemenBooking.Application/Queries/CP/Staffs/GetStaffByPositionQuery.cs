using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Staffs
{
    /// <summary>
    /// استعلام للحصول على الموظفين حسب المنصب
    /// Query to get staff by position
    /// </summary>
    public class GetStaffByPositionQuery : IRequest<PaginatedResult<StaffDto>>
    {
        /// <summary>
        /// المنصب
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// معرف الكيان (اختياري)
        /// </summary>
        public Guid? PropertyId { get; set; }

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