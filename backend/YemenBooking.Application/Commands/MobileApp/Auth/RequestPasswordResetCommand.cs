using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر طلب إعادة تعيين كلمة المرور
    /// Command to request password reset
    /// </summary>
    public class RequestPasswordResetCommand : IRequest<ResultDto<RequestPasswordResetResponse>>
    {
        /// <summary>
        /// البريد الإلكتروني أو رقم الهاتف
        /// </summary>
        [Required(ErrorMessage = "البريد الإلكتروني أو رقم الهاتف مطلوب")]
        public string EmailOrPhone { get; set; } = string.Empty;
    }
}