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
using YemenBooking.Application.Exceptions;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.Commands.Reports
{
    /// <summary>
    /// معالج أمر اتخاذ إجراء على البلاغ
    /// </summary>
    public class TakeReportActionCommandHandler : IRequestHandler<TakeReportActionCommand, ResultDto<bool>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<TakeReportActionCommandHandler> _logger;

        public TakeReportActionCommandHandler(
            IReportRepository reportRepository,
            IAuditService auditService,
            ILogger<TakeReportActionCommandHandler> logger)
        {
            _reportRepository = reportRepository;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(TakeReportActionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء اتخاذ إجراء على البلاغ: Id={ReportId}, Action={Action}", request.Id, request.Action);

            if (request.Id == Guid.Empty)
                return ResultDto<bool>.Failed("معرف البلاغ مطلوب");

            var report = await _reportRepository.GetReportByIdAsync(request.Id, cancellationToken);
            if (report == null)
                return ResultDto<bool>.Failed("البلاغ غير موجود");

            // Update status and action note
            report.Status = request.Action;
            report.ActionNote = request.ActionNote;
            report.AdminId = request.AdminId;
            report.UpdatedAt = DateTime.UtcNow;

            await _reportRepository.UpdateReportAsync(report, cancellationToken);

            // Log audit
            await _auditService.LogBusinessOperationAsync(
                "TakeReportAction",
                $"تم {request.Action} على البلاغ {request.Id}",
                request.Id,
                "Report",
                request.AdminId,
                new Dictionary<string, object?>
                {
                    { "ActionNote", request.ActionNote }
                },
                cancellationToken);

            _logger.LogInformation("اكتمل تنفيذ الإجراء على البلاغ: Id={ReportId}", request.Id);
            return ResultDto<bool>.Succeeded(true, "تم تنفيذ الإجراء على البلاغ بنجاح");
        }
    }
} 