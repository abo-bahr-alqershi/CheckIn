using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لطلب إعادة تعيين كلمة المرور
/// Command to request password reset
/// </summary>
public class ForgotPasswordCommand : IRequest<ResultDto<bool>>
{
    public string Email { get; set; } = string.Empty;
}
