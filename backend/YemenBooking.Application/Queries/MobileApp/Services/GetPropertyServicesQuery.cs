using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.MobileApp.Services;

/// <summary>
/// استعلام للحصول على خدمات الكيان
/// Query to get property services
/// </summary>
public class GetPropertyServicesQuery : IRequest<ResultDto<IEnumerable<ServiceDto>>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
} 