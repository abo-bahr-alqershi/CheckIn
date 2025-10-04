using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لإعادة تعيين كلمة المرور باستخدام رمز
/// Command to reset password using token
/// </summary>
public class ResetPasswordCommand : IRequest<ResultDto<bool>>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
