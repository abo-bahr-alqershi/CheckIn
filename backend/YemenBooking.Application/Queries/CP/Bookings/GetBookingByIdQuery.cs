using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Bookings;
using System;

namespace YemenBooking.Application.Queries.Bookings;

/// <summary>
/// استعلام للحصول على بيانات الحجز بواسطة المعرف
/// Query to get booking details by ID
/// </summary>
public class GetBookingByIdQuery : IRequest<ResultDto<BookingDetailsDto>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
} 