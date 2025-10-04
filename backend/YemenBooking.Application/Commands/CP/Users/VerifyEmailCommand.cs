using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتأكيد البريد الإلكتروني باستخدام رمز
/// Command to verify email using token
/// </summary>
public class VerifyEmailCommand : IRequest<ResultDto<bool>>
{
    public string Token { get; set; } = string.Empty;
}
