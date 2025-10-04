using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Interfaces;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Analytics
{
    /// <summary>
    /// معالج استعلام تحليل نافذة الحجز لكيان محدد
    /// Handles GetBookingWindowAnalysisQuery and returns booking window statistics
    /// </summary>
    public class GetBookingWindowAnalysisQueryHandler : IRequestHandler<GetBookingWindowAnalysisQuery, ResultDto<BookingWindowDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetBookingWindowAnalysisQueryHandler> _logger;

        public GetBookingWindowAnalysisQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetBookingWindowAnalysisQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<BookingWindowDto>> Handle(GetBookingWindowAnalysisQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحليل نافذة الحجز للكيان: {PropertyId}", request.PropertyId);

            if (request.PropertyId == Guid.Empty)
            {
                _logger.LogWarning("معرف الكيان غير صالح");
                return ResultDto<BookingWindowDto>.Failure("معرف الكيان غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<BookingWindowDto>.Failure("يجب تسجيل الدخول لعرض هذا التحليل");
            }

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
            {
                _logger.LogWarning("ليس لدى المستخدم الصلاحية اللازمة لعرض هذا التحليل");
                return ResultDto<BookingWindowDto>.Failure("ليس لديك صلاحية لعرض هذا التحليل");
            }

            var data = await _dashboardService.GetBookingWindowAnalysisAsync(request.PropertyId);
            return ResultDto<BookingWindowDto>.Ok(data);
        }
    }
} 