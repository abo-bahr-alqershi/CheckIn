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
    /// معالج أمر إعادة إرسال رابط التحقق للبريد الإلكتروني لتفعيل الحساب
    /// </summary>
    public class ResendEmailVerificationLinkCommandHandler : IRequestHandler<ResendEmailVerificationLinkCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ResendEmailVerificationLinkCommandHandler> _logger;

        public ResendEmailVerificationLinkCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ResendEmailVerificationLinkCommandHandler> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ResendEmailVerificationLinkCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إعادة إرسال رابط التحقق للبريد الإلكتروني: Email={Email}", request.Email);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<bool>.Failed("البريد الإلكتروني مطلوب");

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetUserByEmailAsync(request.Email.Trim(), cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");

            // التحقق من حالة التأكيد
            if (user.EmailConfirmed)
                return ResultDto<bool>.Failed("البريد الإلكتروني مؤكد بالفعل");

            // إنشاء رمز تأكيد جديد
            var token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(2);
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            // إرسال بريد التحقق
            var subject = "تأكيد البريد الإلكتروني";
            var body = $"<p>مرحباً {user.Name},</p><p>رمز تأكيد بريدك الإلكتروني هو: <strong>{token}</strong></p>";
            var emailSent = await _emailService.SendEmailAsync(user.Email, subject, body, true, cancellationToken);
            if (!emailSent)
                return ResultDto<bool>.Failed("فشل في إرسال بريد التحقق");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "ResendEmailVerification",
                $"تم إرسال رمز التحقق للمستخدم {user.Id}",
                user.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إرسال رمز التحقق بنجاح: Email={Email}", user.Email);
            return ResultDto<bool>.Succeeded(true, "تم إرسال رمز التحقق إلى البريد الإلكتروني");
        }
    }
} 