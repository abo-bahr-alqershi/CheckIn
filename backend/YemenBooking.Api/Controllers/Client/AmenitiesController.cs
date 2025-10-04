using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.MobileApp.Amenities;
using YemenBooking.Application.DTOs;
using System.Collections.Generic;
using YemenBooking.Application.DTOs.Amenities;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر إدارة المرافق للعملاء
    /// Client Amenities Management Controller
    /// </summary>
    public class AmenitiesController : BaseClientController
    {
        public AmenitiesController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// الحصول على جميع المرافق المتاحة
        /// Get all available amenities
        /// </summary>
        /// <returns>قائمة المرافق</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<List<AmenityDto>>>> GetAllAmenities([FromQuery] GetAllAmenitiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
