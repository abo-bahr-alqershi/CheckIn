using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Notifications
{
    /// <summary>
    /// بث إشعار لمجموعة من المستخدمين حسب معايير اختيار
    /// Broadcast notification to target users
    /// </summary>
    public class BroadcastNotificationCommand : IRequest<ResultDto<int>>
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Targeting
        public bool TargetAllUsers { get; set; } = false;
        public Guid[]? TargetUserIds { get; set; }
        public string[]? TargetRoles { get; set; }

        // Optional scheduling
        public DateTime? ScheduledFor { get; set; }
        public string Priority { get; set; } = "MEDIUM";
    }
}

