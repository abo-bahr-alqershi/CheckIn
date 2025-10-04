namespace YemenBooking.Application.Queries.Chat
{
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// استعلام لجلب المحادثات الخاصة بالمستخدم الحالي (بالتقسيم)
    /// Query to get chat conversations for the current user with pagination
    /// </summary>
    public class GetConversationsByParticipantQuery : IRequest<PaginatedResult<ChatConversationDto>>
    {
        /// <summary>
        /// رقم الصفحة
        /// Page number
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
} 