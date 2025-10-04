using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Chat;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Chat
{
    /// <summary>
    /// معالج استعلام جلب محادثة واحدة بناءً على المعرف
    /// Handler for GetConversationByIdQuery
    /// </summary>
    public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ResultDto<ChatConversationDto>>
    {
        private readonly IChatConversationRepository _repository;
        private readonly IMapper _mapper;

        public GetConversationByIdQueryHandler(IChatConversationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto<ChatConversationDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
        {
            // Load with details to include participants and messages for accurate lastMessage/updatedAt
            var conv = await _repository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken);
            if (conv == null)
                return ResultDto<ChatConversationDto>.Failure("المحادثة غير موجودة");

            var dto = _mapper.Map<ChatConversationDto>(conv);
            return ResultDto<ChatConversationDto>.Ok(dto);
        }
    }
} 