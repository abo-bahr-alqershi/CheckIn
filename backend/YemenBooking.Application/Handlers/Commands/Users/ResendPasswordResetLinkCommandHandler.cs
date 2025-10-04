using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر إعادة إرسال رابط إعادة تعيين كلمة المرور
    /// </summary>
    public class ResendPasswordResetLinkCommandHandler : IRequestHandler<ResendPasswordResetLinkCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ResendPasswordResetLinkCommandHandler> _logger;

        public ResendPasswordResetLinkCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ResendPasswordResetLinkCommandHandler> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ResendPasswordResetLinkCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إعادة إرسال رابط إعادة تعيين كلمة المرور: Email={Email}", request.Email);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<bool>.Failed("البريد الإلكتروني مطلوب");

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetUserByEmailAsync(request.Email.Trim(), cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");

            // إنشاء رمز إعادة تعيين جديد
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(2);
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            // إرسال بريد إعادة تعيين كلمة المرور
            var emailSent = await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.Name,
                token,
                cancellationToken);
            if (!emailSent)
                return ResultDto<bool>.Failed("فشل في إرسال رابط إعادة تعيين كلمة المرور");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "ResendPasswordReset",
                $"تم إرسال رابط إعادة تعيين كلمة المرور للمستخدم {user.Id}",
                user.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إرسال رابط إعادة تعيين كلمة المرور: Email={Email}", user.Email);
            return ResultDto<bool>.Succeeded(true, "تم إرسال رابط إعادة تعيين كلمة المرور إلى البريد الإلكتروني");
        }
    }
} 