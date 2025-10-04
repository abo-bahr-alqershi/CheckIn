using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users
{
    /// <summary>
    /// أمر لتحديث إعدادات المستخدم بصيغة JSON
    /// Command to update user settings JSON
    /// </summary>
    public class UpdateUserSettingsCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// إعدادات المستخدم بصيغة JSON
        /// User settings JSON
        /// </summary>
        public string SettingsJson { get; set; } = "{}";
    }
} 