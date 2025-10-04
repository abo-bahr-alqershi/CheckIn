using System;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO للملخص المالي
    /// DTO for financial summary
    /// </summary>
    public class FinancialSummaryDto
    {
        /// <summary>
        /// إجمالي الإيرادات
        /// Total revenue
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// إجمالي الحجوزات
        /// Total bookings
        /// </summary>
        public int TotalBookings { get; set; }

        /// <summary>
        /// متوسط قيمة الحجز
        /// Average booking value
        /// </summary>
        public decimal AverageBookingValue { get; set; }
    }
} 