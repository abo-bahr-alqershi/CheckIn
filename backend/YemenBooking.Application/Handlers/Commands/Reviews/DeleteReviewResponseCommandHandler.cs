using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Reviews
{
    /// <summary>
    /// معالج أمر حذف رد تقييم
    /// </summary>
    public class DeleteReviewResponseCommandHandler : IRequestHandler<DeleteReviewResponseCommand, ResultDto<bool>>
    {
        private readonly IReviewResponseRepository _responseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteReviewResponseCommandHandler> _logger;

        public DeleteReviewResponseCommandHandler(
            IReviewResponseRepository responseRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteReviewResponseCommandHandler> logger)
        {
            _responseRepository = responseRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteReviewResponseCommand request, CancellationToken cancellationToken)
        {
            if (request.ResponseId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الرد مطلوب");

            // Only Admins can delete responses (or the creator)
            // Load entity via base repository
            var exist = await _responseRepository.GetByIdAsync(request.ResponseId, cancellationToken);
            if (exist == null)
                return ResultDto<bool>.Failed("الرد غير موجود");

            var isAuthorized = _currentUserService.Role == "Admin" || exist.CreatedBy == _currentUserService.UserId;
            if (!isAuthorized)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذا الرد");

            var ok = await _responseRepository.DeleteAsync(request.ResponseId, cancellationToken);
            if (!ok) return ResultDto<bool>.Failed("فشل حذف الرد");

            await _auditService.LogBusinessOperationAsync(
                "DeleteReviewResponse",
                $"تم حذف رد {request.ResponseId}",
                request.ResponseId,
                nameof(Core.Entities.ReviewResponse),
                _currentUserService.UserId,
                null,
                cancellationToken);

            return ResultDto<bool>.Ok(true, "تم حذف الرد بنجاح");
        }
    }
}

