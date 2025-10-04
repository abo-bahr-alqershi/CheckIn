using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.SearchLogs;

namespace YemenBooking.Api.Controllers.Admin
{
    [Route("api/admin/search-analytics")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SearchAnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchAnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// جلب تحليلات البحث
        /// Get search analytics with optional date filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultDto<SearchAnalyticsDto>>> GetSearchAnalytics([FromQuery] GetSearchAnalyticsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
} 