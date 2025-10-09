using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تحديث إعدادات المستخدم
    /// </summary>
    public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUserSettingsCommandHandler> _logger;

        public UpdateUserSettingsCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUserSettingsCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            _logger.LogInformation("بدء تحديث إعدادات المستخدم: UserId={UserId}", userId);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.SettingsJson))
                return ResultDto<bool>.Failed("إعدادات المستخدم مطلوبة");

            // تحديث الإعدادات في المستودع
            var updated = await _userRepository.UpdateUserSettingsAsync(userId, request.SettingsJson, cancellationToken);
            if (!updated)
                return ResultDto<bool>.Failed("فشل في تحديث إعدادات المستخدم");

            // تسجيل التدقيق اليدوي مع القيم الجديدة فقط (تجنب التسريب)
            await _auditService.LogAuditAsync(
                entityType: "User",
                entityId: userId,
                action: YemenBooking.Core.Entities.AuditAction.UPDATE,
                oldValues: null,
                newValues: JsonSerializer.Serialize(new { SettingsUpdated = true }),
                performedBy: userId,
                notes: $"تم تحديث إعدادات المستخدم للمستخدم {userId}",
                cancellationToken: cancellationToken);

            _logger.LogInformation("اكتملت عملية تحديث إعدادات المستخدم: UserId={UserId}", userId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث إعدادات المستخدم بنجاح");
        }
    }
} 