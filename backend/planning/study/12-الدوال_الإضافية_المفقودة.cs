using YemenBooking.Core.Entities;
using YemenBooking.Core.DTOs.Search;
using System.Collections.Concurrent;

namespace YemenBooking.Infrastructure.Services.Indexing
{
    /// <summary>
    /// ملف الدوال الإضافية المفقودة - الجزء الثاني
    /// هذا الملف يحتوي على دوال إضافية مفقودة من النظام
    /// </summary>
    public static class AdditionalMissingFunctions
    {
        /// <summary>
        /// الحصول على قائمة المدن المتاحة من الفهارس
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService
        /// </summary>
        /// <returns>قائمة المدن مع إحصائياتها</returns>
        public static async Task<List<CityAvailabilityInfo>> GetAvailableCitiesAsync()
        {
            var citiesInfo = new List<CityAvailabilityInfo>();

            // قراءة فهارس المدن من الملفات
            var indexPath = Path.Combine("wwwroot", "indexes", "cities");
            if (!Directory.Exists(indexPath))
                return citiesInfo;

            var cityFiles = Directory.GetFiles(indexPath, "city_*.json");

            foreach (var file in cityFiles)
            {
                var cityName = Path.GetFileNameWithoutExtension(file).Replace("city_", "");
                var indexContent = await File.ReadAllTextAsync(file);

                if (!string.IsNullOrEmpty(indexContent))
                {
                    var cityIndex = System.Text.Json.JsonSerializer.Deserialize<CityIndex>(indexContent);
                    if (cityIndex?.PropertyIds?.Any() == true)
                    {
                        citiesInfo.Add(new CityAvailabilityInfo
                        {
                            CityName = cityName,
                            CityNameAr = cityIndex.Metadata?.CityNameAr ?? cityName,
                            AvailablePropertiesCount = cityIndex.PropertyIds.Count,
                            AveragePricePerNight = cityIndex.Statistics?.AveragePricePerNight ?? 0,
                            MinPricePerNight = cityIndex.Statistics?.MinPricePerNight ?? 0,
                            MaxPricePerNight = cityIndex.Statistics?.MaxPricePerNight ?? 0,
                            PopularAmenities = cityIndex.Statistics?.PopularAmenities ?? new List<string>()
                        });
                    }
                }
            }

            return citiesInfo.OrderByDescending(c => c.AvailablePropertiesCount).ToList();
        }

        /// <summary>
        /// الحصول على معلومات الفلاتر المطبقة
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService
        /// </summary>
        /// <param name="criteria">معايير البحث</param>
        /// <returns>معلومات الفلاتر المطبقة</returns>
        public static AppliedFiltersInfo GetAppliedFiltersInfo(SearchCriteria criteria)
        {
            var filtersInfo = new AppliedFiltersInfo
            {
                TotalFiltersApplied = 0,
                FilterCategories = new List<FilterCategoryInfo>()
            };

            // فلتر الموقع
            if (!string.IsNullOrWhiteSpace(criteria.City) || !string.IsNullOrWhiteSpace(criteria.Country))
            {
                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "الموقع الجغرافي",
                    CategoryNameEn = "Location",
                    FiltersCount = (!string.IsNullOrWhiteSpace(criteria.City) ? 1 : 0) +
                                 (!string.IsNullOrWhiteSpace(criteria.Country) ? 1 : 0),
                    ActiveFilters = new List<string>
                    {
                        criteria.City,
                        criteria.Country
                    }.Where(f => !string.IsNullOrWhiteSpace(f)).ToList()
                });
                filtersInfo.TotalFiltersApplied++;
            }

