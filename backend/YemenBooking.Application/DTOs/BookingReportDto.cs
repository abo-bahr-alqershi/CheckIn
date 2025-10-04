using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// تقرير الحجز
    /// Booking report DTO
    /// </summary>
    public class BookingReportDto
    {
        /// <summary>
        /// عناصر تقرير الحجوزات اليومية
        /// Daily booking report items
        /// </summary>
        public List<BookingReportItemDto> Items { get; set; } = new List<BookingReportItemDto>();
    }
} 