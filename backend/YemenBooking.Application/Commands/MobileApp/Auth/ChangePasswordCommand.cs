using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر تغيير كلمة المرور
    /// Command to change user password
    /// </summary>
    public class ChangePasswordCommand : IRequest<ResultDto<ChangePasswordResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// كلمة المرور الحالية
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;
    
        /// <summary>
        /// كلمة المرور الجديدة
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}