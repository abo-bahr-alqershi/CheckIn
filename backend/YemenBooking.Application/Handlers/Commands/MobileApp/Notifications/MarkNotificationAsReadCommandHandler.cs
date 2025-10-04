using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Notifications; // updated namespace
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Notifications;

/// <summary>
/// معالج أمر تحديد إشعار كمقروء (تطبيق الجوال)
/// </summary>
public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository, IAuditService auditService, ILogger<MarkNotificationAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<MarkNotificationAsReadResponse> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("تحديد إشعار {NotificationId} كمقروء من قبل المستخدم {UserId}", request.NotificationId, request.UserId);

        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null)
            return new MarkNotificationAsReadResponse { Success = false, Message = "الإشعار غير موجود" };

        if (notification.RecipientId != request.UserId)
            return new MarkNotificationAsReadResponse { Success = false, Message = "غير مصرح لك" };

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        await _auditService.LogBusinessOperationAsync("MarkNotificationRead", $"تم وضع علامة مقروء على الإشعار {notification.Id}", notification.Id, "Notification", request.UserId, null, cancellationToken);

        return new MarkNotificationAsReadResponse { Success = true, Message = "تم التحديث بنجاح" };
    }
}
