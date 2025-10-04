using System;
using System.Collections.Generic;
using System.Linq;
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
    /// معالج استعلام تحليل أسباب إلغاء الحجوزات ضمن نطاق زمني
    /// Handles GetPlatformCancellationAnalysisQuery and returns list of cancellation reasons analysis
    /// </summary>
    public class GetPlatformCancellationAnalysisQueryHandler : IRequestHandler<GetPlatformCancellationAnalysisQuery, ResultDto<List<CancellationReasonDto>>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPlatformCancellationAnalysisQueryHandler> _logger;

        public GetPlatformCancellationAnalysisQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetPlatformCancellationAnalysisQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<List<CancellationReasonDto>>> Handle(GetPlatformCancellationAnalysisQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحليل أسباب الإلغاء من {Start} إلى {End}", request.Range.StartDate, request.Range.EndDate);

            if (request.Range.StartDate > request.Range.EndDate)
            {
                _logger.LogWarning("نطاق التاريخ غير صالح");
                return ResultDto<List<CancellationReasonDto>>.Failure("نطاق التاريخ غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<List<CancellationReasonDto>>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            if (_currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التحليل");
                return ResultDto<List<CancellationReasonDto>>.Failure("ليس لديك صلاحية الوصول إلى هذا التحليل");
            }

            var data = await _dashboardService.GetPlatformCancellationAnalysisAsync(request.Range);
            var list = data.ToList();
            return ResultDto<List<CancellationReasonDto>>.Ok(list);
        }
    }
} 