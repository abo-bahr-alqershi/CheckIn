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
    /// معالج أمر تغيير كلمة مرور المستخدم
    /// </summary>
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تغيير كلمة مرور المستخدم: UserId={UserId}", request.UserId);

            // التحقق من المدخلات
            if (request.UserId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف المستخدم مطلوب");
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return ResultDto<bool>.Failed("كلمة المرور الحالية والجديدة مطلوبة");
            
            // التحقق من الوجود
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");

            // التحقق من الصلاحيات (المستخدم نفسه أو مسؤول)
            if (_currentUserService.Role != "Admin" && user.Id != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتغيير كلمة مرور هذا المستخدم");

            // التحقق من كلمة المرور الحالية
            var validCurrent = await _passwordHashingService.VerifyPasswordAsync(
                request.CurrentPassword, user.Password, cancellationToken);
            if (!validCurrent)
                return ResultDto<bool>.Failed("كلمة المرور الحالية غير صحيحة");

            // تحقق من قوة كلمة المرور الجديدة
            var (isValid, issues) = await _passwordHashingService.ValidatePasswordStrengthAsync(
                request.NewPassword, cancellationToken);
            if (!isValid)
                return ResultDto<bool>.Failed($"كلمة المرور الجديدة غير قوية: {string.Join(", ", issues)}");

            // التنفيذ
            var hashedNew = await _passwordHashingService.HashPasswordAsync(request.NewPassword, cancellationToken);
            user.Password = hashedNew;
            user.UpdatedBy = _currentUserService.UserId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "ChangePassword",
                $"تم تغيير كلمة مرور المستخدم {request.UserId}",
                request.UserId,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تغيير كلمة المرور بنجاح: UserId={UserId}", request.UserId);
            return ResultDto<bool>.Succeeded(true, "تم تغيير كلمة المرور بنجاح");
        }
    }
} 