using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر المصادقة والتفويض للعملاء
    /// Client Authentication and Authorization Controller
    /// </summary>
    public class AuthController : BaseClientController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// تسجيل دخول المستخدم
        /// User login
        /// </summary>
        /// <param name="command">بيانات تسجيل الدخول</param>
        /// <returns>بيانات المستخدم ومعلومات الجلسة</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<ClientLoginUserResponse>>> Login([FromBody] ClientLoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// Register new user
        /// </summary>
        /// <param name="command">بيانات التسجيل</param>
        /// <returns>نتيجة التسجيل</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<RegisterUserResponse>>> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث رمز الوصول
        /// Refresh access token
        /// </summary>
        /// <param name="command">رمز التحديث</param>
        /// <returns>رمز وصول جديد</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل خروج المستخدم
        /// User logout
        /// </summary>
        /// <param name="command">بيانات تسجيل الخروج</param>
        /// <returns>نتيجة تسجيل الخروج</returns>
        [HttpPost("logout")]
        public async Task<ActionResult<ResultDto<LogoutResponse>>> Logout([FromBody] LogoutCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// Change password
        /// </summary>
        /// <param name="command">بيانات تغيير كلمة المرور</param>
        /// <returns>نتيجة تغيير كلمة المرور</returns>
        [HttpPost("change-password")]
        public async Task<ActionResult<ResultDto<ChangePasswordResponse>>> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// طلب إعادة تعيين كلمة المرور
        /// Request password reset
        /// </summary>
        /// <param name="command">بيانات طلب إعادة التعيين</param>
        /// <returns>نتيجة الطلب</returns>
        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<RequestPasswordResetResponse>>> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث ملف المستخدم الشخصي
        /// Update user profile
        /// </summary>
        /// <param name="command">بيانات التحديث</param>
        /// <returns>نتيجة التحديث</returns>
        [HttpPut("profile")]
        public async Task<ActionResult<ResultDto<UpdateUserProfileResponse>>> UpdateProfile([FromBody] UpdateUserProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث ملف المستخدم الشخصي (إصدار العميل)
        /// Update user profile (client version)
        /// </summary>
        /// <param name="command">بيانات التحديث</param>
        /// <returns>نتيجة التحديث</returns>
        [HttpPut("client-profile")]
        public async Task<ActionResult<ResultDto<ClientUserProfileResponse>>> UpdateClientProfile([FromBody] ClientUpdateUserProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// رفع صورة الملف الشخصي (multipart/form-data)
        /// Upload user profile image
        /// </summary>
        /// <param name="file">ملف الصورة</param>
        /// <returns>رابط الصورة الجديد</returns>
        [HttpPost("profile/image")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResultDto<UploadUserProfileImageResponse>>> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Ok(ResultDto<UploadUserProfileImageResponse>.Failed("لا توجد صورة مرفوعة", "FILE_EMPTY"));
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // استنتاج UserId من السياق إذا توفر CurrentUserService، أو تركه فارغًا للمعالجة داخل handler حسب الحاجة
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == "sub" || c.Type.EndsWith("/nameidentifier"))?.Value;
            Guid.TryParse(userIdClaim, out var userId);

            var command = new UploadUserProfileImageCommand
            {
                UserId = userId,
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileBytes = bytes,
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// التحقق من البريد الإلكتروني
        /// Verify email
        /// </summary>
        /// <param name="command">بيانات التحقق</param>
        /// <returns>نتيجة التحقق</returns>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<VerifyEmailResponse>>> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إعادة إرسال رمز التحقق من البريد الإلكتروني
        /// Resend email verification
        /// </summary>
        /// <param name="command">بيانات الإرسال</param>
        /// <returns>نتيجة الإرسال</returns>
        [HttpPost("resend-email-verification")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<ResendEmailVerificationResponse>>> ResendEmailVerification([FromBody] ResendEmailVerificationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف الحساب
        /// Delete account
        /// </summary>
        /// <param name="command">بيانات حذف الحساب</param>
        /// <returns>نتيجة الحذف</returns>
        [HttpDelete("account")]
        public async Task<ActionResult<ResultDto<DeleteAccountResponse>>> DeleteAccount([FromBody] DeleteAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
