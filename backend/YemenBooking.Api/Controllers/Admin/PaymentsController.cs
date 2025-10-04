using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Payments;
using YemenBooking.Application.Queries.Payments;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بمدفوعات النظام للمدراء
    /// Controller for payment operations by admins
    /// </summary>
    public class PaymentsController : BaseAdminController
    {
        public PaymentsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// استرجاع الدفع
        /// Refund a payment
        /// </summary>
        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء الدفع
        /// Void a payment
        /// </summary>
        [HttpPost("void")]
        public async Task<IActionResult> VoidPayment([FromBody] VoidPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة الدفع
        /// Update payment status
        /// </summary>
        [HttpPut("{paymentId}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid paymentId, [FromBody] UpdatePaymentStatusCommand command)
        {
            command.PaymentId = paymentId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب جميع المدفوعات مع دعم الفلاتر
        /// Get all payments with filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] GetAllPaymentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 