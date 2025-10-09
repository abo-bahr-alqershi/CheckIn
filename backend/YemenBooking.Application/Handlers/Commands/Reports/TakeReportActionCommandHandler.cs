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

            // تسجيل التدقيق (يدوي) يتضمن اسم ومعرّف المنفذ
            var notes = $"تم {request.Action} على البلاغ {request.Id} بواسطة {_currentUserService.Username} (ID={_currentUserService.UserId})";
            await _auditService.LogAuditAsync(
                entityType: "Report",
                entityId: request.Id,
                action: YemenBooking.Core.Enums.AuditAction.UPDATE,
                oldValues: null,
                newValues: System.Text.Json.JsonSerializer.Serialize(new { request.Action, request.ActionNote }),
                performedBy: _currentUserService.UserId,
                notes: notes,
                cancellationToken: cancellationToken);

            _logger.LogInformation("اكتمل تنفيذ الإجراء على البلاغ: Id={ReportId}", request.Id);
            return ResultDto<bool>.Succeeded(true, "تم تنفيذ الإجراء على البلاغ بنجاح");
        }
    }
} 