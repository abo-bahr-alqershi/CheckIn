using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Roles;

/// <summary>
/// استعلام للحصول على جميع الأدوار
/// </summary>
public class GetAllRolesQuery : IRequest<PaginatedResult<RoleDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
