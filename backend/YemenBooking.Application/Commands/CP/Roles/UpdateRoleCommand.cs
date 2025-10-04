using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Roles;

/// <summary>
/// أمر لتحديث دور
/// </summary>
public class UpdateRoleCommand : IRequest<ResultDto<bool>>
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
}
