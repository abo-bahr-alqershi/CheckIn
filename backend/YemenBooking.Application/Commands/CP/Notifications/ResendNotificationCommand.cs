using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Notifications
{
    /// <summary>
    /// إعادة إرسال إشعار فشل سابقًا
    /// Resend a previously failed notification
    /// </summary>
    public class ResendNotificationCommand : IRequest<ResultDto<bool>>
    {
        public Guid NotificationId { get; set; }
        public string? Channel { get; set; } // IN_APP/EMAIL/SMS/PUSH (optional)
    }
}

