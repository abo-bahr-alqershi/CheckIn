using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Users
{
    /// <summary>استعلام جلب تفاصيل المستخدم</summary>
    public class GetUserDetailsQuery : IRequest<ResultDto<UserDetailsDto>>
    {
        /// <summary>معرف المستخدم</summary>
        public Guid UserId { get; set; }
    }
} 