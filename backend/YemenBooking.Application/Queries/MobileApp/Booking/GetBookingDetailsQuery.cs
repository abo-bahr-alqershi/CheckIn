using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Bookings;
using System;

namespace YemenBooking.Application.Queries.MobileApp.Booking;

/// <summary>
/// استعلام الحصول على تفاصيل الحجز
/// Query to get booking details
/// </summary>
public class GetBookingDetailsQuery : IRequest<ResultDto<BookingDetailsDto>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
    
    /// <summary>
    /// معرف المستخدم (للتحقق من الصلاحية)
    /// User ID (for authorization check)
    /// </summary>
    public Guid UserId { get; set; }
}
