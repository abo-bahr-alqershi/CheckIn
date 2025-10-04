using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Notifications.Commands;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Notifications;

/// <summary>
/// معالج أمر إخفاء إشعار للموبايل
/// </summary>
public class DismissNotificationCommandHandler : IRequestHandler<DismissNotificationCommand, DismissNotificationResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<DismissNotificationCommandHandler> _logger;

    public DismissNotificationCommandHandler(INotificationRepository notificationRepository, IAuditService auditService, ILogger<DismissNotificationCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<DismissNotificationResponse> Handle(DismissNotificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("إخفاء إشعار {NotificationId} من قبل المستخدم {UserId}", request.NotificationId, request.UserId);

        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null)
            return new DismissNotificationResponse { Success = false, Message = "الإشعار غير موجود" };

        if (notification.RecipientId != request.UserId)
            return new DismissNotificationResponse { Success = false, Message = "غير مصرح لك" };

        notification.IsDismissed = true;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        await _auditService.LogBusinessOperationAsync(
            "DismissNotification",
            $"تم إخفاء الإشعار {notification.Id}",
            notification.Id,
            "Notification",
            request.UserId,
            null,
            cancellationToken);

        return new DismissNotificationResponse { Success = true, Message = "تم الإخفاء بنجاح" };
    }
}
