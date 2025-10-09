using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Notifications; // updated namespace
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Notifications;

/// <summary>
/// معالج أمر تحديد إشعار كمقروء (تطبيق الجوال)
/// </summary>
public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository, IAuditService auditService, ICurrentUserService currentUserService, ILogger<MarkNotificationAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _auditService = auditService;
        _currentUserService = currentUserService;
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

        // تدقيق يدوي مع ذكر اسم ومعرف المنفذ
        var performerName = _currentUserService.Username;
        var performerId = _currentUserService.UserId;
        var notes = $"تم وضع علامة مقروء على الإشعار {notification.Id} بواسطة {performerName} (ID={performerId})";
        await _auditService.LogAuditAsync(
            entityType: "Notification",
            entityId: notification.Id,
            action: YemenBooking.Core.Entities.AuditAction.UPDATE,
            oldValues: null,
            newValues: JsonSerializer.Serialize(new { IsRead = true }),
            performedBy: performerId,
            notes: notes,
            cancellationToken: cancellationToken);

        return new MarkNotificationAsReadResponse { Success = true, Message = "تم التحديث بنجاح" };
    }
}
