using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Queries.Units;

/// <summary>
/// استعلام للحصول على الوحدات المتاحة
/// Query to get available units
/// </summary>
public class GetAvailableUnitsQuery : IRequest<PaginatedResult<UnitDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// تاريخ الدخول
    /// Check-in date
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// تاريخ الخروج
    /// Check-out date
    /// </summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// عدد الضيوف
    /// Number of guests
    /// </summary>
    public int NumberOfGuests { get; set; }

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

    /// <summary>
    /// فلترة بالتوفر (اختياري)
    /// Filter by availability (optional)
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// فلترة بالسعر الأدنى (اختياري)
    /// Filter by minimum base price (optional)
    /// </summary>
    public decimal? MinBasePrice { get; set; }

    /// <summary>
    /// فلترة بالسعر الأقصى (اختياري)
    /// Filter by maximum base price (optional)
    /// </summary>
    public decimal? MaxBasePrice { get; set; }

    /// <summary>
    /// فلترة بالسعة (اختياري)
    /// Filter by minimum capacity (optional)
    /// </summary>
    public int? MinCapacity { get; set; }

    /// <summary>
    /// بحث بالاسم أو الرقم (اختياري)
    /// Name search: name or number (optional)
    /// </summary>
    public string? NameContains { get; set; }
} 