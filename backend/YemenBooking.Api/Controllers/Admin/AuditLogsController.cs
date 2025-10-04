using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.AuditLog;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بسجلات نشاط وتدقيق النظام للمدراء
    /// Controller for admin activity and audit logs
    /// </summary>
    public class AuditLogsController : BaseAdminController
    {
        public AuditLogsController(IMediator mediator) : base(mediator) { }


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

    }
} 