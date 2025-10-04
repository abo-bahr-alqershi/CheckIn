using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.MobileApp.PropertyTypes;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using System.Collections.Generic;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر أنواع العقارات للعملاء
    /// Client Property Types Controller
    /// </summary>
    public class PropertyTypesController : BaseClientController
    {
        public PropertyTypesController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// الحصول على جميع أنواع العقارات
        /// Get all property types
        /// </summary>
        /// <returns>قائمة أنواع العقارات</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<List<PropertyTypeDto>>>> GetPropertyTypes()
        {
            var query = new GetPropertyTypesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
