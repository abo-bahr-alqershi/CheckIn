using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.MobileApp.Properties;

/// <summary>
/// استعلام التحقق من توفر كيان لفترة معينة
/// Query to check property availability for a specific period
/// </summary>
public class CheckPropertyAvailabilityQuery : IRequest<ResultDto<PropertyAvailabilityResponse>>
{
    /// <summary>
    /// معرف الكيان
    /// </summary>
    public Guid PropertyId { get; set; }
    
    /// <summary>
    /// تاريخ الوصول
    /// </summary>
    public DateTime CheckInDate { get; set; }
    
    /// <summary>
    /// تاريخ المغادرة
    /// </summary>
    public DateTime CheckOutDate { get; set; }
    
    /// <summary>
    /// عدد الضيوف
    /// </summary>
    public int GuestsCount { get; set; }
}

/// <summary>
/// استجابة توفر الكيان
/// </summary>
public class PropertyAvailabilityResponse
{
    /// <summary>
    /// هل يوجد وحدات متاحة
    /// </summary>
    public bool HasAvailableUnits { get; set; }
    
    /// <summary>
    /// عدد الوحدات المتاحة
    /// </summary>
    public int AvailableUnitsCount { get; set; }
    
    /// <summary>
    /// أقل سعر متاح
    /// </summary>
    public decimal? MinAvailablePrice { get; set; }
    
    /// <summary>
    /// عملة السعر
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// رسالة توضيحية
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// قائمة بأنواع الوحدات المتاحة
    /// </summary>
    public List<string> AvailableUnitTypes { get; set; } = new();
}