using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لتسجيل المغادرة لحجز
/// Command to check-out a booking
/// </summary>
public class CheckOutBookingCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
}

