using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using System.Text.Json;
using YemenBooking.Application.Interfaces.Services;
using System.Linq;
 
 namespace YemenBooking.Application.Handlers.Commands.Chat
 {
     /// <summary>
     /// معالج أمر تعديل محتوى رسالة المحادثة
     /// Handler for EditMessageCommand
     /// </summary>
     public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, ResultDto<ChatMessageDto>>
     {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFirebaseService _firebaseService;
 
        public EditMessageCommandHandler(
            IChatMessageRepository messageRepo,
            IChatConversationRepository conversationRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFirebaseService firebaseService)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebaseService = firebaseService;
        }
 
        public async Task<ResultDto<ChatMessageDto>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepo.GetByIdAsync(request.MessageId, cancellationToken);
            if (message == null)
                return ResultDto<ChatMessageDto>.Failed("الرسالة غير موجودة");

            message.Content = request.Content;
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;

            await _messageRepo.UpdateAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ChatMessageDto>(message);
            // إشعار بقية المشاركين عبر FCM
            var conversation = await _conversationRepo.GetByIdAsync(message.ConversationId, cancellationToken);
            foreach (var participant in conversation.Participants.Where(p => p.Id != message.SenderId))
            {
                await _firebaseService.SendNotificationAsync($"user_{participant.Id}", "تم تعديل رسالة", dto.Content ?? string.Empty, new System.Collections.Generic.Dictionary<string, string>
                {
                    { "type", "message_updated" },
                    { "conversation_id", message.ConversationId.ToString() },
                    { "message_id", message.Id.ToString() }
                }, cancellationToken);
            }
            return ResultDto<ChatMessageDto>.Ok(dto, "تم تعديل المحتوى بنجاح");
        }
    }
} 