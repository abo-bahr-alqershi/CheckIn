using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Bookings;

/// <summary>
/// استعلام للحصول على خدمات الحجز
/// Query to get booking services
/// </summary>
public class GetBookingServicesQuery : IRequest<ResultDto<IEnumerable<ServiceDto>>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
} 