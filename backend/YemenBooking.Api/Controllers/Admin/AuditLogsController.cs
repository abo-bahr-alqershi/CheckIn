using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.AuditLog;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بسجلات نشاط وتدقيق النظام للمدراء
    /// Controller for admin activity and audit logs
    /// </summary>
    public class AuditLogsController : BaseAdminController
    {
        private readonly IAuditService _auditService;

        public AuditLogsController(IMediator mediator, IAuditService auditService) : base(mediator)
        {
            _auditService = auditService;
        }


        /// <summary>
        /// جلب سجلات التدقيق مع الفلاتر
        /// Get audit logs with optional filters (user, date range, search term, operation type)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب تفاصيل سجل تدقيق محدد (تشمل الحقول الثقيلة old/new/metadata)
        /// Get details for a single audit log (includes heavy JSON fields)
        /// </summary>
        [HttpGet("{auditLogId}/details")]
        public async Task<IActionResult> GetAuditLogDetails(Guid auditLogId, CancellationToken cancellationToken)
        {
            var log = await _auditService.GetAuditLogAsync(auditLogId, cancellationToken);
            if (log is null) return NotFound();

            var dto = new AuditLogDto
            {
                Id = log.Id,
                TableName = log.EntityType,
                Action = log.Action.ToString(),
                RecordId = log.EntityId ?? Guid.Empty,
                RecordName = (log.EntityId ?? Guid.Empty).ToString(),
                UserId = log.PerformedBy ?? Guid.Empty,
                Username = log.Username ?? string.Empty,
                Notes = log.Notes,
                OldValues = log.GetOldValues(),
                NewValues = log.GetNewValues(),
                Metadata = log.GetMetadata(),
                IsSlowOperation = log.IsSlowOperation,
                Changes = log.Notes ?? string.Empty,
                Timestamp = log.CreatedAt
            };
            return Ok(dto);
        }

    }
} 