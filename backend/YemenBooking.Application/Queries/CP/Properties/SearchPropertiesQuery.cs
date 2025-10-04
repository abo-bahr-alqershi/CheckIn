using MediatR;
using YemenBooking.Application.DTOs;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.Properties;

/// <summary>
/// استعلام للبحث عن الكيانات
/// Query to search for properties
/// </summary>
public class SearchPropertiesQuery : IRequest<PaginatedResult<PropertyDto>>
{
    /// <summary>
    /// الموقع (اختياري)
    /// Location (optional)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// الحد الأدنى للسعر (اختياري)
    /// Minimum price (optional)
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// الحد الأقصى للسعر (اختياري)
    /// Maximum price (optional)
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// معرف نوع الكيان (اختياري)
    /// Property type ID (optional)
    /// </summary>
    public Guid? PropertyTypeId { get; set; }

    /// <summary>
    /// تاريخ الدخول (اختياري)
    /// Check-in date (optional)
    /// </summary>
    public DateTime? CheckInDate { get; set; }

    /// <summary>
    /// تاريخ الخروج (اختياري)
    /// Check-out date (optional)
    /// </summary>
    public DateTime? CheckOutDate { get; set; }

    /// <summary>
    /// عدد الضيوف (اختياري)
    /// Number of guests (optional)
    /// </summary>
    public int? NumberOfGuests { get; set; }

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
    /// فلترة بالمرافق: السماح باختيار كيانات تحتوي على كل المرافق المحددة
    /// Amenity filter: include only properties containing all specified amenities
    /// </summary>
    public IEnumerable<Guid>? AmenityIds { get; set; }

    /// <summary>
    /// فلترة بالتقييم النجمي: عرض الكيانات ذات التقييمات المحددة
    /// Star ratings filter: include properties with specified star ratings
    /// </summary>
    public int[]? StarRatings { get; set; }

    /// <summary>
    /// فلترة بمتوسط تقييم أعلى من قيمة معينة
    /// Filter by minimum average rating
    /// </summary>
    public double? MinAverageRating { get; set; }

    /// <summary>
    /// فلترة بحالة الموافقة (مقبول/مرفوض)
    /// Filter by approval status
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// فلترة بحسب وجود حجوزات نشطة
    /// Filter by having active bookings
    /// </summary>
    public bool? HasActiveBookings { get; set; }

    /// <summary>
    /// خط عرض العميل (اختياري)
    /// Customer latitude (optional)
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// خط طول العميل (اختياري)
    /// Customer longitude (optional)
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// نصف قطر البحث بالكيلومتر (اختياري)
    /// Search radius in kilometers (optional)
    /// </summary>
    public double? RadiusKm { get; set; }

    /// <summary>
    /// خيارات الترتيب المتقدمة: rating, popularity, name_asc, name_desc
    /// Advanced sort options: rating, popularity, name_asc, name_desc
    /// </summary>
    public string? SortBy { get; set; }
} 