using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر طلب إعادة تعيين كلمة المرور
    /// </summary>
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء طلب إعادة تعيين كلمة المرور: Email={Email}", request.Email);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<bool>.Failed("البريد الإلكتروني مطلوب");

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetUserByEmailAsync(request.Email.Trim(), cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");

            // إنشاء رمز إعادة تعيين
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(2);
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            // إرسال بريد إعادة تعيين كلمة المرور
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.Name,
                token,
                cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "ForgotPassword",
                $"طلب استعادة كلمة المرور للمستخدم {user.Id}",
                user.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إرسال رابط استعادة كلمة المرور بنجاح: Email={Email}", user.Email);
            return ResultDto<bool>.Succeeded(true, "تم إرسال رابط استعادة كلمة المرور إلى البريد الإلكتروني");
        }
    }
} 