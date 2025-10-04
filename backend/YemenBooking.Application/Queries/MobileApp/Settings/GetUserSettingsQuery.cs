using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.MobileApp.Settings;

/// <summary>
/// استعلام الحصول على إعدادات المستخدم
/// Query to get user settings
/// </summary>
public class GetUserSettingsQuery : IRequest<ResultDto<UserSettingsDto>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
}