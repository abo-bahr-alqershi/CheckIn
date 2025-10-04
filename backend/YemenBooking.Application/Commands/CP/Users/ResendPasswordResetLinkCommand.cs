using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users
{
    /// <summary>
    /// أمر لإعادة إرسال رابط إعادة تعيين كلمة المرور
    /// Command to resend password reset link for user
    /// </summary>
    public class ResendPasswordResetLinkCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// البريد الإلكتروني للمستخدم
        /// User's email
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
} 