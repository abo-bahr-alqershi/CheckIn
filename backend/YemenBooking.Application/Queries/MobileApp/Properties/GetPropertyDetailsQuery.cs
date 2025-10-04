using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.MobileApp.Properties;

/// <summary>
/// استعلام الحصول على تفاصيل عقار محدد
/// Query to get property details
/// </summary>
public class GetPropertyDetailsQuery : IRequest<ResultDto<PropertyDetailsDto>>
{
    /// <summary>
    /// معرف الكيان
    /// </summary>
    public Guid PropertyId { get; set; }
    
    /// <summary>
    /// معرف المستخدم (لمعرفة حالة المفضلات)
    /// </summary>
    public Guid? UserId { get; set; }
}