using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Staffs
{
    /// <summary>
    /// استعلام للحصول على بيانات الموظف للمستخدم
    /// Query to get staff details by user
    /// </summary>
    public class GetStaffByUserQuery : IRequest<ResultDto<StaffDto>>
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public Guid UserId { get; set; }
    }
} 