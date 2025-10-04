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
    /// معالج استعلام تفصيل الإيرادات الكلي للمنصة ضمن نطاق زمني
    /// Handles GetPlatformRevenueBreakdownQuery and returns platform revenue breakdown
    /// </summary>
    public class GetPlatformRevenueBreakdownQueryHandler : IRequestHandler<GetPlatformRevenueBreakdownQuery, ResultDto<RevenueBreakdownDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPlatformRevenueBreakdownQueryHandler> _logger;

        public GetPlatformRevenueBreakdownQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetPlatformRevenueBreakdownQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<RevenueBreakdownDto>> Handle(GetPlatformRevenueBreakdownQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحليل تفصيل إيرادات المنصة من {Start} إلى {End}", request.Range.StartDate, request.Range.EndDate);

            if (request.Range.StartDate > request.Range.EndDate)
            {
                _logger.LogWarning("نطاق التاريخ غير صالح");
                return ResultDto<RevenueBreakdownDto>.Failure("نطاق التاريخ غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<RevenueBreakdownDto>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            if (_currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التحليل");
                return ResultDto<RevenueBreakdownDto>.Failure("ليس لديك صلاحية الوصول إلى هذا التحليل");
            }

            var data = await _dashboardService.GetPlatformRevenueBreakdownAsync(request.Range);
            return ResultDto<RevenueBreakdownDto>.Ok(data);
        }
    }
} 