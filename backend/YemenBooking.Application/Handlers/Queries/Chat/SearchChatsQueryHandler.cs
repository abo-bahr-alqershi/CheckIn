namespace YemenBooking.Application.Handlers.Queries.Chat
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using YemenBooking.Application.DTOs;
    using YemenBooking.Application.Queries.Chat;
    using YemenBooking.Core.Interfaces.Repositories;

    /// <summary>
    /// معالج استعلام البحث في المحادثات والرسائل
    /// Handler for SearchChatsQuery
    /// </summary>
    public class SearchChatsQueryHandler : IRequestHandler<SearchChatsQuery, ResultDto<SearchChatsResultDto>>
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IMapper _mapper;

        public SearchChatsQueryHandler(
            IChatMessageRepository messageRepo,
            IChatConversationRepository conversationRepo,
            IMapper mapper)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _mapper = mapper;
        }

        public async Task<ResultDto<SearchChatsResultDto>> Handle(SearchChatsQuery request, CancellationToken cancellationToken)
        {
            var msgQuery = _messageRepo.GetQueryable()
                .Where(m => m.Content.Contains(request.Query));

            if (request.ConversationId.HasValue)
                msgQuery = msgQuery.Where(m => m.ConversationId == request.ConversationId);

            if (!string.IsNullOrEmpty(request.MessageType))
                msgQuery = msgQuery.Where(m => m.MessageType == request.MessageType);

            if (request.SenderId.HasValue)
                msgQuery = msgQuery.Where(m => m.SenderId == request.SenderId);

            if (request.DateFrom.HasValue)
                msgQuery = msgQuery.Where(m => m.CreatedAt >= request.DateFrom);

            if (request.DateTo.HasValue)
                msgQuery = msgQuery.Where(m => m.CreatedAt <= request.DateTo);

            var totalMessages = await msgQuery.CountAsync(cancellationToken);
            var messages = await msgQuery
                .OrderByDescending(m => m.CreatedAt)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var convQuery = _conversationRepo.GetQueryable()
                .Where(c => c.Title.Contains(request.Query) || c.Participants.Any(p => p.Id.ToString().Contains(request.Query)));

            if (request.ConversationId.HasValue)
                convQuery = convQuery.Where(c => c.Id == request.ConversationId);

            var totalConversations = await convQuery.CountAsync(cancellationToken);
            var conversations = await convQuery
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var resultDto = new SearchChatsResultDto
            {
                Messages = _mapper.Map<IEnumerable<ChatMessageDto>>(messages),
                Conversations = _mapper.Map<IEnumerable<ChatConversationDto>>(conversations),
                TotalCount = totalMessages + totalConversations,
                HasMore = (request.Page * request.Limit) < (totalMessages + totalConversations),
                NextPageNumber = ((request.Page * request.Limit) < (totalMessages + totalConversations)) ? request.Page + 1 : (int?)null
            };

            return ResultDto<SearchChatsResultDto>.Ok(resultDto);
        }
    }
} 