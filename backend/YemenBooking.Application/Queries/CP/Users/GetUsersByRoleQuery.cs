using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Users;

/// <summary>
/// استعلام للحصول على المستخدمين حسب الدور
/// Query to get users by role name
/// </summary>
public class GetUsersByRoleQuery : IRequest<PaginatedResult<UserDto>>
{
    /// <summary>
    /// اسم الدور
    /// Role name
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 