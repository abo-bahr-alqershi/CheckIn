using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.ReportsAnalytics;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.ReportsAnalytics
{
    /// <summary>
    /// معالج استعلام تقرير الحجوزات
    /// Handler for booking report query
    /// </summary>
    public class GetBookingReportQueryHandler : IRequestHandler<GetBookingReportQuery, ResultDto<BookingReportDto>>
    {
        #region Dependencies
        private readonly IReportingService _reportingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetBookingReportQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetBookingReportQueryHandler(
            IReportingService reportingService,
            ICurrentUserService currentUserService,
            ILogger<GetBookingReportQueryHandler> logger)
        {
            _reportingService = reportingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<BookingReportDto>> Handle(GetBookingReportQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب تقرير الحجوزات من {StartDate} إلى {EndDate}, PropertyId: {PropertyId}",
                request.StartDate, request.EndDate, request.PropertyId);

            // 1. Validate date range
            if (request.StartDate > request.EndDate)
            {
                _logger.LogWarning("تاريخ البداية أكبر من تاريخ النهاية");
                return ResultDto<BookingReportDto>.Failure("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");
            }

            // 2. Authorization
            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<BookingReportDto>.Failure("يجب تسجيل الدخول لعرض تقرير الحجوزات");
            }
            var role = _currentUserService.Role;
            if (request.PropertyId.HasValue)
            {
                if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
                {
                    _logger.LogWarning("ليس لدى المستخدم الصلاحيات اللازمة لعرض تقرير الحجوزات");
                    return ResultDto<BookingReportDto>.Failure("ليس لديك صلاحية لعرض تقرير الحجوزات لهذا الكيان");
                }
            }
            else
            {
                if (role != "Admin")
                {
                    _logger.LogWarning("ليس لدى المستخدم الصلاحيات اللازمة لعرض التقرير العام");
                    return ResultDto<BookingReportDto>.Failure("ليس لديك صلاحية لعرض تقرير الحجوزات العام");
                }
            }

            // 3. Fetch report
            var rawReport = await _reportingService.GetBookingReportAsync(
                request.StartDate, request.EndDate, request.PropertyId, cancellationToken);

            // 4. Map to DTO
            var wrapper = new { Items = rawReport };
            var json = JsonSerializer.Serialize(wrapper);
            var reportDto = JsonSerializer.Deserialize<BookingReportDto>(json);
            if (reportDto == null)
            {
                _logger.LogError("فشل في تحليل بيانات تقرير الحجوزات");
                return ResultDto<BookingReportDto>.Failure("فشل في جلب بيانات تقرير الحجوزات");
            }

            _logger.LogInformation("تم جلب تقرير الحجوزات بنجاح");
            return ResultDto<BookingReportDto>.Ok(reportDto);
        }
        #endregion
    }
} 