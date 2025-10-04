namespace YemenBooking.Application.Queries.Chat
{
    using System.Collections.Generic;
    using MediatR;
    using YemenBooking.Application.DTOs;
    using YemenBooking.Application.DTOs.Users;

    /// <summary>
    /// استعلام لجلب حسابات الإدارة (ADMIN و SUPER_ADMIN)
    /// Query to get admin accounts (ADMIN and SUPER_ADMIN)
    /// </summary>
    public class GetAdminUsersQuery : IRequest<ResultDto<IEnumerable<ChatUserDto>>>
    {
    }
}
