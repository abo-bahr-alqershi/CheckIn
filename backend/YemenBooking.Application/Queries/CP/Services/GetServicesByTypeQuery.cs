using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Services;

/// <summary>
/// استعلام للحصول على الخدمات حسب النوع
/// Query to get services by type
/// </summary>
public class GetServicesByTypeQuery : IRequest<PaginatedResult<ServiceDto>>
{
    /// <summary>
    /// نوع الخدمة
    /// Service type
    /// </summary>
    public string ServiceType { get; set; } = string.Empty;

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
} 