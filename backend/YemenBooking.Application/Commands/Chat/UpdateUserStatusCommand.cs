using System.Text.Json.Serialization;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Chat
{
    /// <summary>
    /// أمر لتحديث حالة المستخدم (online, offline, away, busy)
    /// Command to update user status
    /// </summary>
    public class UpdateUserStatusCommand : IRequest<ResultDto>
    {
        /// <summary>الحالة الجديدة للمستخدم</summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
} 