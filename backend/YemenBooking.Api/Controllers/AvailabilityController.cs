// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using YemenBooking.Core.Interfaces.Repositories;
// using YemenBooking.Application.Interfaces.Services;
// using YemenBooking.Application.DTOs;
// using AutoMapper;
// using YemenBooking.Core.Entities;
// using AvailabilitySearchDto = YemenBooking.Application.DTOs.AvailabilitySearchRequest;
// using MediatR;
// using YemenBooking.Application.Commands.Availability;
// using YemenBooking.Application.Queries.Availability;
// using System.Threading;

// namespace YemenBooking.Api.Controllers
// {
//     /// <summary>
//     /// متحكم لإدارة الإتاحة
//     /// Controller for managing unit availability
//     /// </summary>
//     [ApiController]
//     [Route("api/[controller]")]
//     public class AvailabilityController : ControllerBase
//     {
//         private readonly IMediator _mediator;

//         public AvailabilityController(
//             IMediator mediator)
//         {
//             _mediator = mediator;
//         }

//         /// <summary>
//         /// جلب إتاحة الوحدة لفترة معينة
//         /// Get unit availability within date range
//         /// </summary>
//         [HttpGet]
//         public async Task<IActionResult> GetUnitAvailability(
//             [FromQuery] Guid unitId,
//             [FromQuery] DateTime? startDate,
//             [FromQuery] DateTime? endDate,
//             CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new GetUnitAvailabilityQuery
//             {
//                 UnitId = unitId,
//                 StartDate = startDate,
//                 EndDate = endDate
//             }, cancellationToken);
//             return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
//         }

//         /// <summary>
//         /// إنشاء إتاحة جديدة
//         /// Create a new unit availability
//         /// </summary>
//         [HttpPost]
//         public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityRequestDto request, CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new CreateAvailabilityCommand
//             {
//                 UnitId = request.UnitId,
//                 StartDate = request.StartDate,
//                 EndDate = request.EndDate,
//                 Status = request.Status,
//                 Reason = request.Reason,
//                 Notes = request.Notes,
//                 OverrideConflicts = request.OverrideConflicts ?? false
//             }, cancellationToken);
//             return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
//         }


//         /// <summary>
//         /// تحديث إتاحة موجودة
//         /// Update an existing unit availability
//         /// </summary>
//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdateAvailability(
//             [FromRoute] Guid id,
//             [FromBody] UpdateAvailabilityRequestDto request,
//             CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new UpdateAvailabilityCommand
//             {
//                 AvailabilityId = id,
//                 UnitId = request.UnitId,
//                 StartDate = request.StartDate,
//                 EndDate = request.EndDate,
//                 Status = request.Status,
//                 Reason = request.Reason,
//                 Notes = request.Notes,
//                 OverrideConflicts = request.OverrideConflicts ?? false
//             }, cancellationToken);
//             return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
//         }

//         /// <summary>
//         /// حذف إتاحة
//         /// Delete a unit availability
//         /// </summary>
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteAvailability([FromRoute] Guid id, CancellationToken cancellationToken = default)
//         {
//             var result = await _mediator.Send(new DeleteAvailabilityCommand { AvailabilityId = id }, cancellationToken);
//             return result.IsSuccess ? NoContent() : NotFound();
//         }

//     }
// } 