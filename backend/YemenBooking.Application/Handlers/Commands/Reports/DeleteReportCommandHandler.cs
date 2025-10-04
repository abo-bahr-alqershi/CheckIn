using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Reports;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.Commands.Reports
{
    /// <summary>
    /// معالج أمر حذف بلاغ
    /// </summary>
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, ResultDto<bool>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteReportCommandHandler> _logger;

        public DeleteReportCommandHandler(
            IReportRepository reportRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteReportCommandHandler> logger)
        {
            _reportRepository = reportRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف البلاغ: Id={ReportId}", request.Id);

            // التحقق من المدخلات
            if (request.Id == Guid.Empty)
                return ResultDto<bool>.Failed("معرف البلاغ مطلوب");

            // التحقق من الوجود
            var report = await _reportRepository.GetReportByIdAsync(request.Id, cancellationToken);
            if (report == null)
                return ResultDto<bool>.Failed("البلاغ غير موجود");

            // التحقق من الصلاحيات
            if (_currentUserService.Role != "Admin" && report.ReporterUserId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذا البلاغ");

            // تنفيذ الحذف
            bool deleted = await _reportRepository.DeleteReportAsync(request.Id, cancellationToken);
            if (!deleted)
                return ResultDto<bool>.Failed("فشل حذف البلاغ");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "DeleteReport",
                $"تم حذف البلاغ {request.Id}",
                request.Id,
                "Report",
                _currentUserService.UserId,
                new Dictionary<string, object> { { "DeletionReason", request.DeletionReason } },
                cancellationToken);

            _logger.LogInformation("اكتمل حذف البلاغ بنجاح: Id={ReportId}", request.Id);
            return ResultDto<bool>.Succeeded(true, "تم حذف البلاغ بنجاح");
        }
    }
} 