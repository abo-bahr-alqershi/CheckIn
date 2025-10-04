using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.Reports;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Reports;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر التقارير للعملاء
    /// Client Reports Controller
    /// </summary>
    public class ReportsController : BaseClientController
    {
        public ReportsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// الإبلاغ عن عقار
        /// Report a property
        /// </summary>
        /// <param name="command">بيانات التقرير</param>
        /// <returns>نتيجة الإبلاغ</returns>
        [HttpPost("property")]
        public async Task<ActionResult<ResultDto<ReportPropertyResponse>>> ReportProperty([FromBody] ReportPropertyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
