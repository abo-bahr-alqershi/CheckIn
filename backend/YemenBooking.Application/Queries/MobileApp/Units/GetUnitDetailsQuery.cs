using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;

namespace YemenBooking.Application.Queries.MobileApp.Units;

/// <summary>
/// استعلام الحصول على تفاصيل الوحدة
/// Query to get unit details
/// </summary>
public class GetUnitDetailsQuery : IRequest<ResultDto<UnitDetailsDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// </summary>
    public Guid UnitId { get; set; }
    
    /// <summary>
    /// تاريخ الوصول (لحساب السعر)
    /// </summary>
    public DateTime? CheckIn { get; set; }
    
    /// <summary>
    /// تاريخ المغادرة (لحساب السعر)
    /// </summary>
    public DateTime? CheckOut { get; set; }
}