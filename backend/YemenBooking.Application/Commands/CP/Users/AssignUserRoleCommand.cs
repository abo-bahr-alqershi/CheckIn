using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتخصيص دور للمستخدم
/// Command to assign a role to a user
/// </summary>
public class AssignUserRoleCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// معرف الدور
    /// Role ID
    /// </summary>
    public Guid RoleId { get; set; }
} 