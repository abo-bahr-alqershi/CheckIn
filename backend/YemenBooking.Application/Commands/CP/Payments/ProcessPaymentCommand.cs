using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Payments;

/// <summary>
/// أمر لمعالجة الدفع
/// Command to process a payment
/// </summary>
public class ProcessPaymentCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// المبلغ المدفوع
    /// Amount paid
    /// </summary>
    public MoneyDto Amount { get; set; }

    /// <summary>
    /// طريقة الدفع
    /// Payment method
    /// </summary>
    public PaymentMethodEnum PaymentMethod { get; set; }

    /// <summary>
    /// معرف المعاملة
    /// Transaction ID
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;
} 