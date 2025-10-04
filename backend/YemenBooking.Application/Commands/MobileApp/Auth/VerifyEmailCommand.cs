using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;

namespace YemenBooking.Application.Commands.MobileApp.Auth;

/// <summary>
/// أمر تأكيد البريد الإلكتروني
/// Command to verify email
/// </summary>
public class VerifyEmailCommand : IRequest<ResultDto<VerifyEmailResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// رمز التأكيد
    /// </summary>
    public string VerificationToken { get; set; } = string.Empty;
}