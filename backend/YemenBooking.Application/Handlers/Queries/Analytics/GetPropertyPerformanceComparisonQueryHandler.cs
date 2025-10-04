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
    /// معالج استعلام مقارنة أداء الكيان بين فترتين زمنيتين
    /// Handles GetPropertyPerformanceComparisonQuery and returns performance comparison
    /// </summary>
    public class GetPropertyPerformanceComparisonQueryHandler : IRequestHandler<GetPropertyPerformanceComparisonQuery, ResultDto<PerformanceComparisonDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyPerformanceComparisonQueryHandler> _logger;

        public GetPropertyPerformanceComparisonQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyPerformanceComparisonQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<PerformanceComparisonDto>> Handle(GetPropertyPerformanceComparisonQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء مقارنة أداء الكيان: {PropertyId} من {CurrentStart}-{CurrentEnd} مقابل {PrevStart}-{PrevEnd}",
                request.PropertyId, request.CurrentRange.StartDate, request.CurrentRange.EndDate, request.PreviousRange.StartDate, request.PreviousRange.EndDate);

            if (request.PropertyId == Guid.Empty)
            {
                _logger.LogWarning("معرف الكيان غير صالح");
                return ResultDto<PerformanceComparisonDto>.Failure("معرف الكيان غير صالح");
            }

            if (request.CurrentRange.StartDate > request.CurrentRange.EndDate || request.PreviousRange.StartDate > request.PreviousRange.EndDate)
            {
                _logger.LogWarning("نطاق التاريخ غير صالح");
                return ResultDto<PerformanceComparisonDto>.Failure("نطاق التاريخ غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<PerformanceComparisonDto>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التحليل");
                return ResultDto<PerformanceComparisonDto>.Failure("ليس لديك صلاحية الوصول إلى هذا التحليل");
            }

            var data = await _dashboardService.GetPropertyPerformanceComparisonAsync(request.PropertyId, request.CurrentRange, request.PreviousRange);
            return ResultDto<PerformanceComparisonDto>.Ok(data);
        }
    }
} 