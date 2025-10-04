using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتفعيل الحساب بعد التحقق من البريد الإلكتروني
/// Command to activate user after email verification
/// </summary>
public class ActivateUserCommand : IRequest<ResultDto<bool>>
{
    public Guid UserId { get; set; }
}
