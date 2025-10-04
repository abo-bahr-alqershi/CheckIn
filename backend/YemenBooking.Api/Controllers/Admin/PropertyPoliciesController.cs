using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Policies;
using YemenBooking.Application.Queries.Policies;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بسياسات الكيانات للمدراء
    /// Controller for managing property policies by admins
    /// </summary>
    public class PropertyPoliciesController : BaseAdminController
    {
        public PropertyPoliciesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء سياسة جديدة للكيان
        /// Create a new property policy
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePropertyPolicy([FromBody] CreatePropertyPolicyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث سياسة للكيان
        /// Update an existing property policy
        /// </summary>
        [HttpPut("{policyId}")]
        public async Task<IActionResult> UpdatePropertyPolicy(Guid policyId, [FromBody] UpdatePropertyPolicyCommand command)
        {
            command.PolicyId = policyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف سياسة للكيان
        /// Delete a property policy
        /// </summary>
        [HttpDelete("{policyId}")]
        public async Task<IActionResult> DeletePropertyPolicy(Guid policyId)
        {
            var command = new DeletePropertyPolicyCommand { PolicyId = policyId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب جميع سياسات كيان معين
        /// Get all property policies
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPropertyPolicies([FromQuery] GetPropertyPoliciesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب سياسة معينة حسب المعرف
        /// Get a property policy by ID
        /// </summary>
        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicyById(Guid policyId)
        {
            var query = new GetPolicyByIdQuery { PolicyId = policyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب السياسات حسب النوع مع الصفحات
        /// Get policies by type with pagination
        /// </summary>
        [HttpGet("by-type")]
        public async Task<IActionResult> GetPoliciesByType([FromQuery] GetPoliciesByTypeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 