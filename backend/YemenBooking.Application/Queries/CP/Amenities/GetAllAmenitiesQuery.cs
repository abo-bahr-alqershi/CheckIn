using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using System.Collections.Generic;

namespace YemenBooking.Application.Queries.Amenities;

/// <summary>
/// استعلام للحصول على جميع المرافق
/// Query to get all amenities
/// </summary>
public class GetAllAmenitiesQuery : IRequest<PaginatedResult<AmenityDto>>
{
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
    /// مصطلح البحث النصي (اختياري)
    /// Full-text search term (optional)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by related property ID (optional)
    /// </summary>
    public Guid? PropertyId { get; set; }

    /// <summary>
    /// Filter amenities assigned to property or globally (optional)
    /// </summary>
    public bool? IsAssigned { get; set; }

    /// <summary>
    /// Filter amenities free (extra cost == 0) (optional)
    /// </summary>
    public bool? IsFree { get; set; }

    /// <summary>
    /// Filter amenities that belong to a specific property type
    /// </summary>
    public Guid? PropertyTypeId { get; set; }
} 