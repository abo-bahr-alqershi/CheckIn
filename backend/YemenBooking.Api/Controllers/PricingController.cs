// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using YemenBooking.Core.Entities;
// using YemenBooking.Core.Interfaces.Repositories;
// using YemenBooking.Application.DTOs;
// using AutoMapper;
// using System.Linq;
// using System.Threading;
// using MediatR;
// using YemenBooking.Application.Commands.Pricing;

// namespace YemenBooking.Api.Controllers
// {
//     /// <summary>
//     /// متحكم لإدارة قواعد التسعير
//     /// Controller for managing pricing rules
//     /// </summary>
//     [ApiController]
//     [Route("api/[controller]")]
//     public class PricingController : ControllerBase
//     {
//         private readonly IMediator _mediator;
//         private readonly IPricingRuleRepository _pricingRepository;
//         private readonly IMapper _mapper;

//         public PricingController(IMediator mediator, IPricingRuleRepository pricingRepository, IMapper mapper)
//         {
//             _mediator = mediator;
//             _pricingRepository = pricingRepository;
//             _mapper = mapper;
//         }

//         /// <summary>
//         /// جلب قواعد التسعير لوحدة معينة ضمن نطاق زمني
//         /// Get pricing rules for a unit within a date range
//         /// </summary>
//         [HttpGet]
//         public async Task<IActionResult> GetUnitPricing([FromQuery] Guid unitId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken = default)
//         {
//             var rules = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, startDate, endDate, cancellationToken);
//             var dtos = rules.Select(r => _mapper.Map<PricingRuleDto>(r));
//             return Ok(new { data = dtos });
//         }


//         /// <summary>
//         /// إنشاء قاعدة تسعير جديدة
//         /// Create a new pricing rule
//         /// </summary>
//         [HttpPost]
//         public async Task<IActionResult> CreatePricing([FromBody] CreatePricingRequestDto request, CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new CreatePricingCommand
//             {
//                 UnitId = request.UnitId,
//                 PriceType = request.PriceType,
//                 StartDate = request.StartDate,
//                 EndDate = request.EndDate,
//                 PriceAmount = request.PriceAmount,
//                 PricingTier = request.PricingTier,
//                 PercentageChange = request.PercentageChange,
//                 MinPrice = request.MinPrice,
//                 MaxPrice = request.MaxPrice,
//                 Description = request.Description,
//                 Currency = request.Currency
//             }, cancellationToken);
//             return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
//         }

//         /// <summary>
//         /// تحديث قاعدة تسعير موجودة
//         /// Update an existing pricing rule
//         /// </summary>
//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdatePricing([FromRoute] Guid id, [FromBody] UpdatePricingRequestDto request, CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new UpdatePricingCommand
//             {
//                 PricingRuleId = id,
//                 UnitId = request.UnitId,
//                 PriceType = request.PriceType,
//                 StartDate = request.StartDate,
//                 EndDate = request.EndDate,
//                 PriceAmount = request.PriceAmount,
//                 PricingTier = request.PricingTier,
//                 PercentageChange = request.PercentageChange,
//                 MinPrice = request.MinPrice,
//                 MaxPrice = request.MaxPrice,
//                 Description = request.Description,
//                 Currency = request.Currency
//             }, cancellationToken);
//             return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
//         }

//         /// <summary>
//         /// حذف قاعدة تسعير
//         /// Delete a pricing rule
//         /// </summary>
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeletePricing([FromRoute] Guid id, CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new DeletePricingCommand { PricingRuleId = id }, cancellationToken);
//             return result.IsSuccess ? NoContent() : NotFound();
//         }
//     }
// } 