using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Staffs
{
    /// <summary>
    /// أمر لإزالة الموظف
    /// Command to remove a staff member
    /// </summary>
    public class RemoveStaffCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الموظف
        /// </summary>
        public Guid StaffId { get; set; }
    }
} 