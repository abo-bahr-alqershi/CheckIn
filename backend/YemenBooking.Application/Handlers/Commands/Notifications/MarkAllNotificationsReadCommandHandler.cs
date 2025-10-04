using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Notifications;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Notifications
{
    /// <summary>
    /// معالج أمر وضع علامة قراءة على جميع الإشعارات لمستخدم معين
    /// </summary>
    public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, ResultDto<bool>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<MarkAllNotificationsReadCommandHandler> _logger;

        public MarkAllNotificationsReadCommandHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<MarkAllNotificationsReadCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResultDto<bool>> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء وضع علامة قراءة على جميع الإشعارات: RecipientId={RecipientId}", request.RecipientId);

            // التحقق من صحة المدخلات
            if (request.RecipientId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف المستلم مطلوب");

            // التحقق من الصلاحيات (المالك أو المسؤول)
            if (_currentUserService.Role != "Admin" && request.RecipientId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بوضع علامة القراءة على إشعارات هذا المستخدم");

            // التنفيذ: وضع علامة القراءة على جميع الإشعارات
            var success = await _notificationRepository.MarkAllUserNotificationsAsReadAsync(request.RecipientId, cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "MarkAllNotificationsRead",
                $"تم وضع علامة قراءة على جميع الإشعارات للمستخدم {request.RecipientId}",
                request.RecipientId,
                "Notification",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل وضع علامة القراءة على جميع الإشعارات للمستخدم: RecipientId={RecipientId}", request.RecipientId);
            return ResultDto<bool>.Succeeded(success, success ? "تم وضع علامة قراءة على جميع الإشعارات بنجاح" : "لم يكن هناك إشعارات غير مقروءة");
        }
    }
} 