using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Payments;

/// <summary>
/// أمر لإبطال الدفعة
/// Command to void a payment
/// </summary>
public class VoidPaymentCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الدفعة
    /// Payment identifier
    /// </summary>
    public Guid PaymentId { get; set; }
}