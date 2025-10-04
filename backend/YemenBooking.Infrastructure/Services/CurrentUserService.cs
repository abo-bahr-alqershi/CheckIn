using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using YemenBooking.Core.Entities;
using YemenBooking.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Interfaces.Repositories;
using TimeZoneConverter;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// خدمة المستخدم الحالي التي تستخرج معلومات المستخدم من HttpContext
    /// Implementation of ICurrentUserService that extracts user information from HttpContext
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IGeolocationService _geolocationService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CurrentUserService> _logger;
        /// <summary>
        /// مُنشئ خدمة المستخدم الحالي مع حقن HttpContextAccessor و IUserRepository
        /// Constructor for CurrentUserService with injected HttpContextAccessor and IUserRepository
        /// </summary>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IGeolocationService geolocationService, IMemoryCache cache, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _geolocationService = geolocationService;
            _cache = cache;
            _logger = logger;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        /// <summary>
        /// معرّف المستخدم الحالي
        /// Identifier of the current user
        /// </summary>
        public Guid UserId
        {
            get
            {
                var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
            }
        }

        /// <summary>
        /// اسم المستخدم الحالي
        /// Username of the current user
        /// </summary>
        public string Username => User?.Identity?.Name ?? string.Empty;

        /// <summary>
        /// الدور الخاص بالمستخدم الحالي
        /// Role of the current user
        /// </summary>
        public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        /// <summary>
        /// نوع الحساب الموحّد (Admin, Owner, Staff, Customer)
        /// </summary>
        public string AccountRole => User?.FindFirst("accountRole")?.Value ?? string.Empty;

        /// <summary>
        /// هل المستخدم الحالي مدير
        /// Is the current user an admin
        /// </summary>
        public bool IsAdmin => UserRoles.Contains("Admin");

        /// <summary>
        /// قائمة الأذونات الخاصة بالمستخدم الحالي
        /// Permissions of the current user
        /// </summary>
        public IEnumerable<string> Permissions =>
            User?.FindAll("permission")?.Select(c => c.Value) ?? Enumerable.Empty<string>();

        /// <summary>
        /// قائمة الأدوار الخاصة بالمستخدم الحالي
        /// User roles of the current user
        /// </summary>
        public IEnumerable<string> UserRoles =>
            User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();

        /// <summary>
        /// معرف التتبّع لربط الطلبات
        /// Correlation identifier for tracing
        /// </summary>
        public string CorrelationId => User?.FindFirst("correlationId")?.Value ?? Guid.NewGuid().ToString();

        /// <summary>
        /// معرف الكيان المرتبط بالمستخدم (إن وجد)
        /// Property ID related to the user (if owner or staff)
        /// </summary>
        public Guid? PropertyId
        {
            get
            {
                var propIdClaim = User?.FindFirst("propertyId")?.Value;
                return Guid.TryParse(propIdClaim, out var pid) ? pid : (Guid?)null;
            }
        }

        /// <summary>
        /// اسم الكيان المرتبط بالمستخدم (إن وجد)
        /// Property name related to the user (if owner or staff)
        /// </summary>
        public string? PropertyName => User?.FindFirst("propertyName")?.Value;

        /// <summary>
        /// عملة العقار المرتبط بالمستخدم (إن وجدت)
        /// </summary>
        public string? PropertyCurrency => User?.FindFirst("propertyCurrency")?.Value;

        /// <summary>
        /// معرف موظف الكيان المرتبط بالمستخدم (إن وجد)
        /// Property ID related to the user (if owner or staff)
        /// </summary>
        public Guid? StaffId
        {
            get
            {
                var staffIdClaim = User?.FindFirst("staffId")?.Value;
                return Guid.TryParse(staffIdClaim, out var sid) ? sid : (Guid?)null;
            }
        }

        /// <summary>
        /// التحقق مما إذا كان المستخدم الحالي موظفاً في الكيان المحدد
        /// Checks if the current user is staff of the specified property
        /// </summary>
        public bool IsStaffInProperty(Guid propertyId)
        {
            var userPropertyId = PropertyId;
            return userPropertyId.HasValue && userPropertyId.Value == propertyId &&
                   (UserRoles.Contains("Staff"));
        }

        /// <summary>
        /// جلب بيانات المستخدم الحالي من قاعدة البيانات بناءً على المعرف من HttpContext
        /// </summary>
        public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            // التحقق من وجود هوية مصدقة
            if (User == null || UserId == Guid.Empty)
                throw new UnauthorizedAccessException("المستخدم غير مصدق عليه");

            // جلب الكيان من قاعدة البيانات
            var user = await _userRepository.GetUserByIdAsync(UserId, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException($"المستخدم بالمعرف {UserId} غير موجود");

            return user;
        }

        public Task<bool> IsInRoleAsync(string role)
        {
            var hasRole = UserRoles != null && UserRoles.Contains(role);
            return Task.FromResult(hasRole);
        }

                public async Task<UserLocationInfo> GetUserLocationAsync()
        {
            var cacheKey = $"user_location_{UserId}";
            
            // التحقق من الـ cache
            if (_cache.TryGetValue<UserLocationInfo>(cacheKey, out var cachedLocation))
                return cachedLocation;

            try
            {
                // 1. محاولة الحصول على المنطقة الزمنية من profile المستخدم
                var user = await GetCurrentUserAsync();
                var timeZoneId = user?.TimeZoneId;

                // 2. إذا لم يكن محفوظاً، استخدم IP geolocation
                if (string.IsNullOrEmpty(timeZoneId))
                {
                    var ipAddress = _geolocationService.GetClientIpAddress();
                    var geoInfo = await _geolocationService.GetLocationInfoAsync(ipAddress);
                    timeZoneId = geoInfo?.TimeZoneId;

                    // حفظ المنطقة الزمنية في profile المستخدم للمرات القادمة
                    if (!string.IsNullOrEmpty(timeZoneId) && user != null)
                    {
                        user.TimeZoneId = timeZoneId;
                        user.Country = geoInfo.Country;
                        user.City = geoInfo.City;
                        await _userRepository.UpdateAsync(user);
                    }

                    var locationInfo = CreateLocationInfo(timeZoneId, geoInfo);
                    
                    // Cache لمدة ساعة
                    _cache.Set(cacheKey, locationInfo, TimeSpan.FromHours(1));
                    
                    return locationInfo;
                }

                var cachedInfo = CreateLocationInfo(timeZoneId, null);
                _cache.Set(cacheKey, cachedInfo, TimeSpan.FromHours(1));
                
                return cachedInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user location");
                return GetDefaultLocation();
            }
        }

        public async Task<DateTime> ConvertFromUtcToUserLocalAsync(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            var timeZoneId = await GetUserTimeZoneIdAsync();
            
            try
            {
                // استخدام TimeZoneConverter للتعامل مع IANA و Windows time zones
                var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting from UTC to local time. TimeZone: {TimeZone}", timeZoneId);
                // Fallback to Yemen time
                var yemenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, yemenTimeZone);
            }
        }

        public async Task<DateTime> ConvertFromUserLocalToUtcAsync(DateTime localDateTime)
        {
            var timeZoneId = await GetUserTimeZoneIdAsync();
            
            try
            {
                // التأكد من أن التوقيت محلي
                if (localDateTime.Kind == DateTimeKind.Utc)
                    return localDateTime;

                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
                
                var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting from local to UTC time. TimeZone: {TimeZone}", timeZoneId);
                // Fallback
                var yemenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, yemenTimeZone);
            }
        }

        public async Task<string> GetUserTimeZoneIdAsync()
        {
            var location = await GetUserLocationAsync();
            return location?.TimeZoneId ?? "Asia/Aden";
        }

        public async Task<TimeSpan> GetUserTimeZoneOffsetAsync()
        {
            var location = await GetUserLocationAsync();
            return location?.UtcOffset ?? TimeSpan.FromHours(3);
        }

        private UserLocationInfo CreateLocationInfo(string timeZoneId, GeolocationInfo geoInfo)
        {
            try
            {
                var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId ?? "Asia/Aden");
                var now = DateTime.UtcNow;
                var offset = timeZone.GetUtcOffset(now);
                
                return new UserLocationInfo
                {
                    Country = geoInfo?.Country ?? "Yemen",
                    CountryCode = geoInfo?.CountryCode ?? "YE",
                    City = geoInfo?.City ?? "Sana'a",
                    TimeZoneId = timeZoneId ?? "Asia/Aden",
                    UtcOffset = offset,
                    TimeZoneName = timeZone.DisplayName,
                    IsDaylightSaving = timeZone.IsDaylightSavingTime(now)
                };
            }
            catch
            {
                return GetDefaultLocation();
            }
        }

        private UserLocationInfo GetDefaultLocation()
        {
            return new UserLocationInfo
            {
                Country = "Yemen",
                CountryCode = "YE",
                City = "Sana'a",
                TimeZoneId = "Asia/Aden",
                UtcOffset = TimeSpan.FromHours(3),
                TimeZoneName = "(UTC+03:00) Yemen Time",
                IsDaylightSaving = false
            };
        }
    }
} 