using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تأكيد البريد الإلكتروني باستخدام الرمز
    /// </summary>
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ResultDto<bool>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(
            IAuthenticationService authenticationService,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<VerifyEmailCommandHandler> logger)
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تأكيد البريد الإلكتروني باستخدام الرمز");

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Token))
                return ResultDto<bool>.Failed("الرمز مطلوب");

            // تنفيذ التحقق
            var verified = await _authenticationService.VerifyEmailAsync(request.Token, cancellationToken);
            if (!verified)
                return ResultDto<bool>.Failed("فشل تأكيد البريد الإلكتروني أو الرمز غير صالح");

            // تحديث حالة البريد المؤكد
            var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (user != null)
            {
                // حاليًا يمكن تحديث الحقل EmailConfirmed حسب المنطق الداخلي
            }

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "VerifyEmail",
                "تم تأكيد البريد الإلكتروني بنجاح",
                _currentUserService.UserId,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تأكيد البريد الإلكتروني بنجاح");
            return ResultDto<bool>.Succeeded(true, "تم تأكيد البريد الإلكتروني بنجاح");
        }
    }
} 