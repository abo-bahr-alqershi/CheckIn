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
    /// معالج أمر وضع علامة قراءة على إشعار
    /// </summary>
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ResultDto<bool>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

        public MarkNotificationAsReadCommandHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<MarkNotificationAsReadCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResultDto<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء وضع علامة قراءة على الإشعار: NotificationId={NotificationId}", request.NotificationId);

            // التحقق من صحة المدخلات
            if (request.NotificationId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الإشعار مطلوب");

            // التحقق من وجود الإشعار
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null)
                return ResultDto<bool>.Failed("الإشعار غير موجود");

            // التحقق من الصلاحيات (المتلقي أو المسؤول)
            if (_currentUserService.Role != "Admin" && notification.RecipientId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بوضع علامة القراءة على هذا الإشعار");

            // التنفيذ: وضع علامة القراءة
            var success = await _notificationRepository.MarkNotificationAsReadAsync(request.NotificationId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل وضع علامة القراءة على الإشعار");

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "MarkNotificationAsRead",
                $"تم وضع علامة قراءة على الإشعار {request.NotificationId}",
                request.NotificationId,
                "Notification",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("تم وضع علامة القراءة بنجاح على الإشعار: NotificationId={NotificationId}", request.NotificationId);
            return ResultDto<bool>.Succeeded(true, "تم وضع علامة القراءة بنجاح");
        }
    }
} 