namespace YemenBooking.Application.Commands.Chat
{
    using System;
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// أمر لإضافة تفاعل على رسالة
    /// Command to add a reaction to a chat message
    /// </summary>
    public class AddReactionCommand : IRequest<ResultDto>
    {
        /// <summary>
        /// معرف الرسالة
        /// Message ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// نوع التفاعل (like, love, laugh, etc.)
        /// Reaction type
        /// </summary>
        public string ReactionType { get; set; } = string.Empty;
    }
} 