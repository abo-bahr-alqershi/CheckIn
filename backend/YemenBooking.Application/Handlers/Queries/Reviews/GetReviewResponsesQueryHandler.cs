using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Reviews
{
    /// <summary>
    /// معالج استعلام جلب ردود التقييم
    /// </summary>
    public class GetReviewResponsesQueryHandler : IRequestHandler<GetReviewResponsesQuery, ResultDto<List<ReviewResponseDto>>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewResponseRepository _responseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReviewResponsesQueryHandler> _logger;

        public GetReviewResponsesQueryHandler(
            IReviewRepository reviewRepository,
            IReviewResponseRepository responseRepository,
            ICurrentUserService currentUserService,
            ILogger<GetReviewResponsesQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _responseRepository = responseRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<List<ReviewResponseDto>>> Handle(GetReviewResponsesQuery request, CancellationToken cancellationToken)
        {
            if (request.ReviewId == Guid.Empty)
                return ResultDto<List<ReviewResponseDto>>.Failed("معرف التقييم مطلوب");

            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                return ResultDto<List<ReviewResponseDto>>.Failed("التقييم غير موجود");

            // Admin or property staff can view
            var isAuthorized = _currentUserService.Role == "Admin" ||
                               (_currentUserService.PropertyId.HasValue && _currentUserService.PropertyId.Value == review.PropertyId);
            if (!isAuthorized)
                return ResultDto<List<ReviewResponseDto>>.Failed("غير مصرح لك بعرض الردود");

            var responses = await _responseRepository.GetByReviewIdAsync(request.ReviewId, cancellationToken);
            var list = responses.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                ReviewId = r.ReviewId,
                ResponseText = r.Text,
                RespondedBy = r.RespondedBy,
                RespondedByName = r.RespondedByName,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return ResultDto<List<ReviewResponseDto>>.Ok(list);
        }
    }
}

