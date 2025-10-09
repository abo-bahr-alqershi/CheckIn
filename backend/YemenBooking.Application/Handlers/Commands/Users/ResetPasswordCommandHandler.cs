using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using System.Collections.Generic;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر إعادة تعيين كلمة المرور باستخدام رمز
    /// </summary>
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResultDto<bool>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IAuthenticationService authenticationService,
            IPasswordHashingService passwordHashingService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _authenticationService = authenticationService;
            _passwordHashingService = passwordHashingService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إعادة تعيين كلمة المرور باستخدام الرمز");

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Token))
                return ResultDto<bool>.Failed("الرمز مطلوب");
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return ResultDto<bool>.Failed("كلمة المرور الجديدة مطلوبة");

            // تحقق من قوة كلمة المرور الجديدة
            var (isValid, issues) = await _passwordHashingService.ValidatePasswordStrengthAsync(request.NewPassword, cancellationToken);
            if (!isValid)
                return ResultDto<bool>.Failed($"كلمة المرور الجديدة غير قوية: {string.Join(", ", issues)}");

            // تنفيذ إعادة التعيين
            var result = await _authenticationService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);
            if (!result)
                return ResultDto<bool>.Failed("فشل إعادة تعيين كلمة المرور");

            // تسجيل التدقيق اليدوي كعملية حساسة بدون قيم حساسة
            await _auditService.LogAuditAsync(
                entityType: "User",
                entityId: _currentUserService.UserId,
                action: YemenBooking.Core.Entities.AuditAction.PASSWORD_RESET,
                oldValues: null,
                newValues: JsonSerializer.Serialize(new { PasswordReset = true }),
                performedBy: _currentUserService.UserId,
                notes: "كلمة المرور تم إعادة تعيينها بنجاح",
                cancellationToken: cancellationToken);

            return ResultDto<bool>.Succeeded(true);
        }
    }
} 