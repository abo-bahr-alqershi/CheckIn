using MediatR;
using System;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Chat
{
    /// <summary>
    /// استعلام جلب محادثة واحدة بناءً على المعرف
    /// Query for retrieving a single chat conversation by its ID
    /// </summary>
    public class GetConversationByIdQuery : IRequest<ResultDto<ChatConversationDto>>
    {
        /// <summary>
        /// معرف المحادثة
        /// Conversation identifier
        /// </summary>
        public Guid ConversationId { get; set; }
    }
} 