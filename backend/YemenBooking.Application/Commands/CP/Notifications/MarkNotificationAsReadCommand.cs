using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Notifications;

/// <summary>
/// أمر لوضع علامة قراءة على إشعار
/// Command to mark notification as read
/// </summary>
public class MarkNotificationAsReadCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الإشعار
    /// Notification identifier
    /// </summary>
    public Guid NotificationId { get; set; }
} 