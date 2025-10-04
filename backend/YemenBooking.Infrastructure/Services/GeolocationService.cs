using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Interfaces.Services;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using Microsoft.Extensions.Options;
using YemenBooking.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة تحديد الموقع الجغرافي
    /// Geolocation service implementation
    /// </summary>
    public class GeolocationService : IGeolocationService
    {
        private readonly ILogger<GeolocationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly GeolocationSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        public GeolocationService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<GeolocationService> logger,
            HttpClient httpClient,
            IOptions<GeolocationSettings> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _settings = options.Value;
        }

        /// <summary>
        /// الحصول على عنوان IP الخاص بالعميل من HttpContext
        /// Get client IP address from HttpContext
        /// </summary>
        /// <returns></returns>
        public string GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return string.Empty;

            // تحقق من headers مختلفة للحصول على IP الحقيقي
            string ip = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? // Cloudflare
                       httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim() ??
                       httpContext.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                       httpContext.Connection.RemoteIpAddress?.ToString() ??
                       "127.0.0.1";

            // إزالة IPv6 prefix إذا وجد
            if (ip.Contains("::ffff:"))
                ip = ip.Replace("::ffff:", "");

            return ip;
        }

        /// <summary>
        /// الحصول على معلومات الموقع من عنوان IP
        /// Get location info from IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<GeolocationInfo> GetLocationInfoAsync(string ipAddress)
        {
            // التحقق من الـ cache أولاً
            var cacheKey = $"geo_{ipAddress}";
            if (_cache.TryGetValue<GeolocationInfo>(cacheKey, out var cachedInfo))
                return cachedInfo;

            try
            {
                // استخدام خدمات متعددة للـ fallback
                var info = await GetFromIpApiAsync(ipAddress) ??
                          await GetFromIpGeolocationAsync(ipAddress) ??
                          GetDefaultLocationInfo();

                // حفظ في الـ cache لمدة 24 ساعة
                _cache.Set(cacheKey, info, TimeSpan.FromHours(24));

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting geolocation for IP: {IpAddress}", ipAddress);
                return GetDefaultLocationInfo();
            }
        }

        /// <summary>
        /// الحصول على معلومات الموقع من ip-api.com
        /// Get location info from ip-api.com
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private async Task<GeolocationInfo> GetFromIpApiAsync(string ipAddress)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"http://ip-api.com/json/{ipAddress}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    if (data?.status == "success")
                    {
                        return new GeolocationInfo
                        {
                            Country = data.country,
                            CountryCode = data.countryCode,
                            City = data.city,
                            TimeZoneId = data.timezone,
                            Latitude = data.lat,
                            Longitude = data.lon,
                            Region = data.regionName
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get location from ip-api");
            }

            return null;
        }

        /// <summary>
        /// الحصول على معلومات الموقع من ipgeolocation.io
        /// Get location info from ipgeolocation.io
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private async Task<GeolocationInfo> GetFromIpGeolocationAsync(string ipAddress)
        {
            try
            {
                var apiKey = _configuration["Geolocation:IpGeolocationApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    return null;

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(
                    $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ipAddress}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    return new GeolocationInfo
                    {
                        Country = data.country_name,
                        CountryCode = data.country_code2,
                        City = data.city,
                        TimeZoneId = data.time_zone?.name,
                        Latitude = double.Parse(data.latitude?.ToString() ?? "0"),
                        Longitude = double.Parse(data.longitude?.ToString() ?? "0"),
                        Region = data.state_prov
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get location from ipgeolocation");
            }

            return null;
        }

        /// <summary>
        /// الحصول على معلومات الموقع الافتراضية (اليمن/صنعاء)
        /// Get default location info (Yemen/Sana'a)
        /// </summary>
        /// <returns></returns>
        private GeolocationInfo GetDefaultLocationInfo()
        {
            // Default to Yemen/Sana'a
            return new GeolocationInfo
            {
                Country = "Yemen",
                CountryCode = "YE",
                City = "Sana'a",
                TimeZoneId = "Asia/Aden",
                Latitude = 15.3694,
                Longitude = 44.1910,
                Region = "Sana'a"
            };
        }

        /// <summary>
        /// الحصول على الإحداثيات من العنوان
        /// Get coordinates from address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على الإحداثيات من العنوان: {Address}", address);
            try
            {
                var url = $"{_settings.GeocodingApiUrl}/search?format=json&q={Uri.EscapeDataString(address)}&limit=1";
                var response = await _httpClient.GetStringAsync(url);
                var elements = System.Text.Json.JsonSerializer.Deserialize<JsonElement[]>(response);
                if (elements != null && elements.Length > 0)
                {
                    var first = elements[0];
                    var lat = double.Parse(first.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
                    var lon = double.Parse(first.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);
                    return (lat, lon);
                }
                return (0, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على الإحداثيات");
                throw;
            }
        }

        /// <summary>
        ///    الحصول على العنوان من الإحداثيات
        /// Get address from coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على العنوان من الإحداثيات: {Latitude},{Longitude}", latitude, longitude);
            try
            {
                var url = $"{_settings.GeocodingApiUrl}/reverse?format=json&lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}";
                var response = await _httpClient.GetStringAsync(url);
                var doc = JsonDocument.Parse(response);
                if (doc.RootElement.TryGetProperty("display_name", out var name))
                    return name.GetString()!;
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على العنوان");
                throw;
            }
        }

        /// <summary>
        /// حساب المسافة بين نقطتين
        /// Calculate distance between two points
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<double> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("حساب المسافة بين نقطتين: {Lat1},{Lon1} و {Lat2},{Lon2}", lat1, lon1, lat2, lon2);
            double ToRadians(double deg) => deg * Math.PI / 180.0;
            var R = 6371.0; // Earth radius in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Pow(Math.Sin(dLon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;
            return Task.FromResult(distance);
        }

        /// <summary>
        /// العثور على الأماكن القريبة
        /// Find nearby places
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radiusKm"></param>
        /// <param name="placeType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<object>> FindNearbyPlacesAsync(double latitude, double longitude, double radiusKm, string placeType = "all", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("العثور على الأماكن القريبة من: {Latitude},{Longitude} بنصف قطر: {RadiusKm} كم (Type: {PlaceType})", latitude, longitude, radiusKm, placeType);
            try
            {
                var radius = (int)(radiusKm * 1000);
                var filter = placeType.ToLower() == "all" ? "" : $"[amenity={placeType.ToLower()}]";
                var query = $"[out:json];node(around:{radius},{latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}){filter};out;";
                var url = $"https://overpass-api.de/api/interpreter?data={Uri.EscapeDataString(query)}";
                var response = await _httpClient.GetStringAsync(url);
                var doc = JsonDocument.Parse(response);
                var list = new List<object>();
                if (doc.RootElement.TryGetProperty("elements", out var elements))
                {
                    foreach (var el in elements.EnumerateArray())
                    {
                        var id = el.GetProperty("id").GetInt64();
                        var lat = el.GetProperty("lat").GetDouble();
                        var lon = el.GetProperty("lon").GetDouble();
                        var tags = el.TryGetProperty("tags", out var t) ? t : default;
                        list.Add(new { Id = id, Latitude = lat, Longitude = lon, Tags = tags });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء العثور على الأماكن القريبة");
                return Array.Empty<object>();
            }
        }


        /// <summary>
        /// التحقق من صحة الإحداثيات
        /// Validate coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> ValidateCoordinatesAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("التحقق من صحة الإحداثيات: {Latitude},{Longitude}", latitude, longitude);
            var isValid = latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
            return Task.FromResult(isValid);
        }


        /// <summary>
        /// الحصول على معلومات المنطقة الزمنية من الإحداثيات
        /// Get timezone info from coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetTimezoneAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على معلومات المنطقة الزمنية للإحداثيات: {Latitude},{Longitude}", latitude, longitude);
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var url = $"{_settings.TimezoneApiUrl}?key={_settings.ApiKey}&format=json&by=position&lat={latitude.ToString(CultureInfo.InvariantCulture)}&lng={longitude.ToString(CultureInfo.InvariantCulture)}&time={timestamp}";
                var response = await _httpClient.GetStringAsync(url);
                var doc = JsonDocument.Parse(response);
                if (doc.RootElement.TryGetProperty("zoneName", out var zone))
                    return zone.GetString()!;
                if (doc.RootElement.TryGetProperty("zone_name", out var zone2))
                    return zone2.GetString()!;
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على معلومات المنطقة الزمنية");
                throw;
            }
        }
    }
} 