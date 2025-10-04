using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Payments;

/// <summary>
/// أمر لتحديث حالة الدفع
/// Command to update payment status
/// </summary>
public class UpdatePaymentStatusCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الدفع
    /// Payment ID
    /// </summary>
    public Guid PaymentId { get; set; }

    /// <summary>
    /// الحالة الجديدة للدفع
    /// New payment status
    /// </summary>
    public PaymentStatus NewStatus { get; set; }
} 