using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر تسجيل مستخدم جديد
    /// Command to register a new user
    /// </summary>
    public class RegisterUserCommand : IRequest<ResultDto<RegisterUserResponse>>
    {
        /// <summary>
        /// اسم المستخدم الكامل
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        public string Phone { get; set; } = string.Empty;
    }
}
