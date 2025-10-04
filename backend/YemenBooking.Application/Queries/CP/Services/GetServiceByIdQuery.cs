using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Services;

/// <summary>
/// استعلام للحصول على بيانات الخدمة بواسطة المعرف
/// Query to get service details by ID
/// </summary>
public class GetServiceByIdQuery : IRequest<ResultDto<ServiceDetailsDto>>
{
    /// <summary>
    /// معرف الخدمة
    /// Service ID
    /// </summary>
    public Guid ServiceId { get; set; }
} 