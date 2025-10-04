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
    /// معالج استعلام تقرير العملاء
    /// Handler for customer report query
    /// </summary>
    public class GetCustomerReportQueryHandler : IRequestHandler<GetCustomerReportQuery, ResultDto<CustomerReportDto>>
    {
        #region Dependencies
        private readonly IReportingService _reportingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerReportQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetCustomerReportQueryHandler(
            IReportingService reportingService,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerReportQueryHandler> logger)
        {
            _reportingService = reportingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<CustomerReportDto>> Handle(GetCustomerReportQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب تقرير العملاء من {StartDate} إلى {EndDate}", request.StartDate, request.EndDate);

            // 1. Validate date range
            if (request.StartDate > request.EndDate)
            {
                _logger.LogWarning("تاريخ البداية أكبر من تاريخ النهاية");
                return ResultDto<CustomerReportDto>.Failure("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");
            }

            // 2. Authorization: admin only
            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null || _currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم الصلاحيات اللازمة لعرض تقرير العملاء");
                return ResultDto<CustomerReportDto>.Failure("ليس لديك صلاحية لعرض تقرير العملاء");
            }

            // 3. Fetch report
            var rawReport = await _reportingService.GetCustomerReportAsync(request.StartDate, request.EndDate, cancellationToken);

            // 4. Map to DTO
            var wrapper = new { Items = rawReport };
            var json = JsonSerializer.Serialize(wrapper);
            var reportDto = JsonSerializer.Deserialize<CustomerReportDto>(json);
            if (reportDto == null)
            {
                _logger.LogError("فشل في تحليل بيانات تقرير العملاء");
                return ResultDto<CustomerReportDto>.Failure("فشل في جلب بيانات تقرير العملاء");
            }

            _logger.LogInformation("تم جلب تقرير العملاء بنجاح");
            return ResultDto<CustomerReportDto>.Ok(reportDto);
        }
        #endregion
    }
} 