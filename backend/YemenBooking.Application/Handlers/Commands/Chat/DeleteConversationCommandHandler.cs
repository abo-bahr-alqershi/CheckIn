using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر حذف المحادثة
    /// Handler for DeleteConversationCommand
    /// </summary>
    public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, ResultDto>
    {
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteConversationCommandHandler(IChatConversationRepository conversationRepo, IUnitOfWork unitOfWork)
        {
            _conversationRepo = conversationRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
        {
            var conv = await _conversationRepo.GetByIdAsync(request.ConversationId);
            if (conv == null)
                return ResultDto.Failure("المحادثة غير موجودة");

            await _conversationRepo.DeleteAsync(conv, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ResultDto.Ok(null, "تم حذف المحادثة بنجاح");
        }
    }
} 