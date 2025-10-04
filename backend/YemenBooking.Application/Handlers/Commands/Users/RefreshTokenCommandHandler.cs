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
    /// معالج أمر تجديد رمز الوصول باستخدام التوكن المنعش
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultDto<AuthResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuditService _auditService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IAuthenticationService authenticationService,
            IAuditService auditService,
            IUserRepository userRepository,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _authenticationService = authenticationService;
            _auditService = auditService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ResultDto<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تجديد رمز الوصول");

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return ResultDto<AuthResultDto>.Failed("رمز التحديث مطلوب");

            // تنفيذ التجديد
            YemenBooking.Core.DTOs.Common.AuthResultDto coreResult;
            try
            {
                coreResult = await _authenticationService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            }
            catch (Exception)
            {
                return ResultDto<AuthResultDto>.Failed("فشل تجديد رمز الوصول");
            }
            // التحقق من حالة المستخدم
            var userEntity = await _userRepository.GetUserByIdAsync(coreResult.UserId, cancellationToken);
            if (userEntity == null)
                return ResultDto<AuthResultDto>.Failed("المستخدم غير موجود");
            if (!userEntity.EmailConfirmed)
                return ResultDto<AuthResultDto>.Failed("يجب تأكيد البريد الإلكتروني قبل تجديد الرمز");
            if (!userEntity.IsActive)
                return ResultDto<AuthResultDto>.Failed("لا يمكن تجديد الرمز لحساب معطل");
            // تحويل نتيجة المصادقة إلى DTO الخاص بالتطبيق
            var authResult = new AuthResultDto
            {
                AccessToken = coreResult.AccessToken,
                RefreshToken = coreResult.RefreshToken,
                ExpiresAt = coreResult.ExpiresAt,
                UserId = coreResult.UserId,
                UserName = coreResult.UserName,
                Email = coreResult.Email,
                Role = coreResult.Role,
                AccountRole = coreResult.AccountRole,
                ProfileImage = coreResult.ProfileImage,
                PropertyName = coreResult.PropertyName,
                PropertyId = coreResult.PropertyId,
                StaffId = coreResult.StaffId,
                PropertyCurrency = coreResult.PropertyCurrency
            };

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "RefreshToken",
                $"تم تجديد رمز الوصول للمستخدم {authResult.UserId}",
                authResult.UserId,
                "User",
                authResult.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تجديد رمز الوصول بنجاح: UserId={UserId}", authResult.UserId);
            return ResultDto<AuthResultDto>.Succeeded(authResult, "تم تجديد رمز الوصول بنجاح");
        }
    }
} 