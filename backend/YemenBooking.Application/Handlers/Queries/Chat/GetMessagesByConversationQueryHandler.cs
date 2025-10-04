using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Chat
{
    /// <summary>
    /// معالج استعلام جلب الرسائل في محادثة محددة
    /// </summary>
    public class GetMessagesByConversationQueryHandler : IRequestHandler<GetMessagesByConversationQuery, PaginatedResult<ChatMessageDto>>
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMessagesByConversationQueryHandler> _logger;

        public GetMessagesByConversationQueryHandler(
            IChatMessageRepository messageRepo,
            IMapper mapper,
            ILogger<GetMessagesByConversationQueryHandler> logger)
        {
            _messageRepo = messageRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<ChatMessageDto>> Handle(GetMessagesByConversationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جلب الرسائل للمحادثة {ConversationId}", request.ConversationId);
            var beforeId = request.BeforeMessageId?.ToString();
            var (items, total) = await _messageRepo.GetMessagesByConversationAsync(request.ConversationId, request.PageNumber, request.PageSize, beforeId, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<ChatMessageDto>>(items);
            return PaginatedResult<ChatMessageDto>.Create(dtos, request.PageNumber, request.PageSize, total);
        }
    }
} 