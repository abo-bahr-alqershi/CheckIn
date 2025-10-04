using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Notifications;

/// <summary>
/// أمر لإنشاء إشعار جديد
/// Command to create a new notification
/// </summary>
public class CreateNotificationCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// نوع الإشعار
    /// Notification type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// عنوان الإشعار
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// محتوى الإشعار
    /// Notification message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// معرف المستلم
    /// Recipient identifier
    /// </summary>
    public Guid RecipientId { get; set; }

    /// <summary>
    /// معرف المرسل (اختياري)
    /// Sender identifier (optional)
    /// </summary>
    public Guid? SenderId { get; set; }
} 