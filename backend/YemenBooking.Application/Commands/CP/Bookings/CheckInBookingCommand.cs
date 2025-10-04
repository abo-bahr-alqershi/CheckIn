using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لتسجيل الوصول لحجز
/// Command to check-in a booking
/// </summary>
public class CheckInBookingCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
}

