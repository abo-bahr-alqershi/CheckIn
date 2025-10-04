using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لإلغاء الحجز
/// Command to cancel a booking
/// </summary>
public class CancelBookingCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
    /// <summary>
    /// سبب الإلغاء
    /// Cancellation reason
    /// </summary>
    public string CancellationReason { get; set; }
} 