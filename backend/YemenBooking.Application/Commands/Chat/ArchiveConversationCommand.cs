namespace YemenBooking.Application.Commands.Chat
{
    using System;
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// أمر لأرشفة المحادثة
    /// Command to archive a chat conversation
    /// </summary>
    public class ArchiveConversationCommand : IRequest<ResultDto>
    {
        /// <summary>
        /// معرف المحادثة المراد أرشفتها
        /// Conversation ID to archive
        /// </summary>
        public Guid ConversationId { get; set; }
    }
} 