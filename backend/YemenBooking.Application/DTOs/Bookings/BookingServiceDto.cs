using System;

namespace YemenBooking.Application.DTOs.Bookings
{
    /// <summary>
    /// DTO لخدمة مرتبطة بالحجز
    /// Booking service DTO
    /// </summary>
    public class BookingServiceDto
    {
        /// <summary>
        /// معرف الخدمة
        /// Service identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// اسم الخدمة
        /// Service name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// الكمية
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// السعر الإجمالي للخدمة
        /// Total price
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// العملة
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "YER";
    }
}