            // فلتر السعر
            if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
            {
                var priceFilters = new List<string>();
                if (criteria.MinPrice.HasValue)
                    priceFilters.Add($"الحد الأدنى: {criteria.MinPrice.Value:C}");
                if (criteria.MaxPrice.HasValue)
                    priceFilters.Add($"الحد الأعلى: {criteria.MaxPrice.Value:C}");

                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "النطاق السعري",
                    CategoryNameEn = "Price Range",
                    FiltersCount = priceFilters.Count,
                    ActiveFilters = priceFilters
                });
                filtersInfo.TotalFiltersApplied++;
            }

            // فلتر التاريخ
            if (criteria.CheckIn.HasValue || criteria.CheckOut.HasValue)
            {
                var dateFilters = new List<string>();
                if (criteria.CheckIn.HasValue)
                    dateFilters.Add($"تسجيل الوصول: {criteria.CheckIn.Value:yyyy-MM-dd}");
                if (criteria.CheckOut.HasValue)
                    dateFilters.Add($"تسجيل المغادرة: {criteria.CheckOut.Value:yyyy-MM-dd}");

                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "التواريخ",
                    CategoryNameEn = "Dates",
                    FiltersCount = dateFilters.Count,
                    ActiveFilters = dateFilters
                });
                filtersInfo.TotalFiltersApplied++;
            }

            // فلتر الأشخاص
            if (criteria.Adults > 0 || criteria.Children > 0 || criteria.Infants > 0)
            {
                var guestFilters = new List<string>();
                if (criteria.Adults > 0)
                    guestFilters.Add($"البالغين: {criteria.Adults}");
                if (criteria.Children > 0)
                    guestFilters.Add($"الأطفال: {criteria.Children}");
                if (criteria.Infants > 0)
                    guestFilters.Add($"الرضع: {criteria.Infants}");

                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "عدد الأشخاص",
                    CategoryNameEn = "Guests",
                    FiltersCount = guestFilters.Count,
                    ActiveFilters = guestFilters
                });
                filtersInfo.TotalFiltersApplied++;
            }

            // فلتر المرافق
            if (criteria.RequiredAmenities?.Any() == true)
            {
                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "المرافق المطلوبة",
                    CategoryNameEn = "Required Amenities",
                    FiltersCount = criteria.RequiredAmenities.Count,
                    ActiveFilters = criteria.RequiredAmenities.ToList()
                });
                filtersInfo.TotalFiltersApplied++;
            }

            // فلتر نوع العقار
            if (criteria.PropertyTypes?.Any() == true)
            {
                filtersInfo.FilterCategories.Add(new FilterCategoryInfo
                {
                    CategoryName = "أنواع العقارات",
                    CategoryNameEn = "Property Types",
                    FiltersCount = criteria.PropertyTypes.Count,
                    ActiveFilters = criteria.PropertyTypes.ToList()
                });
                filtersInfo.TotalFiltersApplied++;
            }

            return filtersInfo;
        }

        /// <summary>
        /// التحقق من مطابقة النطاق السعري
        /// الموقع: 05-خدمة_البحث_السريع.cs - في FastSearchService
        /// </summary>
        /// <param name="price">السعر المراد فحصه</param>
        /// <param name="minPrice">الحد الأدنى للسعر</param>
        /// <param name="maxPrice">الحد الأعلى للسعر</param>
        /// <returns>true إذا كان السعر ضمن النطاق المحدد</returns>
        public static bool IsPriceRangeMatch(decimal price, decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice.HasValue && price < minPrice.Value)
                return false;

            if (maxPrice.HasValue && price > maxPrice.Value)
                return false;

            return true;
        }

        /// <summary>
        /// الحصول على مفاتيح النطاقات السعرية
        /// الموقع: 04-خدمة_إنشاء_الفهارس.cs - في IndexGenerationService
        /// </summary>
        /// <returns>قائمة بمفاتيح النطاقات السعرية</returns>
        public static List<string> GetPriceRangeKeys()
        {
            return new List<string>
            {
                "0-50",      // اقتصادي جداً
                "50-100",    // اقتصادي
                "100-200",   // متوسط
                "200-300",   // فوق المتوسط
                "300-500",   // راقي
                "500-750",   // فاخر
                "750-1000",  // فاخر جداً
                "1000+"      // استثنائي
            };
        }

        /// <summary>
        /// حساب تفصيل أنواع الوحدات
        /// الموقع: 09-نماذج_الفهارس_الكاملة.cs - في helper functions section
        /// </summary>
        /// <param name="units">قائمة الوحدات</param>
        /// <returns>تفصيل أنواع الوحدات مع الإحصائيات</returns>
        public static Dictionary<string, UnitTypeBreakdown> CalculateUnitTypeBreakdown(List<Unit> units)
        {
            var breakdown = new Dictionary<string, UnitTypeBreakdown>();

            if (units?.Any() != true)
                return breakdown;

            var groupedUnits = units.GroupBy(u => u.UnitType?.TypeName ?? "غير محدد");

            foreach (var group in groupedUnits)
            {
                var unitsList = group.ToList();
                var prices = unitsList
                    .Where(u => u.PricingRules?.Any() == true)
                    .SelectMany(u => u.PricingRules.Select(pr => pr.PricePerNight))
                    .Where(p => p > 0)
                    .ToList();

                breakdown[group.Key] = new UnitTypeBreakdown
                {
                    TypeName = group.Key,
                    TypeNameEn = GetEnglishUnitTypeName(group.Key),
                    UnitsCount = unitsList.Count,
                    AvailableUnitsCount = unitsList.Count(u => u.UnitAvailabilities?
                        .Any(ua => ua.Date >= DateTime.Today && ua.IsAvailable) == true),
                    AveragePrice = prices.Any() ? Math.Round(prices.Average(), 2) : 0,
                    MinPrice = prices.Any() ? prices.Min() : 0,
                    MaxPrice = prices.Any() ? prices.Max() : 0,
                    MaxCapacity = unitsList.Max(u => u.MaxGuests ?? 0),
                    AverageCapacity = Math.Round(unitsList.Average(u => u.MaxGuests ?? 0), 1),
                    TotalBedrooms = unitsList.Sum(u => u.Bedrooms ?? 0),
                    TotalBathrooms = unitsList.Sum(u => u.Bathrooms ?? 0),
                    AverageBedrooms = Math.Round(unitsList.Average(u => u.Bedrooms ?? 0), 1),
                    AverageBathrooms = Math.Round(unitsList.Average(u => u.Bathrooms ?? 0), 1),
                    CommonAmenities = GetCommonAmenities(unitsList),
                    PopularityScore = CalculateUnitTypePopularity(unitsList)
                };
            }

            return breakdown.OrderByDescending(kvp => kvp.Value.UnitsCount).ToDictionary();
        }

        /// <summary>
        /// الحصول على الاسم الإنجليزي لنوع الوحدة
        /// دالة مساعدة لـ CalculateUnitTypeBreakdown
        /// </summary>
        private static string GetEnglishUnitTypeName(string arabicName)
        {
            var translations = new Dictionary<string, string>
            {
                { "شقة", "Apartment" },
                { "فيلا", "Villa" },
                { "منزل", "House" },
                { "استوديو", "Studio" },
                { "جناح", "Suite" },
                { "غرفة", "Room" },
                { "شاليه", "Chalet" },
                { "مزرعة", "Farm" },
                { "قصر", "Palace" },
                { "غير محدد", "Unspecified" }
            };

            return translations.ContainsKey(arabicName) ? translations[arabicName] : arabicName;
        }

        /// <summary>
        /// الحصول على المرافق الشائعة للوحدات
        /// دالة مساعدة لـ CalculateUnitTypeBreakdown
        /// </summary>
        private static List<string> GetCommonAmenities(List<Unit> units)
        {
            var amenityCount = new Dictionary<string, int>();

            foreach (var unit in units)
            {
                if (unit.Property?.PropertyAmenities?.Any() == true)
                {
                    foreach (var amenity in unit.Property.PropertyAmenities)
                    {
                        var amenityName = amenity.Amenity?.Name ?? amenity.Amenity?.NameAr ?? "غير محدد";
                        amenityCount[amenityName] = amenityCount.ContainsKey(amenityName)
                            ? amenityCount[amenityName] + 1
                            : 1;
                    }
                }
            }

            // إرجاع المرافق التي تظهر في أكثر من 50% من الوحدات
            var threshold = Math.Ceiling(units.Count * 0.5);
            return amenityCount
                .Where(kvp => kvp.Value >= threshold)
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Take(10) // أهم 10 مرافق
                .ToList();
        }

        /// <summary>
        /// حساب نقاط الشعبية لنوع الوحدة
        /// دالة مساعدة لـ CalculateUnitTypeBreakdown
        /// </summary>
        private static double CalculateUnitTypePopularity(List<Unit> units)
        {
            if (!units.Any()) return 0;

            double popularityScore = 0;

            // عدد الوحدات (30%)
            popularityScore += Math.Min(units.Count / 50.0, 1.0) * 30;

            // متوسط التقييمات (40%)
            var avgRating = units
                .Where(u => u.Property?.Reviews?.Any() == true)
                .SelectMany(u => u.Property.Reviews)
                .Where(r => r.Rating.HasValue)
                .Select(r => r.Rating.Value)
                .DefaultIfEmpty(0)
                .Average();
            popularityScore += (avgRating / 5.0) * 40;

            // عدد الحجوزات (30%)
            var totalBookings = units
                .Where(u => u.Bookings?.Any() == true)
                .Sum(u => u.Bookings.Count);
            popularityScore += Math.Min(totalBookings / 100.0, 1.0) * 30;

            return Math.Round(popularityScore, 2);
        }
    }

    /// <summary>
    /// معلومات توفر المدن
    /// </summary>
    public class CityAvailabilityInfo
    {
        public string CityName { get; set; } = string.Empty;
        public string CityNameAr { get; set; } = string.Empty;
        public int AvailablePropertiesCount { get; set; }
        public decimal AveragePricePerNight { get; set; }
        public decimal MinPricePerNight { get; set; }
        public decimal MaxPricePerNight { get; set; }
        public List<string> PopularAmenities { get; set; } = new();
    }

    /// <summary>
    /// معلومات الفلاتر المطبقة
    /// </summary>
    public class AppliedFiltersInfo
    {
        public int TotalFiltersApplied { get; set; }
        public List<FilterCategoryInfo> FilterCategories { get; set; } = new();
    }

    /// <summary>
    /// معلومات فئة الفلتر
    /// </summary>
    public class FilterCategoryInfo
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryNameEn { get; set; } = string.Empty;
        public int FiltersCount { get; set; }
        public List<string> ActiveFilters { get; set; } = new();
    }

    /// <summary>
    /// تفصيل نوع الوحدة
    /// </summary>
    public class UnitTypeBreakdown
    {
        public string TypeName { get; set; } = string.Empty;
        public string TypeNameEn { get; set; } = string.Empty;
        public int UnitsCount { get; set; }
        public int AvailableUnitsCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int MaxCapacity { get; set; }
        public double AverageCapacity { get; set; }
        public int TotalBedrooms { get; set; }
        public int TotalBathrooms { get; set; }
        public double AverageBedrooms { get; set; }
        public double AverageBathrooms { get; set; }
        public List<string> CommonAmenities { get; set; } = new();
        public double PopularityScore { get; set; }
    }


}