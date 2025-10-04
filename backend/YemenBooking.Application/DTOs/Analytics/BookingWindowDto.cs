using System;

namespace YemenBooking.Application.DTOs.Analytics;

/// <summary>
/// بيانات تحليل نافذة الحجز
/// Booking window analysis data
/// </summary>
public class BookingWindowDto
{
    /// <summary>
    /// متوسط فترة الحجز بالأيام
    /// Average lead time in days
    /// </summary>
    public double AverageLeadTimeInDays { get; set; }

    /// <summary>
    /// عدد الحجوزات في اللحظة الأخيرة
    /// Bookings last minute count
    /// </summary>
    public int BookingsLastMinute { get; set; }
} 