using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Reports;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Reports
{
    /// <summary>
    /// معالج أمر تحديث بلاغ
    /// </summary>
    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, ResultDto<bool>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateReportCommandHandler> _logger;

        public UpdateReportCommandHandler(
            IReportRepository reportRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateReportCommandHandler> logger)
        {
            _reportRepository = reportRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث البلاغ: Id={ReportId}", request.Id);

            // التحقق من المدخلات
            if (request.Id == Guid.Empty)
                return ResultDto<bool>.Failed("معرف البلاغ مطلوب");
            if (string.IsNullOrWhiteSpace(request.Reason) && string.IsNullOrWhiteSpace(request.Description))
                return ResultDto<bool>.Failed("يجب تحديد سبب أو تفاصيل جديدة للتحديث");

            // التحقق من الوجود
            var report = await _reportRepository.GetReportByIdAsync(request.Id, cancellationToken);
            if (report == null)
                return ResultDto<bool>.Failed("البلاغ غير موجود");

            // التحقق من الصلاحيات
            if (_currentUserService.Role != "Admin" && report.ReporterUserId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث هذا البلاغ");

            // تنفيذ التحديث
            if (!string.IsNullOrWhiteSpace(request.Reason))
                report.Reason = request.Reason.Trim();
            if (!string.IsNullOrWhiteSpace(request.Description))
                report.Description = request.Description.Trim();
            report.UpdatedBy = _currentUserService.UserId;
            report.UpdatedAt = DateTime.UtcNow;

            await _reportRepository.UpdateReportAsync(report, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateReport",
                $"تم تحديث البلاغ {request.Id}",
                request.Id,
                "Report",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث البلاغ بنجاح: Id={ReportId}", request.Id);
            return ResultDto<bool>.Succeeded(true, "تم تحديث البلاغ بنجاح");
        }
    }
} 