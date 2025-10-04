namespace YemenBooking.Application.Queries.Chat
{
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// استعلام لجلب إعدادات الشات الخاصة بالمستخدم الحالي
    /// Query to get chat settings for the current user
    /// </summary>
    public class GetChatSettingsQuery : IRequest<ResultDto<ChatSettingsDto>>
    {
    }
} 