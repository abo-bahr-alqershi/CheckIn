using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Reviews
{
    /// <summary>
    /// معالج أمر إضافة رد على تقييم
    /// </summary>
    public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, ResultDto<ReviewResponseDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewResponseRepository _responseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<RespondToReviewCommandHandler> _logger;

        public RespondToReviewCommandHandler(
            IReviewRepository reviewRepository,
            IReviewResponseRepository responseRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<RespondToReviewCommandHandler> logger,
            IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _responseRepository = responseRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ResultDto<ReviewResponseDto>> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.ReviewId == Guid.Empty || string.IsNullOrWhiteSpace(request.ResponseText))
                return ResultDto<ReviewResponseDto>.Failed("ReviewId and Text are required");

            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                return ResultDto<ReviewResponseDto>.Failed("التقييم غير موجود");

            // Only Admin or property staff can respond
            var isAuthorized = _currentUserService.Role == "Admin" ||
                               (_currentUserService.PropertyId.HasValue && _currentUserService.PropertyId.Value == review.PropertyId);
            if (!isAuthorized)
                return ResultDto<ReviewResponseDto>.Failed("غير مصرح لك بالرد على هذا التقييم");

            var responderId = request.OwnerId != Guid.Empty ? request.OwnerId : _currentUserService.UserId;
            var responder = await _userRepository.GetUserByIdAsync(responderId, cancellationToken);
            var responderName = responder?.Name ?? _currentUserService.Username ?? string.Empty;

            var entity = new ReviewResponse
            {
                Id = Guid.NewGuid(),
                ReviewId = request.ReviewId,
                Text = request.ResponseText.Trim(),
                RespondedAt = DateTime.UtcNow,
                RespondedBy = responderId,
                RespondedByName = responderName,
                CreatedBy = responderId
            };

            entity = await _responseRepository.CreateAsync(entity, cancellationToken);

            // Update quick fields on Review for CP app
            review.ResponseText = entity.Text;
            review.ResponseDate = entity.RespondedAt;
            review.UpdatedBy = responderId;
            review.UpdatedAt = DateTime.UtcNow;
            await _reviewRepository.UpdateReviewAsync(review, cancellationToken);

            await _auditService.LogBusinessOperationAsync(
                "CreateReviewResponse",
                $"تم إضافة رد على التقييم {request.ReviewId}",
                entity.Id,
                nameof(ReviewResponse),
                _currentUserService.UserId,
                null,
                cancellationToken);

            var dto = new ReviewResponseDto
            {
                Id = entity.Id,
                ReviewId = entity.ReviewId,
                ResponseText = entity.Text,
                RespondedBy = entity.RespondedBy,
                RespondedByName = entity.RespondedByName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            return ResultDto<ReviewResponseDto>.Ok(dto, "تم إضافة الرد بنجاح");
        }
    }
}

