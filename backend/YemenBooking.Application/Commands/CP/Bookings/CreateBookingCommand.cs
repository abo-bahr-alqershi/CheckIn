using MediatR;
using YemenBooking.Application.DTOs;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لإنشاء حجز جديد
/// Command to create a new booking
/// </summary>
public class CreateBookingCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// تاريخ الوصول
    /// Check-in date
    /// </summary>
    public DateTime CheckIn { get; set; }

    /// <summary>
    /// تاريخ المغادرة
    /// Check-out date
    /// </summary>
    public DateTime CheckOut { get; set; }

    /// <summary>
    /// عدد الضيوف
    /// Number of guests
    /// </summary>
    public int GuestsCount { get; set; }

    /// <summary>
    /// قائمة الخدمات الإضافية
    /// List of additional service IDs
    /// </summary>
    public IEnumerable<Guid>? Services { get; set; }
} 