using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتجديد رمز الوصول باستخدام التوكن المنعش
/// Command to refresh access token
/// </summary>
public class RefreshTokenCommand : IRequest<ResultDto<AuthResultDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
