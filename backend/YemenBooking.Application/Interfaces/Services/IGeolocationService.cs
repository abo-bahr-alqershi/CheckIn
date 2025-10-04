namespace YemenBooking.Application.Interfaces.Services;

/// <summary>
/// واجهة خدمة تحديد الموقع الجغرافي
/// Geolocation service interface
/// </summary>
public interface IGeolocationService
{

    /// <summary>
    /// الحصول على معلومات الموقع من عنوان IP
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    Task<GeolocationInfo> GetLocationInfoAsync(string ipAddress);

    /// <summary>
    /// الحصول على عنوان IP الخاص بالعميل من HttpContext
    /// </summary>
    /// <returns></returns>
    string GetClientIpAddress();

    /// <summary>
    /// الحصول على الإحداثيات من العنوان
    /// Get coordinates from address
    /// </summary>
    Task<(double Latitude, double Longitude)> GetCoordinatesAsync(
        string address, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على العنوان من الإحداثيات
    /// Get address from coordinates
    /// </summary>
    Task<string> GetAddressAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حساب المسافة بين نقطتين
    /// Calculate distance between two points
    /// </summary>
    Task<double> CalculateDistanceAsync(
        double lat1, 
        double lon1, 
        double lat2, 
        double lon2, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// العثور على الأماكن القريبة
    /// Find nearby places
    /// </summary>
    Task<IEnumerable<object>> FindNearbyPlacesAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        string placeType = "all", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من صحة الإحداثيات
    /// Validate coordinates
    /// </summary>
    Task<bool> ValidateCoordinatesAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على معلومات المنطقة الزمنية
    /// Get timezone information
    /// </summary>
    Task<string> GetTimezoneAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default);
}

public class GeolocationInfo
{
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string City { get; set; }
    public string TimeZoneId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Region { get; set; }
}