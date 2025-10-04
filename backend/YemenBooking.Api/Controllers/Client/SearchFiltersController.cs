using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.MobileApp.SearchFilters;
using YemenBooking.Application.DTOs;
using System.Collections.Generic;
using YemenBooking.Application.DTOs.Statistics;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر فلاتر البحث للعملاء
    /// Client Search Filters Controller
    /// </summary>
    public class SearchFiltersController : BaseClientController
    {
        public SearchFiltersController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// الحصول على الوجهات الشائعة
        /// Get popular destinations
        /// </summary>
        /// <returns>قائمة الوجهات الشائعة</returns>
        [HttpGet("popular-destinations")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<List<PopularDestinationDto>>>> GetPopularDestinations()
        {
            var query = new GetPopularDestinationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على العقارات الموصى بها
        /// Get recommended properties
        /// </summary>
        /// <param name="query">معايير التوصية</param>
        /// <returns>قائمة العقارات الموصى بها</returns>
        [HttpGet("recommended-properties")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<PaginatedResult<PropertySearchResultDto>>>> GetRecommendedProperties([FromQuery] GetRecommendedPropertiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على فلاتر البحث
        /// Get search filters
        /// </summary>
        /// <returns>فلاتر البحث المتاحة</returns>
        [HttpGet("filters")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<SearchFiltersDto>>> GetSearchFilters()
        {
            var query = new GetSearchFiltersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
