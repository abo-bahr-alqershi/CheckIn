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
    /// معالج استعلام تحليل أفواج العملاء ضمن نطاق زمني
    /// Handles GetCustomerCohortAnalysisQuery and returns customer cohort statistics
    /// </summary>
    public class GetCustomerCohortAnalysisQueryHandler : IRequestHandler<GetCustomerCohortAnalysisQuery, ResultDto<List<CohortDto>>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerCohortAnalysisQueryHandler> _logger;

        public GetCustomerCohortAnalysisQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerCohortAnalysisQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<List<CohortDto>>> Handle(GetCustomerCohortAnalysisQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحليل أفواج العملاء من {Start} إلى {End}", request.Range.StartDate, request.Range.EndDate);

            if (request.Range.StartDate > request.Range.EndDate)
            {
                _logger.LogWarning("نطاق التاريخ غير صالح");
                return ResultDto<List<CohortDto>>.Failure("نطاق التاريخ غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<List<CohortDto>>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            if (_currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التحليل");
                return ResultDto<List<CohortDto>>.Failure("ليس لديك صلاحية الوصول إلى هذا التحليل");
            }

            var data = await _dashboardService.GetCustomerCohortAnalysisAsync(request.Range);
            var list = data.ToList();
            return ResultDto<List<CohortDto>>.Ok(list);
        }
    }
} 