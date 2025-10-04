using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.DTOs.Common;

namespace YemenBooking.Application.Interfaces.Services
{
    /// <summary>
    /// واجهة خدمة المصادقة وإدارة الجلسة
    /// Interface for authentication and session management service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// تسجيل الدخول وإصدار التوكنات
        /// Authenticate user and issue tokens
        /// </summary>
        Task<AuthResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// تجديد رمز الوصول باستخدام التوكن المنعش
        /// Refresh access token using refresh token
        /// </summary>
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// تغيير كلمة المرور
        /// Change user password
        /// </summary>
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// طلب إعادة تعيين كلمة المرور
        /// Request password reset
        /// </summary>
        Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// إعادة تعيين كلمة المرور باستخدام رمز
        /// Reset password using token
        /// </summary>
        Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// تأكيد البريد الإلكتروني باستخدام رمز
        /// Verify email using token
        /// </summary>
        Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// تفعيل المستخدم بعد التحقق من البريد
        /// Activate user account after email verification
        /// </summary>
        Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
} 