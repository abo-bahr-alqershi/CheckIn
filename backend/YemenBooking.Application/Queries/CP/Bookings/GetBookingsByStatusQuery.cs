using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Bookings;

/// <summary>
/// استعلام للحصول على الحجوزات حسب الحالة
/// Query to get bookings by status
/// </summary>
public class GetBookingsByStatusQuery : IRequest<PaginatedResult<BookingDto>>
{
    /// <summary>
    /// حالة الحجز
    /// Booking status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 