using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Notifications
{
    /// <summary>
    /// حذف إشعار
    /// Delete notification
    /// </summary>
    public class DeleteNotificationCommand : IRequest<ResultDto<bool>>
    {
        public Guid NotificationId { get; set; }
    }
}

