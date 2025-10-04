using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Roles;

/// <summary>
/// أمر لحذف دور
/// </summary>
public class DeleteRoleCommand : IRequest<ResultDto<bool>>
{
    public Guid RoleId { get; set; }
}
