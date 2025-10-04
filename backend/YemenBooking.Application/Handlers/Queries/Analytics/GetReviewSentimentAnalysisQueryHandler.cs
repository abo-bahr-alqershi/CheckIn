using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Analytics
{
    /// <summary>
    /// معالج استعلام تحليل مشاعر التقييمات لكيان محدد
    /// Handles GetReviewSentimentAnalysisQuery and returns review sentiment data
    /// </summary>
    public class GetReviewSentimentAnalysisQueryHandler : IRequestHandler<GetReviewSentimentAnalysisQuery, ResultDto<ReviewSentimentDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReviewSentimentAnalysisQueryHandler> _logger;

        public GetReviewSentimentAnalysisQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetReviewSentimentAnalysisQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<ReviewSentimentDto>> Handle(GetReviewSentimentAnalysisQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحليل مشاعر التقييمات للكيان: {PropertyId}", request.PropertyId);

            if (request.PropertyId == Guid.Empty)
            {
                _logger.LogWarning("معرف الكيان غير صالح");
                return ResultDto<ReviewSentimentDto>.Failure("معرف الكيان غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<ReviewSentimentDto>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التحليل");
                return ResultDto<ReviewSentimentDto>.Failure("ليس لديك صلاحية الوصول إلى هذا التحليل");
            }

            var data = await _dashboardService.GetReviewSentimentAnalysisAsync(request.PropertyId);
            return ResultDto<ReviewSentimentDto>.Ok(data);
        }
    }
} 