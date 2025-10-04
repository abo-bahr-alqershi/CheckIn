using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace BookN.Core.Helpers
{
    /// <summary>
    /// مجموعة الدوال المساعدة المستخدمة في النظام
    /// تحتوي على جميع الدوال المرجعية في الخدمات المختلفة
    /// </summary>
    public static class IndexingHelpers
    {
        #region Property Related Helpers
        // الملف: 04-خدمة_إنشاء_الفهارس.cs - IndexGenerationService

        /// <summary>
        /// حساب إحصائيات المدينة من قائمة الكيانات
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateCityStatistics
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>إحصائيات المدينة</returns>
        public static CityStatistics CalculateCityStatistics(List<Properties> properties)
        {
            var prices = properties.SelectMany(p => p.Units)
                .Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                .Select(u => decimal.TryParse(u.BasePrice_Amount, out var price) ? price : 0)
                .Where(price => price > 0)
                .ToList();

            var reviews = properties.SelectMany(p => p.Reviews)
                .Where(r => r != null && r.IsDeleted == false)
                .ToList();

            return new CityStatistics
            {
                AveragePrice = prices.Any() ? prices.Average() : 0,
                MinPrice = prices.Any() ? prices.Min() : 0,
                MaxPrice = prices.Any() ? prices.Max() : 0,
                PopularAmenities = GetPopularAmenities(properties),
                AverageRating = reviews.Any() ? (double)reviews.Average(r => r.AverageRating ?? 0) : 0,
                TotalReviews = reviews.Count,
                OccupancyRate = CalculateOccupancyRate(properties),
                AverageStayDuration = CalculateAverageStayDuration(properties),
                PopularDistricts = GetPopularDistricts(properties),
                SeasonalStats = CalculateSeasonalStatistics(properties)
            };
        }

        /// <summary>
        /// تجميع الكيانات حسب نطاقات الأسعار
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GroupPropertiesByPriceRanges
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>توزيع نطاقات الأسعار</returns>
        public static Dictionary<string, PriceRangeData> GroupPropertiesByPriceRanges(List<Properties> properties)
        {
            var ranges = new Dictionary<string, PriceRangeData>();
            var priceRangeDefinitions = new[]
            {
                new { Key = "0-100", Min = 0m, Max = 100m },
                new { Key = "100-300", Min = 100m, Max = 300m },
                new { Key = "300-500", Min = 300m, Max = 500m },
                new { Key = "500-1000", Min = 500m, Max = 1000m },
                new { Key = "1000+", Min = 1000m, Max = decimal.MaxValue }
            };

            foreach (var rangeDef in priceRangeDefinitions)
            {
                var propertiesInRange = properties.Where(p =>
                {
                    var prices = p.Units?.Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                        .Select(u => decimal.TryParse(u.BasePrice_Amount, out var price) ? price : 0)
                        .Where(price => price > 0) ?? Enumerable.Empty<decimal>();

                    return prices.Any(price =>
                        price >= rangeDef.Min &&
                        (rangeDef.Max == decimal.MaxValue || price < rangeDef.Max));
                }).ToList();

                if (propertiesInRange.Any())
                {
                    var avgPrice = propertiesInRange.SelectMany(p => p.Units)
                        .Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                        .Select(u => decimal.TryParse(u.BasePrice_Amount, out var price) ? price : 0)
                        .Where(price => price > 0)
                        .DefaultIfEmpty(0)
                        .Average();

                    ranges[rangeDef.Key] = new PriceRangeData
                    {
                        Count = propertiesInRange.Count,
                        PropertyIds = propertiesInRange.Take(100)
                            .Select(p => int.TryParse(p.PropertyId, out var id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList(),
                        AveragePrice = avgPrice,
                        AverageRating = CalculateAverageRatingForProperties(propertiesInRange),
                        OccupancyRate = CalculateOccupancyRate(propertiesInRange),
                        CityDistribution = propertiesInRange.GroupBy(p => p.City ?? "Unknown")
                            .Where(g => !string.IsNullOrEmpty(g.Key) && g.Key != "Unknown")
                            .ToDictionary(g => g.Key, g => g.Count()),
                        PropertyTypeDistribution = GroupPropertiesByTypeIds(propertiesInRange)
                    };
                }
            }

            return ranges;
        }

        /// <summary>
        /// تجميع الكيانات حسب المرافق
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GroupPropertiesByAmenities
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>توزيع المرافق</returns>
        public static Dictionary<string, AmenityData> GroupPropertiesByAmenities(List<Properties> properties)
        {
            return properties
                .SelectMany(p => p.PropertyAmenities ?? Enumerable.Empty<PropertyAmenities>())
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .Where(pa => pa.PropertyTypeAmenity?.Amenity != null)
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name ?? "Unknown")
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Key != "Unknown")
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => new AmenityData
                    {
                        Count = g.Count(),
                        PropertyIds = g.Select(pa => int.TryParse(pa.PropertyId, out var id) ? id : 0)
                            .Where(id => id > 0)
                            .Distinct()
                            .Take(100)
                            .ToList(),
                        AveragePrice = CalculateAveragePriceForAmenity(g.ToList()),
                        AverageRating = CalculateAverageRatingForAmenity(g.ToList()),
                        Popularity = CalculateAmenityPopularity(g.Count(), properties.Count),
                        PopularWith = GetAmenityPopularCategories(g.ToList())
                    });
        }

        /// <summary>
        /// تجميع الكيانات حسب الأنواع
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GroupPropertiesByType
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>توزيع أنواع الكيانات</returns>
        public static Dictionary<string, PropertyTypeData> GroupPropertiesByType(List<Properties> properties)
        {
            return properties
                .Where(p => !string.IsNullOrEmpty(p.TypeId))
                .GroupBy(p => GetPropertyTypeName(p.TypeId))
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => new PropertyTypeData
                    {
                        Count = g.Count(),
                        AveragePrice = CalculateAveragePrice(g.ToList()),
                        AverageRating = CalculateAverageRatingForProperties(g.ToList()),
                        PropertyIds = g.Take(100)
                            .Select(p => int.TryParse(p.PropertyId, out var id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList(),
                        DefaultCapacity = CalculateDefaultCapacity(g.ToList()),
                        CommonAmenities = GetCommonAmenitiesForType(g.ToList())
                    });
        }

        /// <summary>
        /// تجميع الكيانات حسب المدن
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GroupPropertiesByCity
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>توزيع المدن</returns>
        public static Dictionary<string, CityPriceData> GroupPropertiesByCity(List<Properties> properties)
        {
            return properties
                .Where(p => !string.IsNullOrEmpty(p.City))
                .GroupBy(p => p.City!)
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => new CityPriceData
                    {
                        Count = g.Count(),
                        AveragePrice = CalculateAveragePrice(g.ToList()),
                        PropertyIds = g.Take(100)
                            .Select(p => int.TryParse(p.PropertyId, out var id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList()
                    });
        }

        #endregion

        #region Price Calculation Helpers

        /// <summary>
        /// حساب متوسط الأسعار للكيانات
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateAveragePrice
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>متوسط السعر</returns>
        public static decimal CalculateAveragePrice(List<Properties> properties)
        {
            var prices = properties.SelectMany(p => p.Units ?? Enumerable.Empty<Units>())
                .Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                .Select(u => decimal.TryParse(u.BasePrice_Amount, out var price) ? price : 0)
                .Where(price => price > 0)
                .ToList();

            return prices.Any() ? prices.Average() : 0;
        }

        /// <summary>
        /// الحصول على نطاقات الأسعار المتأثرة
        /// الموقع: 06-خدمة_التحديث_التلقائي.cs - في IndexAutoUpdateService.GetAffectedPriceRanges
        /// </summary>
        /// <param name="minPrice">الحد الأدنى للسعر</param>
        /// <param name="maxPrice">الحد الأقصى للسعر</param>
        /// <returns>قائمة نطاقات الأسعار المتأثرة</returns>
        public static List<(string Key, decimal Min, decimal Max)> GetAffectedPriceRanges(decimal? minPrice, decimal? maxPrice)
        {
            var ranges = new List<(string Key, decimal Min, decimal Max)>();
            var price = minPrice ?? maxPrice ?? 0;

            var allRanges = new[]
            {
                ("0-100", 0m, 100m),
                ("100-300", 100m, 300m),
                ("300-500", 300m, 500m),
                ("500-1000", 500m, 1000m),
                ("1000+", 1000m, decimal.MaxValue)
            };

            foreach (var range in allRanges)
            {
                if (price >= range.Item2 && (range.Item3 == decimal.MaxValue || price < range.Item3))
                {
                    ranges.Add(range);
                }
            }

            return ranges.Any() ? ranges : new List<(string, decimal, decimal)> { ("100-300", 100m, 300m) };
        }

        /// <summary>
        /// حساب التسعير الموسمي
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateSeasonalPricing
        /// </summary>
        /// <param name="units">قائمة الوحدات</param>
        /// <returns>بيانات التسعير الموسمي</returns>
        public static SeasonalPricingData CalculateSeasonalPricing(List<Units> units)
        {
            return new SeasonalPricingData
            {
                Peak = new SeasonPriceData
                {
                    Months = new List<string> { "12", "01", "02" },
                    AverageChange = 25.5,
                    AffectedProperties = units.Count * 80 / 100 // تقدير 80% من الكيانات متأثرة
                },
                Off = new SeasonPriceData
                {
                    Months = new List<string> { "06", "07", "08" },
                    AverageChange = -15.2,
                    AffectedProperties = units.Count * 90 / 100 // تقدير 90% من الكيانات متأثرة
                },
                Regular = new SeasonPriceData
                {
                    Months = new List<string> { "03", "04", "05", "09", "10", "11" },
                    AverageChange = 0,
                    AffectedProperties = units.Count
                }
            };
        }

        #endregion

        #region Rating and Review Helpers

        /// <summary>
        /// حساب متوسط التقييمات للكيانات
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateAverageRatingForProperties
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>متوسط التقييم</returns>
        public static double CalculateAverageRatingForProperties(List<Properties> properties)
        {
            var ratings = properties
                .SelectMany(p => p.Reviews ?? Enumerable.Empty<Reviews>())
                .Where(r => r.IsDeleted == false && r.AverageRating.HasValue)
                .Select(r => (double)r.AverageRating.Value)
                .ToList();

            return ratings.Any() ? ratings.Average() : 0;
        }

        /// <summary>
        /// تجميع الكيانات حسب التقييم النجمي
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GroupPropertiesByStarRating
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>توزيع التقييمات النجمية</returns>
        public static Dictionary<string, StarRatingData> GroupPropertiesByStarRating(List<Properties> properties)
        {
            return properties
                .Where(p => p.StarRating.HasValue && p.StarRating > 0)
                .GroupBy(p => $"{p.StarRating}-star")
                .ToDictionary(
                    g => g.Key,
                    g => new StarRatingData
                    {
                        Count = g.Count(),
                        AveragePrice = CalculateAveragePrice(g.ToList()),
                        AverageRating = CalculateAverageRatingForProperties(g.ToList()),
                        PropertyIds = g.Take(100)
                            .Select(p => int.TryParse(p.PropertyId, out var id) ? id : 0)
                            .Where(id => id > 0)
                            .ToList()
                    });
        }

        #endregion

        #region Amenity Helpers

        /// <summary>
        /// الحصول على المرافق الشائعة للكيانات
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GetPopularAmenities
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>قائمة المرافق الشائعة</returns>
        public static List<string> GetPopularAmenities(List<Properties> properties)
        {
            return properties
                .SelectMany(p => p.PropertyAmenities ?? Enumerable.Empty<PropertyAmenities>())
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .Where(pa => pa.PropertyTypeAmenity?.Amenity?.Name != null)
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name!)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key.ToLowerInvariant())
                .ToList();
        }

        /// <summary>
        /// حساب المرافق المرتبطة لمرفق معين
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateRelatedAmenitiesAsync
        /// </summary>
        /// <param name="amenityId">معرف المرفق</param>
        /// <param name="properties">الكيانات التي تحتوي على المرفق</param>
        /// <returns>قائمة المرافق المرتبطة</returns>
        public static async Task<Dictionary<string, RelatedAmenityData>> CalculateRelatedAmenitiesAsync(string amenityId, List<Properties> properties)
        {
            var relatedAmenities = new Dictionary<string, RelatedAmenityData>();

            // الحصول على جميع المرافق الأخرى في نفس الكيانات
            var otherAmenities = properties
                .SelectMany(p => p.PropertyAmenities ?? Enumerable.Empty<PropertyAmenities>())
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .Where(pa => pa.PropertyTypeAmenity?.AmenityId != amenityId)
                .Where(pa => pa.PropertyTypeAmenity?.Amenity?.Name != null)
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name!)
                .ToList();

            foreach (var amenityGroup in otherAmenities)
            {
                var commonProperties = amenityGroup
                    .Select(pa => pa.PropertyId)
                    .Intersect(properties.Select(p => p.PropertyId))
                    .ToList();

                if (commonProperties.Any())
                {
                    var correlation = (double)commonProperties.Count / properties.Count;
                    if (correlation >= 0.1) // عتبة الارتباط 10%
                    {
                        relatedAmenities[amenityGroup.Key.ToLowerInvariant()] = new RelatedAmenityData
                        {
                            Count = commonProperties.Count,
                            Correlation = correlation,
                            PropertyIds = commonProperties
                                .Select(id => int.TryParse(id, out var intId) ? intId : 0)
                                .Where(id => id > 0)
                                .Take(100)
                                .ToList()
                        };
                    }
                }
            }

            return relatedAmenities;
        }

        /// <summary>
        /// حساب المرافق الشائعة لنوع كيان معين
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateCommonAmenitiesAsync
        /// </summary>
        /// <param name="properties">الكيانات من نفس النوع</param>
        /// <returns>المرافق الشائعة مع نسبة التوفر</returns>
        public static async Task<Dictionary<string, AmenityAvailabilityData>> CalculateCommonAmenitiesAsync(List<Properties> properties)
        {
            var totalProperties = properties.Count;
            if (totalProperties == 0) return new Dictionary<string, AmenityAvailabilityData>();

            var amenityGroups = properties
                .SelectMany(p => p.PropertyAmenities ?? Enumerable.Empty<PropertyAmenities>())
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .Where(pa => pa.PropertyTypeAmenity?.Amenity?.Name != null)
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name!)
                .ToList();

            var commonAmenities = new Dictionary<string, AmenityAvailabilityData>();

            foreach (var group in amenityGroups)
            {
                var propertiesWithAmenity = group
                    .Select(pa => pa.PropertyId)
                    .Distinct()
                    .ToList();

                var availability = (double)propertiesWithAmenity.Count / totalProperties;

                if (availability >= 0.1) // 10% على الأقل من الكيانات
                {
                    commonAmenities[group.Key.ToLowerInvariant()] = new AmenityAvailabilityData
                    {
                        Availability = availability,
                        Count = propertiesWithAmenity.Count,
                        TopProperties = propertiesWithAmenity
                            .Select(id => int.TryParse(id, out var intId) ? intId : 0)
                            .Where(id => id > 0)
                            .Take(10)
                            .ToList()
                    };
                }
            }

            return commonAmenities;
        }

        #endregion

        #region Availability Helpers

        /// <summary>
        /// حساب معدل الإشغال للكيانات
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateOccupancyRate
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>معدل الإشغال</returns>
        public static double CalculateOccupancyRate(List<Properties> properties)
        {
            var currentDate = DateTime.UtcNow;
            var totalUnits = properties.SelectMany(p => p.Units ?? Enumerable.Empty<Units>()).Count();
            
            if (totalUnits == 0) return 0;

            var bookedUnits = properties
                .SelectMany(p => p.Units ?? Enumerable.Empty<Units>())
                .Count(u => (u.UnitAvailabilities ?? Enumerable.Empty<UnitAvailability>()).Any(ua =>
                    ua.Status == "Booked" &&
                    ua.IsDeleted == false &&
                    ua.StartDate <= currentDate &&
                    ua.EndDate >= currentDate));

            return (double)bookedUnits / totalUnits;
        }

        /// <summary>
        /// حساب التوفر الموسمي
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateSeasonalAvailability
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <returns>بيانات التوفر الموسمي</returns>
        public static SeasonalAvailabilityData CalculateSeasonalAvailability(List<Properties> properties)
        {
            return new SeasonalAvailabilityData
            {
                Summer = new SeasonAvailabilityInfo { AvailabilityRate = 0.75, PopularityBoost = 1.5 },
                Winter = new SeasonAvailabilityInfo { AvailabilityRate = 0.85, PopularityBoost = 1.2 },
                Spring = new SeasonAvailabilityInfo { AvailabilityRate = 0.90, PopularityBoost = 1.1 },
                Autumn = new SeasonAvailabilityInfo { AvailabilityRate = 0.88, PopularityBoost = 1.0 }
            };
        }

        /// <summary>
        /// حساب التوفر اليومي
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateDailyAvailability
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <param name="startDate">تاريخ البداية</param>
        /// <param name="endDate">تاريخ النهاية</param>
        /// <returns>توفر يومي</returns>
        public static Dictionary<string, DailyAvailabilityData> CalculateDailyAvailability(
            List<Properties> properties, DateTime startDate, DateTime endDate)
        {
            var dailyAvailability = new Dictionary<string, DailyAvailabilityData>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var availableProperties = properties.Where(p =>
                    (p.Units ?? Enumerable.Empty<Units>()).Any(u =>
                        !(u.UnitAvailabilities ?? Enumerable.Empty<UnitAvailability>()).Any(ua =>
                            ua.Status == "Booked" &&
                            ua.IsDeleted == false &&
                            ua.StartDate <= currentDate &&
                            ua.EndDate >= currentDate))).ToList();

                var availableUnits = availableProperties
                    .SelectMany(p => p.Units ?? Enumerable.Empty<Units>())
                    .Count(u => !(u.UnitAvailabilities ?? Enumerable.Empty<UnitAvailability>()).Any(ua =>
                        ua.Status == "Booked" &&
                        ua.IsDeleted == false &&
                        ua.StartDate <= currentDate &&
                        ua.EndDate >= currentDate));

                dailyAvailability[currentDate.ToString("yyyy-MM-dd")] = new DailyAvailabilityData
                {
                    AvailableProperties = availableProperties.Count,
                    AvailableUnits = availableUnits,
                    AveragePrice = CalculateAveragePrice(availableProperties),
                    PropertyIds = availableProperties.Take(20)
                        .Select(p => int.TryParse(p.PropertyId, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .ToList()
                };

                currentDate = currentDate.AddDays(1);
            }

            return dailyAvailability;
        }

        /// <summary>
        /// حساب توفر نهايات الأسبوع
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.CalculateWeekendAvailability
        /// </summary>
        /// <param name="properties">قائمة الكيانات</param>
        /// <param name="startDate">تاريخ البداية</param>
        /// <param name="endDate">تاريخ النهاية</param>
        /// <returns>بيانات توفر نهايات الأسبوع</returns>
        public static WeekendAvailabilityData CalculateWeekendAvailability(
            List<Properties> properties, DateTime startDate, DateTime endDate)
        {
            var weekendDays = new List<DateTime>();
            var weekdays = new List<DateTime>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek == DayOfWeek.Friday || currentDate.DayOfWeek == DayOfWeek.Saturday)
                    weekendDays.Add(currentDate);
                else
                    weekdays.Add(currentDate);

                currentDate = currentDate.AddDays(1);
            }

            var weekendAvailable = weekendDays.Count > 0 ? 
                weekendDays.Average(date => GetAvailablePropertiesForDate(properties, date)) : 0;
            var weekdaysAvailable = weekdays.Count > 0 ? 
                weekdays.Average(date => GetAvailablePropertiesForDate(properties, date)) : 0;

            return new WeekendAvailabilityData
            {
                Weekends = new WeekendData
                {
                    AverageAvailable = (int)weekendAvailable,
                    PriceChange = 1.25, // زيادة 25% في نهايات الأسبوع
                    PopularCities = new List<string> { "sanaa", "taiz", "adan" }
                },
                Weekdays = new WeekendData
                {
                    AverageAvailable = (int)weekdaysAvailable,
                    PriceChange = 0.85, // خصم 15% في أيام الأسبوع
                    PopularCities = new List<string> { "dammam", "khobar", "abha" }
                }
            };
        }

        /// <summary>
        /// الحصول على الأشهر المتأثرة بفترة زمنية معينة
        /// الموقع: 06-خدمة_التحديث_التلقائي.cs - في IndexAutoUpdateService.GetAffectedMonths
        /// </summary>
        /// <param name="startDate">تاريخ البداية</param>
        /// <param name="endDate">تاريخ النهاية</param>
        /// <returns>قائمة الأشهر المتأثرة</returns>
        public static List<string> GetAffectedMonths(DateTime startDate, DateTime endDate)
        {
            var months = new List<string>();
            var current = new DateTime(startDate.Year, startDate.Month, 1);
            var end = new DateTime(endDate.Year, endDate.Month, 1);

            while (current <= end)
            {
                months.Add($"{current.Year}-{current.Month:D2}");
                current = current.AddMonths(1);
            }

            return months;
        }

        #endregion

        #region Text Search Helpers

        /// <summary>
        /// استخراج الكلمات القابلة للبحث من النص
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.ExtractSearchableWords
        /// </summary>
        /// <param name="text">النص المراد تحليله</param>
        /// <returns>قائمة الكلمات</returns>
        public static List<string> ExtractSearchableWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            // تنظيف النص وتقسيمه
            var words = text.ToLowerInvariant()
                .Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?', '-', '_' },
                       StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length >= 2) // كلمات بحرفين على الأقل
                .Where(w => !IsStopWord(w)) // إزالة كلمات التوقف
                .Distinct()
                .ToList();

            return words;
        }

        /// <summary>
        /// إضافة كلمة إلى هيكل Trie
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.AddWordToTrie
        /// </summary>
        /// <param name="trie">هيكل Trie</param>
        /// <param name="word">الكلمة</param>
        /// <param name="propertyId">معرف الكيان</param>
        public static void AddWordToTrie(Dictionary<string, object> trie, string word, int propertyId)
        {
            var current = trie;
            
            foreach (char c in word)
            {
                string key = c.ToString();
                if (!current.ContainsKey(key))
                {
                    current[key] = new Dictionary<string, object>();
                }
                current = (Dictionary<string, object>)current[key];
            }

            // إضافة معرف الكيان في نهاية الكلمة
            if (!current.ContainsKey("propertyIds"))
            {
                current["propertyIds"] = new List<int>();
            }
            
            var propertyIds = (List<int>)current["propertyIds"];
            if (!propertyIds.Contains(propertyId))
            {
                propertyIds.Add(propertyId);
            }

            current["isComplete"] = true;
        }

        /// <summary>
        /// تحديث المصطلحات الشائعة
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.UpdatePopularTerms
        /// </summary>
        /// <param name="popularTerms">قاموس المصطلحات الشائعة</param>
        /// <param name="word">الكلمة</param>
        /// <param name="propertyId">معرف الكيان</param>
        public static void UpdatePopularTerms(Dictionary<string, TextSearchTerm> popularTerms, string word, int propertyId)
        {
            if (!popularTerms.ContainsKey(word))
            {
                popularTerms[word] = new TextSearchTerm
                {
                    Term = word,
                    Frequency = 0,
                    PropertyIds = new List<int>(),
                    Weight = CalculateWordWeight(word),
                    Category = DetermineWordCategory(word)
                };
            }

            var term = popularTerms[word];
            term.Frequency++;
            
            if (!term.PropertyIds.Contains(propertyId))
            {
                term.PropertyIds.Add(propertyId);
            }
        }

        /// <summary>
        /// البحث في هيكل Trie
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.SearchInTrie
        /// </summary>
        /// <param name="trie">هيكل Trie</param>
        /// <param name="searchTerm">المصطلح المراد البحث عنه</param>
        /// <returns>نتائج البحث</returns>
        public static List<TrieSearchResult> SearchInTrie(Dictionary<string, object> trie, string searchTerm)
        {
            var results = new List<TrieSearchResult>();
            var current = trie;

            // البحث الدقيق
            foreach (char c in searchTerm.ToLowerInvariant())
            {
                string key = c.ToString();
                if (!current.ContainsKey(key))
                {
                    return results; // لم يتم العثور على المصطلح
                }
                current = (Dictionary<string, object>)current[key];
            }

            // التحقق من وجود نتائج
            if (current.ContainsKey("propertyIds") && current.ContainsKey("isComplete"))
            {
                var propertyIds = (List<int>)current["propertyIds"];
                results.Add(new TrieSearchResult
                {
                    Word = searchTerm,
                    PropertyIds = propertyIds,
                    IsExactMatch = true
                });
            }

            // البحث عن التكملات المحتملة
            var completions = GetTrieCompletions(current, searchTerm, 10);
            results.AddRange(completions);

            return results;
        }

        /// <summary>
        /// الحصول على تكملات من Trie
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetTrieCompletions
        /// </summary>
        /// <param name="trie">عقدة Trie الحالية</param>
        /// <param name="prefix">البادئة</param>
        /// <param name="maxResults">العدد الأقصى للنتائج</param>
        /// <returns>قائمة التكملات</returns>
        public static List<string> GetTrieCompletions(Dictionary<string, object> trie, string prefix, int maxResults = 10)
        {
            var completions = new List<string>();
            var current = trie;

            // الانتقال إلى البادئة
            foreach (char c in prefix.ToLowerInvariant())
            {
                string key = c.ToString();
                if (!current.ContainsKey(key))
                {
                    return completions;
                }
                current = (Dictionary<string, object>)current[key];
            }

            // البحث عن التكملات
            FindCompletionsRecursive(current, prefix, completions, maxResults);
            return completions;
        }

        /// <summary>
        /// إضافة المرادفات الشائعة
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.AddCommonSynonyms
        /// </summary>
        /// <param name="synonyms">قاموس المرادفات</param>
        public static void AddCommonSynonyms(Dictionary<string, List<string>> synonyms)
        {
            var commonSynonyms = new Dictionary<string, List<string>>
            {
                ["فندق"] = new List<string> { "نزل", "بيت ضيافة", "لوكاندة", "موتيل" },
                ["شقة"] = new List<string> { "وحدة سكنية", "استوديو", "دوبلكس" },
                ["فيلا"] = new List<string> { "بيت", "منزل", "قصر", "شاليه" },
                ["منتجع"] = new List<string> { "ريزورت", "مجمع سياحي", "قرية سياحية" },
                ["مسبح"] = new List<string> { "حمام سباحة", "بركة سباحة" },
                ["موقف"] = new List<string> { "جراج", "مرآب", "باركنج" },
                ["واي فاي"] = new List<string> { "انترنت", "wifi", "شبكة" },
                ["مطعم"] = new List<string> { "مقهى", "كافيه", "بوفيه" }
            };

            foreach (var synonym in commonSynonyms)
            {
                synonyms[synonym.Key] = synonym.Value;
            }
        }

        #endregion

        #region Utility Helpers

        /// <summary>
        /// الحصول على اسم نوع الكيان من معرفه
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService.GetPropertyTypeName
        /// </summary>
        /// <param name="typeId">معرف النوع</param>
        /// <returns>اسم النوع</returns>
        public static string GetPropertyTypeName(string typeId)
        {
            // يجب تنفيذ هذا باستخدام قاعدة البيانات أو كاش
            // هذا تنفيذ مؤقت للتوضيح
            var typeNames = new Dictionary<string, string>
            {
                ["hotel-type-001"] = "Hotel",
                ["apartment-type-001"] = "Apartment", 
                ["villa-type-001"] = "Villa",
                ["resort-type-001"] = "Resort"
            };

            return typeNames.TryGetValue(typeId, out var name) ? name : "Unknown";
        }

        /// <summary>
        /// الحصول على اسم المرفق من معرفه
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetAmenityNameByIdAsync
        /// </summary>
        /// <param name="amenityId">معرف المرفق</param>
        /// <returns>اسم المرفق</returns>
        public static async Task<string?> GetAmenityNameByIdAsync(string amenityId)
        {
            // يجب تنفيذ هذا باستخدام قاعدة البيانات أو كاش
            // هذا تنفيذ مؤقت للتوضيح
            var amenityNames = new Dictionary<string, string>
            {
                ["wifi-001"] = "wifi",
                ["pool-001"] = "pool",
                ["parking-001"] = "parking",
                ["gym-001"] = "gym"
            };

            return amenityNames.TryGetValue(amenityId, out var name) ? name : null;
        }

        /// <summary>
        /// الحصول على اسم نوع الكيان من معرفه
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetPropertyTypeNameByIdAsync
        /// </summary>
        /// <param name="propertyTypeId">معرف نوع الكيان</param>
        /// <returns>اسم نوع الكيان</returns>
        public static async Task<string?> GetPropertyTypeNameByIdAsync(string propertyTypeId)
        {
            return GetPropertyTypeName(propertyTypeId);
        }

        /// <summary>
        /// الحصول على جميع معرفات الكيانات النشطة
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetAllActivePropertyIdsAsync
        /// </summary>
        /// <returns>قائمة معرفات الكيانات النشطة</returns>
        public static async Task<List<int>> GetAllActivePropertyIdsAsync()
        {
            // يجب تنفيذ هذا باستخدام قاعدة البيانات
            // هذا تنفيذ مؤقت للتوضيح
            return Enumerable.Range(1, 1000).ToList();
        }

        /// <summary>
        /// تحميل الكيانات مع معلومات الموقع
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.LoadPropertiesWithLocationAsync
        /// </summary>
        /// <param name="propertyIds">معرفات الكيانات</param>
        /// <returns>الكيانات مع معلومات الموقع</returns>
        public static async Task<List<PropertyWithLocation>> LoadPropertiesWithLocationAsync(List<int> propertyIds)
        {
            // يجب تنفيذ هذا باستخدام قاعدة البيانات
            // هذا تنفيذ مؤقت للتوضيح
            return propertyIds.Select(id => new PropertyWithLocation
            {
                PropertyId = id,
                Latitude = 24.7136 + (new Random().NextDouble() - 0.5) * 0.1,
                Longitude = 46.6753 + (new Random().NextDouble() - 0.5) * 0.1,
                Name = $"Property {id}",
                DistanceKm = 0
            }).ToList();
        }

        /// <summary>
        /// فلترة الكيانات حسب الموقع الجغرافي
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.FilterByLocation
        /// </summary>
        /// <param name="properties">الكيانات مع المواقع</param>
        /// <param name="latitude">خط العرض</param>
        /// <param name="longitude">خط الطول</param>
        /// <param name="radiusKm">نطاق البحث بالكيلومتر</param>
        /// <returns>الكيانات ضمن النطاق الجغرافي</returns>
        public static List<PropertyWithLocation> FilterByLocation(
            List<PropertyWithLocation> properties, double latitude, double longitude, double radiusKm)
        {
            return properties.Where(p =>
            {
                var distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude);
                p.DistanceKm = distance;
                return distance <= radiusKm;
            }).ToList();
        }

        /// <summary>
        /// حساب المسافة بين نقطتين جغرافيتين
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.CalculateDistance
        /// </summary>
        /// <param name="lat1">خط العرض الأول</param>
        /// <param name="lon1">خط الطول الأول</param>
        /// <param name="lat2">خط العرض الثاني</param>
        /// <param name="lon2">خط الطول الثاني</param>
        /// <returns>المسافة بالكيلومتر</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // نصف قطر الأرض بالكيلومتر
            
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// تحميل بيانات الترتيب للكيانات
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.LoadSortingDataAsync
        /// </summary>
        /// <param name="propertyIds">معرفات الكيانات</param>
        /// <param name="sortBy">نوع الترتيب</param>
        /// <returns>بيانات الترتيب</returns>
        public static async Task<Dictionary<int, double>> LoadSortingDataAsync(List<int> propertyIds, string sortBy)
        {
            var sortingData = new Dictionary<int, double>();
            var random = new Random();

            foreach (var id in propertyIds)
            {
                sortingData[id] = sortBy.ToLowerInvariant() switch
                {
                    "price" => 100 + random.NextDouble() * 900, // سعر وهمي 100-1000
                    "rating" => 1 + random.NextDouble() * 4, // تقييم وهمي 1-5
                    "reviews" => random.Next(0, 500), // عدد تقييمات وهمي
                    _ => random.NextDouble() // ترتيب عشوائي
                };
            }

            return sortingData;
        }

        /// <summary>
        /// حساب درجة الصلة للبحث النصي
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.CalculateRelevanceScore
        /// </summary>
        /// <param name="searchTerm">مصطلح البحث</param>
        /// <param name="foundWord">الكلمة الموجودة</param>
        /// <param name="frequency">تكرار الكلمة</param>
        /// <returns>درجة الصلة</returns>
        public static double CalculateRelevanceScore(string searchTerm, string foundWord, int frequency)
        {
            var baseScore = 0.5;
            
            // زيادة النقاط للتطابق الدقيق
            if (string.Equals(searchTerm, foundWord, StringComparison.OrdinalIgnoreCase))
            {
                baseScore = 1.0;
            }
            // زيادة النقاط للتطابق الجزئي
            else if (foundWord.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                baseScore = 0.8;
            }
            // زيادة النقاط للبداية المتطابقة
            else if (foundWord.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                baseScore = 0.7;
            }

            // ضرب النتيجة في معامل التكرار (محدود)
            var frequencyBoost = Math.Min(frequency / 10.0, 2.0);
            return baseScore * frequencyBoost;
        }

        /// <summary>
        /// الحصول على الكلمات المطابقة لكيان معين
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetMatchedTermsForProperty
        /// </summary>
        /// <param name="propertyId">معرف الكيان</param>
        /// <param name="searchTerms">مصطلحات البحث</param>
        /// <param name="textIndex">فهرس البحث النصي</param>
        /// <returns>الكلمات المطابقة</returns>
        public static List<string> GetMatchedTermsForProperty(int propertyId, List<string> searchTerms, TextSearchIndex textIndex)
        {
            var matchedTerms = new List<string>();

            foreach (var term in searchTerms)
            {
                if (textIndex.PopularTerms.TryGetValue(term, out var searchTerm) &&
                    searchTerm.PropertyIds.Contains(propertyId))
                {
                    matchedTerms.Add(term);
                }
            }

            return matchedTerms;
        }

        /// <summary>
        /// الحصول على اقتراحات التصحيح للبحث
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetSearchCorrectionsAsync
        /// </summary>
        /// <param name="query">الاستعلام الأصلي</param>
        /// <param name="textIndex">فهرس البحث النصي</param>
        /// <returns>اقتراحات التصحيح</returns>
        public static async Task<List<string>> GetSearchCorrectionsAsync(string query, TextSearchIndex textIndex)
        {
            var corrections = new List<string>();
            var words = ExtractSearchableWords(query);

            foreach (var word in words)
            {
                if (textIndex.SpellCorrections.TryGetValue(word, out var correction))
                {
                    corrections.Add(correction);
                }
                else
                {
                    // البحث عن كلمات مشابهة باستخدام Levenshtein Distance
                    var similarWords = textIndex.PopularTerms.Keys
                        .Where(key => CalculateLevenshteinDistance(word, key) <= 2)
                        .OrderBy(key => CalculateLevenshteinDistance(word, key))
                        .Take(1);
                    
                    corrections.AddRange(similarWords);
                }
            }

            return corrections.Distinct().ToList();
        }

        #endregion

        #region Performance Helpers

        /// <summary>
        /// الحصول على حجم الكاش
        /// الموقع: 08-دليل_التنفيذ_والتشغيل.md - في SystemInfo
        /// </summary>
        /// <param name="cache">كاش الذاكرة</param>
        /// <returns>عدد عناصر الكاش</returns>
        public static int GetCacheSize(Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            // استخدام Reflection للوصول لحجم الكاش
            var field = typeof(Microsoft.Extensions.Caching.Memory.MemoryCache).GetField("_coherentState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field?.GetValue(cache) is not object coherentState) return 0;
            
            var entriesCollection = coherentState.GetType()
                .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (entriesCollection?.GetValue(coherentState) is System.Collections.IDictionary entries)
                return entries.Count;
            
            return 0;
        }

        /// <summary>
        /// حساب حجم مجلد بالبايت
        /// الموقع: 08-دليل_التنفيذ_والتشغيل.md - في SystemInfo
        /// </summary>
        /// <param name="directoryPath">مسار المجلد</param>
        /// <returns>الحجم بالبايت</returns>
        public static long CalculateDirectorySize(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return 0;

            try
            {
                return new DirectoryInfo(directoryPath)
                    .GetFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// الحصول على استهلاك المعالج
        /// الموقع: 08-دليل_التنفيذ_والتشغيل.md - في SystemInfo
        /// </summary>
        /// <returns>نسبة استهلاك المعالج</returns>
        public static double GetCpuUsage()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                return process.TotalProcessorTime.TotalMilliseconds;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// الحصول على استهلاك القرص
        /// الموقع: 08-دليل_التنفيذ_والتشغيل.md - في SystemInfo
        /// </summary>
        /// <returns>معلومات استهلاك القرص</returns>
        public static object GetDiskUsage()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady)
                    .Select(d => new
                    {
                        Name = d.Name,
                        TotalSize = d.TotalSize,
                        FreeSpace = d.AvailableFreeSpace,
                        UsedSpace = d.TotalSize - d.AvailableFreeSpace,
                        UsagePercentage = Math.Round((double)(d.TotalSize - d.AvailableFreeSpace) / d.TotalSize * 100, 2)
                    });
                
                return drives.ToList();
            }
            catch
            {
                return new { };
            }
        }

        #endregion

        #region Private Helper Methods

        private static double CalculateAverageStayDuration(List<Properties> properties)
        {
            // تنفيذ مؤقت - يجب حساب متوسط مدة الإقامة من الحجوزات الفعلية
            return 3.5; // متوسط 3.5 يوم
        }

        private static List<PopularDistrict> GetPopularDistricts(List<Properties> properties)
        {
            // تنفيذ مؤقت - يجب استخراج الأحياء من العناوين
            return new List<PopularDistrict>
            {
                new PopularDistrict { Name = "المربع", PropertyCount = 50, AveragePrice = 300, PopularPropertyType = "Hotel" },
                new PopularDistrict { Name = "العليا", PropertyCount = 75, AveragePrice = 450, PopularPropertyType = "Apartment" }
            };
        }

        private static Dictionary<string, SeasonalStatistics> CalculateSeasonalStatistics(List<Properties> properties)
        {
            return new Dictionary<string, SeasonalStatistics>
            {
                ["summer"] = new SeasonalStatistics { AveragePriceInSeason = 350, OccupancyRate = 0.85, AverageStayDuration = 4.2, DemandChangePercentage = 15 },
                ["winter"] = new SeasonalStatistics { AveragePriceInSeason = 280, OccupancyRate = 0.70, AverageStayDuration = 3.1, DemandChangePercentage = -10 }
            };
        }

        private static Dictionary<string, int> GroupPropertiesByTypeIds(List<Properties> properties)
        {
            return properties
                .Where(p => !string.IsNullOrEmpty(p.TypeId))
                .GroupBy(p => GetPropertyTypeName(p.TypeId))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private static decimal CalculateAveragePriceForAmenity(List<PropertyAmenities> propertyAmenities)
        {
            var properties = propertyAmenities
                .Select(pa => pa.Property)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            return CalculateAveragePrice(properties);
        }

        private static double CalculateAverageRatingForAmenity(List<PropertyAmenities> propertyAmenities)
        {
            var properties = propertyAmenities
                .Select(pa => pa.Property)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            return CalculateAverageRatingForProperties(properties);
        }

        private static double CalculateAmenityPopularity(int amenityCount, int totalProperties)
        {
            return totalProperties > 0 ? (double)amenityCount / totalProperties : 0;
        }

        private static List<string> GetAmenityPopularCategories(List<PropertyAmenities> propertyAmenities)
        {
            // تنفيذ مؤقت - يجب تحليل الفئات المستهدفة الفعلية
            return new List<string> { "families", "business", "tourists" };
        }

        private static int CalculateDefaultCapacity(List<Properties> properties)
        {
            var capacities = properties
                .SelectMany(p => p.Units ?? Enumerable.Empty<Units>())
                .Where(u => u.MaxCapacity > 0)
                .Select(u => u.MaxCapacity)
                .ToList();

            return capacities.Any() ? (int)capacities.Average() : 2;
        }

        private static List<string> GetCommonAmenitiesForType(List<Properties> properties)
        {
            return properties
                .SelectMany(p => p.PropertyAmenities ?? Enumerable.Empty<PropertyAmenities>())
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .Where(pa => pa.PropertyTypeAmenity?.Amenity?.Name != null)
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name!)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key.ToLowerInvariant())
                .ToList();
        }

        private static int GetAvailablePropertiesForDate(List<Properties> properties, DateTime date)
        {
            return properties.Count(p =>
                (p.Units ?? Enumerable.Empty<Units>()).Any(u =>
                    !(u.UnitAvailabilities ?? Enumerable.Empty<UnitAvailability>()).Any(ua =>
                        ua.Status == "Booked" &&
                        ua.IsDeleted == false &&
                        ua.StartDate <= date &&
                        ua.EndDate >= date)));
        }

        private static bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string> { "في", "من", "إلى", "على", "عن", "مع", "أو", "و", "لا", "ما", "هذا", "هذه", "التي", "الذي" };
            return stopWords.Contains(word);
        }

        private static double CalculateWordWeight(string word)
        {
            // حساب وزن الكلمة بناء على طولها وأهميتها
            if (word.Length <= 2) return 0.5;
            if (word.Length <= 4) return 0.8;
            return 1.0;
        }

        private static string DetermineWordCategory(string word)
        {
            var categories = new Dictionary<string, List<string>>
            {
                ["amenity"] = new List<string> { "مسبح", "واي فاي", "موقف", "مطعم", "صالة ألعاب" },
                ["type"] = new List<string> { "فندق", "شقة", "فيلا", "منتجع" },
                ["location"] = new List<string> { "الرياض", "جدة", "الدمام", "مكة" }
            };

            foreach (var category in categories)
            {
                if (category.Value.Any(term => term.Contains(word) || word.Contains(term)))
                {
                    return category.Key;
                }
            }

            return "general";
        }

        private static void FindCompletionsRecursive(Dictionary<string, object> node, string prefix, List<string> completions, int maxResults)
        {
            if (completions.Count >= maxResults) return;

            if (node.ContainsKey("isComplete") && node.ContainsKey("propertyIds"))
            {
                completions.Add(prefix);
            }

            foreach (var key in node.Keys)
            {
                if (key != "isComplete" && key != "propertyIds" && node[key] is Dictionary<string, object> childNode)
                {
                    FindCompletionsRecursive(childNode, prefix + key, completions, maxResults);
                    if (completions.Count >= maxResults) break;
                }
            }
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static List<string> ProcessSearchQuery(string query)
        {
            return ExtractSearchableWords(query);
        }

        private static int CalculateLevenshteinDistance(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
            if (string.IsNullOrEmpty(s2)) return s1.Length;

            var matrix = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[s1.Length, s2.Length];
        }

        /// <summary>
        /// الحصول على المرافق الشائعة لقائمة كيانات
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService.GetPopularAmenitiesForPropertiesAsync
        /// </summary>
        /// <param name="propertyIds">معرفات الكيانات</param>
        /// <returns>قائمة المرافق الشائعة</returns>
        public static async Task<List<string>> GetPopularAmenitiesForPropertiesAsync(List<int> propertyIds)
        {
            // تنفيذ مؤقت - يجب تحميل المرافق الفعلية من قاعدة البيانات
            return new List<string> { "wifi", "parking", "pool", "gym", "restaurant" };
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// نتيجة البحث في Trie
    /// </summary>
    public class TrieSearchResult
    {
        public string Word { get; set; } = string.Empty;
        public List<int> PropertyIds { get; set; } = new();
        public bool IsExactMatch { get; set; }
    }

    /// <summary>
    /// كيان مع معلومات الموقع
    /// </summary>
    public class PropertyWithLocation
    {
        public int PropertyId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }

    #endregion
}