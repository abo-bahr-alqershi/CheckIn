using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر إلغاء الحجز
/// Cancel booking command
/// </summary>
public class CancelBookingCommand : IRequest<ResultDto>
{
    /// <summary>
    /// معرّف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// سبب الإلغاء
    /// Cancellation reason
    /// </summary>
    public string Reason { get; set; } = null!;

    /// <summary>
    /// نوع الإلغاء
    /// Cancellation type
    /// </summary>
    public CancellationType CancellationType { get; set; }

    /// <summary>
    /// معرّف المستخدم الذي ألغى الحجز
    /// Cancelled by user ID
    /// </summary>
    public Guid CancelledByUserId { get; set; }

    /// <summary>
    /// هل يتم الاسترداد تلقائياً
    /// Auto refund
    /// </summary>
    public bool AutoRefund { get; set; } = true;

    /// <summary>
    /// مبلغ الاسترداد (اختياري)
    /// Refund amount (optional - calculated if not provided)
    /// </summary>
    public MoneyDto? RefundAmount { get; set; }

    /// <summary>
    /// رسوم الإلغاء
    /// Cancellation fees
    /// </summary>
    public MoneyDto? CancellationFees { get; set; }

    /// <summary>
    /// ملاحظات إضافية
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    

    /// <summary>
    /// هل يتم إرسال إشعار للفندق
    /// Send hotel notification
    /// </summary>
    public bool SendHotelNotification { get; set; } = true;

    /// <summary>
    /// هل هو إلغاء قسري
    /// Force cancellation (bypass cancellation policy)
    /// </summary>
    public bool ForceCancellation { get; set; } = false;

    /// <summary>
    /// طريقة الاسترداد المفضلة
    /// Preferred refund method
    /// </summary>
    public RefundMethod? PreferredRefundMethod { get; set; }

    /// <summary>
    /// المهلة الزمنية للاسترداد (بالأيام)
    /// Refund processing time (in days)
    /// </summary>
    public int? RefundProcessingDays { get; set; }

    
}