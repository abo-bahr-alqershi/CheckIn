using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Chat
{
    /// <summary>
    /// أمر حذف محادثة
    /// Command to delete a chat conversation by ID
    /// </summary>
    public class DeleteConversationCommand : IRequest<ResultDto>
    {
        /// <summary>
        /// معرف المحادثة
        /// Conversation identifier
        /// </summary>
        public Guid ConversationId { get; set; }
    }
} 