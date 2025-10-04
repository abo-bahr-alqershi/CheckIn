using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.CP.Sections;
using YemenBooking.Application.Queries.MobileApp.Sections;

namespace YemenBooking.Api.Controllers.Client
{
	public class SectionsController : BaseClientController
	{
		public SectionsController(IMediator mediator) : base(mediator) { }


		

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetSections([FromQuery] GetActiveSectionsForHomeQuery query)
		{
			var result = await _mediator.Send(query);
			return Ok(result);
		}

        [HttpGet("{sectionId}/items")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSectionItems(Guid sectionId, [FromQuery] GetSectionItemsQuery query)
        {
            query.SectionId = sectionId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
		
	}
}