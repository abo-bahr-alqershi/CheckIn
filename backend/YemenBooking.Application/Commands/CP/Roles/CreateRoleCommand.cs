using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Roles;

/// <summary>
/// أمر لإنشاء دور جديد
/// </summary>
public class CreateRoleCommand : IRequest<ResultDto<Guid>>
{
    public string Name { get; set; } = string.Empty;
}
