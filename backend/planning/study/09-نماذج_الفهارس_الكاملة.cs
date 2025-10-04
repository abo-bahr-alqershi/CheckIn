using System;
using System.Collections.Generic;

namespace BookN.Core.DTOs.Indexing
{
    #region Base Models

    /// <summary>
    /// بيانات الميتاداتا العامة للفهرس
    /// تحتوي على معلومات أساسية مشتركة لجميع الفهارس
    /// </summary>
    public class IndexMetadata
    {
        /// <summary>
        /// معرف الفهرس الفريد
        /// </summary>
        public string IndexId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// نوع الفهرس (city, price-range, amenity, etc.)
        /// </summary>
        public string IndexType { get; set; } = string.Empty;

        /// <summary>
        /// مفتاح الفهرس (sanaa, range_100_500, etc.)
        /// </summary>
        public string IndexKey { get; set; } = string.Empty;

        /// <summary>
        /// إصدار الفهرس
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// تاريخ آخر تحديث
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// تاريخ انتهاء الصلاحية
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// العدد الإجمالي للعناصر في الفهرس
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// حجم الفهرس بالبايت
        /// </summary>
        public long SizeBytes { get; set; }

        /// <summary>
        /// معلومات إضافية
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// بيانات الخطأ للفهرس
    /// </summary>
    public class IndexError
    {
        /// <summary>
        /// رمز الخطأ
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// رسالة الخطأ
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ حدوث الخطأ
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// تفاصيل إضافية عن الخطأ
        /// </summary>
        public string? Details { get; set; }
    }

    #endregion

    #region City Index Models

    /// <summary>
    /// فهرس بيانات المدن الشامل
    /// يحتوي على جميع البيانات المتعلقة بمدينة معينة
    /// </summary>
    public class CityIndex
    {
        /// <summary>
        /// بيانات الميتاداتا للفهرس
        /// </summary>
        public CityIndexMetadata Metadata { get; set; } = new();

        /// <summary>
        /// الإحصائيات الأساسية للمدينة
        /// </summary>
        public CityStatistics Statistics { get; set; } = new();

        /// <summary>
        /// توزيع الكيانات حسب نطاقات الأسعار
        /// </summary>
        public Dictionary<string, PriceRangeData> PriceRanges { get; set; } = new();

        /// <summary>
        /// توزيع الكيانات حسب المرافق
        /// </summary>
        public Dictionary<string, AmenityData> AmenityIntersections { get; set; } = new();

        /// <summary>
        /// توزيع الكيانات حسب الأنواع
        /// </summary>
        public Dictionary<string, PropertyTypeData> PropertyTypeDistribution { get; set; } = new();

        /// <summary>
        /// التوفر الشهري للكيانات
        /// </summary>
        public Dictionary<string, AvailabilityData> AvailabilityCalendar { get; set; } = new();

        /// <summary>
        /// الأماكن الشائعة والمعالم القريبة
        /// </summary>
        public List<Landmark> NearbyLandmarks { get; set; } = new();

        /// <summary>
        /// الطقس الموسمي والأحداث
        /// </summary>
        public Dictionary<string, SeasonalInfo> SeasonalData { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا خاصة بفهرس المدينة
    /// </summary>
    public class CityIndexMetadata : IndexMetadata
    {
        /// <summary>
        /// اسم المدينة
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// معرف المدينة
        /// </summary>
        public string CityId { get; set; } = string.Empty;

        /// <summary>
        /// العدد الإجمالي للكيانات
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// إحداثيات مركز المدينة
        /// </summary>
        public GeoPoint CityCenter { get; set; } = new();

        /// <summary>
        /// نطاق المدينة الجغرافي
        /// </summary>
        public GeoBounds CityBounds { get; set; } = new();

        /// <summary>
        /// المنطقة الزمنية
        /// </summary>
        public string TimeZone { get; set; } = "Asia/Riyadh";

        /// <summary>
        /// العملة المستخدمة
        /// </summary>
        public string Currency { get; set; } = "SAR";
    }

    /// <summary>
    /// إحصائيات المدن الشاملة
    /// </summary>
    public class CityStatistics
    {
        /// <summary>
        /// متوسط الأسعار في المدينة
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// أقل سعر متاح
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// أعلى سعر متاح
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// المرافق الأكثر شيوعاً
        /// </summary>
        public List<string> PopularAmenities { get; set; } = new();

        /// <summary>
        /// متوسط التقييمات
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// العدد الإجمالي للتقييمات
        /// </summary>
        public int TotalReviews { get; set; }

        /// <summary>
        /// معدل الإشغال
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// متوسط مدة الإقامة
        /// </summary>
        public double AverageStayDuration { get; set; }

        /// <summary>
        /// أكثر الأحياء شيوعاً
        /// </summary>
        public List<PopularDistrict> PopularDistricts { get; set; } = new();

        /// <summary>
        /// الإحصائيات الموسمية
        /// </summary>
        public Dictionary<string, SeasonalStatistics> SeasonalStats { get; set; } = new();
    }

    #endregion

    #region Price Range Models

    /// <summary>
    /// بيانات نطاق السعر المفصلة
    /// </summary>
    public class PriceRangeData
    {
        /// <summary>
        /// عدد الكيانات في هذا النطاق
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات (أول 100 كيان)
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();

        /// <summary>
        /// متوسط السعر في النطاق
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// متوسط التقييم في النطاق
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// معدل الإشغال في النطاق
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// التوزيع الجغرافي للكيانات
        /// </summary>
        public Dictionary<string, int> CityDistribution { get; set; } = new();

        /// <summary>
        /// أنواع الكيانات الشائعة في هذا النطاق
        /// </summary>
        public Dictionary<string, int> PropertyTypeDistribution { get; set; } = new();
    }

    /// <summary>
    /// فهرس نطاقات الأسعار
    /// </summary>
    public class PriceRangeIndex
    {
        /// <summary>
        /// ميتاداتا الفهرس
        /// </summary>
        public PriceRangeMetadata Metadata { get; set; } = new();

        /// <summary>
        /// توزيع الكيانات حسب المدن
        /// </summary>
        public Dictionary<string, CityPriceData> CityBreakdown { get; set; } = new();

        /// <summary>
        /// تقاطع مع أنواع الكيانات
        /// </summary>
        public Dictionary<string, PropertyTypeData> PropertyTypeIntersection { get; set; } = new();

        /// <summary>
        /// تقاطع مع المرافق
        /// </summary>
        public Dictionary<string, AmenityData> AmenityIntersection { get; set; } = new();

        /// <summary>
        /// التسعير الموسمي
        /// </summary>
        public SeasonalPricingData SeasonalPricing { get; set; } = new();

        /// <summary>
        /// اتجاهات الأسعار
        /// </summary>
        public PriceTrendsData PriceTrends { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا فهرس نطاق الأسعار
    /// </summary>
    public class PriceRangeMetadata : IndexMetadata
    {
        /// <summary>
        /// نطاق السعر (100-300, 300-500, etc.)
        /// </summary>
        public string PriceRange { get; set; } = string.Empty;

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "SAR";

        /// <summary>
        /// العدد الإجمالي للكيانات في النطاق
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// الحد الأدنى للنطاق
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// الحد الأقصى للنطاق
        /// </summary>
        public decimal MaxPrice { get; set; }
    }

    #endregion

    #region Amenity Models

    /// <summary>
    /// بيانات المرفق
    /// </summary>
    public class AmenityData
    {
        /// <summary>
        /// عدد الكيانات التي تحتوي على هذا المرفق
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();

        /// <summary>
        /// متوسط السعر للكيانات التي تحتوي على هذا المرفق
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// متوسط التقييم
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// مدى الشيوع (0.0 - 1.0)
        /// </summary>
        public double Popularity { get; set; }

        /// <summary>
        /// الفئات الأكثر استخداماً لهذا المرفق
        /// </summary>
        public List<string> PopularWith { get; set; } = new();

        /// <summary>
        /// التكلفة الإضافية للمرفق إن وجدت
        /// </summary>
        public PriceInfo? ExtraCost { get; set; }
    }

    /// <summary>
    /// فهرس المرافق
    /// </summary>
    public class AmenityIndex
    {
        /// <summary>
        /// ميتاداتا المرفق
        /// </summary>
        public AmenityMetadata Metadata { get; set; } = new();

        /// <summary>
        /// توزيع الكيانات حسب المدن
        /// </summary>
        public Dictionary<string, CityAmenityData> CityDistribution { get; set; } = new();

        /// <summary>
        /// تقاطع مع نطاقات الأسعار
        /// </summary>
        public Dictionary<string, PriceRangeData> PriceRangeIntersection { get; set; } = new();

        /// <summary>
        /// تقاطع مع أنواع الكيانات
        /// </summary>
        public Dictionary<string, PropertyTypeData> PropertyTypeIntersection { get; set; } = new();

        /// <summary>
        /// المرافق المرتبطة والمتكاملة
        /// </summary>
        public Dictionary<string, RelatedAmenityData> RelatedAmenities { get; set; } = new();

        /// <summary>
        /// التوفر الموسمي للمرفق
        /// </summary>
        public SeasonalAvailabilityData SeasonalAvailability { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا فهرس المرفق
    /// </summary>
    public class AmenityMetadata : IndexMetadata
    {
        /// <summary>
        /// معرف المرفق
        /// </summary>
        public string AmenityId { get; set; } = string.Empty;

        /// <summary>
        /// اسم المرفق باللغة الإنجليزية
        /// </summary>
        public string AmenityName { get; set; } = string.Empty;

        /// <summary>
        /// اسم المرفق باللغة العربية
        /// </summary>
        public string AmenityNameAr { get; set; } = string.Empty;

        /// <summary>
        /// فئة المرفق (recreation, business, wellness, etc.)
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// وصف المرفق
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// أيقونة المرفق
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        /// العدد الإجمالي للكيانات التي تحتوي على هذا المرفق
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// درجة الأهمية (1-10)
        /// </summary>
        public int Priority { get; set; } = 5;
    }

    #endregion

    #region Property Type Models

    /// <summary>
    /// بيانات نوع الكيان
    /// </summary>
    public class PropertyTypeData
    {
        /// <summary>
        /// عدد الكيانات من هذا النوع
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعر لهذا النوع
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// متوسط التقييم
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();

        /// <summary>
        /// السعة الافتراضية
        /// </summary>
        public int DefaultCapacity { get; set; }

        /// <summary>
        /// المرافق الأكثر شيوعاً لهذا النوع
        /// </summary>
        public List<string> CommonAmenities { get; set; } = new();
    }

    /// <summary>
    /// فهرس أنواع الكيانات
    /// </summary>
    public class PropertyTypeIndex
    {
        /// <summary>
        /// ميتاداتا نوع الكيان
        /// </summary>
        public PropertyTypeMetadata Metadata { get; set; } = new();

        /// <summary>
        /// توزيع حسب التقييم النجمي
        /// </summary>
        public Dictionary<string, StarRatingData> StarRatingDistribution { get; set; } = new();

        /// <summary>
        /// توزيع حسب المدن
        /// </summary>
        public Dictionary<string, CityTypeData> CityDistribution { get; set; } = new();

        /// <summary>
        /// المرافق الشائعة لهذا النوع
        /// </summary>
        public Dictionary<string, AmenityAvailabilityData> CommonAmenities { get; set; } = new();

        /// <summary>
        /// تفصيل أنواع الوحدات
        /// </summary>
        public Dictionary<string, UnitTypeBreakdownData> UnitTypeBreakdown { get; set; } = new();

        /// <summary>
        /// السياسات الشائعة
        /// </summary>
        public CommonPoliciesData CommonPolicies { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا فهرس نوع الكيان
    /// </summary>
    public class PropertyTypeMetadata : IndexMetadata
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public string PropertyTypeId { get; set; } = string.Empty;

        /// <summary>
        /// اسم النوع بالإنجليزية
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// اسم النوع بالعربية
        /// </summary>
        public string TypeNameAr { get; set; } = string.Empty;

        /// <summary>
        /// وصف النوع
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// العدد الإجمالي للكيانات من هذا النوع
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// الفئة الرئيسية (accommodation, venue, etc.)
        /// </summary>
        public string MainCategory { get; set; } = string.Empty;
    }

    #endregion

    #region Availability Models

    /// <summary>
    /// بيانات التوفر
    /// </summary>
    public class AvailabilityData
    {
        /// <summary>
        /// عدد الكيانات المتاحة
        /// </summary>
        public int AvailableCount { get; set; }

        /// <summary>
        /// عدد الكيانات المحجوزة
        /// </summary>
        public int BookedCount { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات المتاحة
        /// </summary>
        public List<int> AvailableIds { get; set; } = new();

        /// <summary>
        /// متوسط السعر للمتاح
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// معدل الإشغال (0.0 - 1.0)
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// التوفر حسب نوع الكيان
        /// </summary>
        public Dictionary<string, int> TypeAvailability { get; set; } = new();
    }

    /// <summary>
    /// فهرس التوفر
    /// </summary>
    public class AvailabilityIndex
    {
        /// <summary>
        /// ميتاداتا التوفر
        /// </summary>
        public AvailabilityMetadata Metadata { get; set; } = new();

        /// <summary>
        /// التوفر اليومي
        /// </summary>
        public Dictionary<string, DailyAvailabilityData> DailyAvailability { get; set; } = new();

        /// <summary>
        /// توفر نهايات الأسبوع مقابل أيام الأسبوع
        /// </summary>
        public WeekendAvailabilityData WeekendAvailability { get; set; } = new();

        /// <summary>
        /// توفر نطاقات الأسعار
        /// </summary>
        public Dictionary<string, PriceRangeAvailabilityData> PriceRangeAvailability { get; set; } = new();

        /// <summary>
        /// التوقعات المستقبلية للتوفر
        /// </summary>
        public List<AvailabilityForecast> AvailabilityForecasts { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا فهرس التوفر
    /// </summary>
    public class AvailabilityMetadata : IndexMetadata
    {
        /// <summary>
        /// الشهر (2024-02)
        /// </summary>
        public string Month { get; set; } = string.Empty;

        /// <summary>
        /// العدد الإجمالي للكيانات
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// العدد الإجمالي للوحدات
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// معدل الإشغال العام
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// متوسط مدة الإقامة
        /// </summary>
        public double AverageStayDuration { get; set; }
    }

    #endregion

    #region Text Search Models

    /// <summary>
    /// فهرس البحث النصي
    /// </summary>
    public class TextSearchIndex
    {
        /// <summary>
        /// ميتاداتا البحث النصي
        /// </summary>
        public TextSearchMetadata Metadata { get; set; } = new();

        /// <summary>
        /// هيكل Trie للبحث السريع
        /// </summary>
        public Dictionary<string, object> Trie { get; set; } = new();

        /// <summary>
        /// المصطلحات الشائعة
        /// </summary>
        public Dictionary<string, TextSearchTerm> PopularTerms { get; set; } = new();

        /// <summary>
        /// المرادفات والكلمات المشابهة
        /// </summary>
        public Dictionary<string, List<string>> Synonyms { get; set; } = new();

        /// <summary>
        /// فهرس الكلمات المفتاحية حسب الفئة
        /// </summary>
        public Dictionary<string, CategoryKeywords> CategoryKeywords { get; set; } = new();

        /// <summary>
        /// التصحيح التلقائي للأخطاء الإملائية
        /// </summary>
        public Dictionary<string, string> SpellCorrections { get; set; } = new();
    }

    /// <summary>
    /// ميتاداتا فهرس البحث النصي
    /// </summary>
    public class TextSearchMetadata : IndexMetadata
    {
        /// <summary>
        /// العدد الإجمالي للكلمات المفهرسة
        /// </summary>
        public int TotalWords { get; set; }

        /// <summary>
        /// العدد الإجمالي للكيانات المفهرسة
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// اللغة الأساسية للفهرس
        /// </summary>
        public string Language { get; set; } = "ar_SA";

        /// <summary>
        /// اللغات المدعومة
        /// </summary>
        public List<string> SupportedLanguages { get; set; } = new() { "ar", "en" };

        /// <summary>
        /// إصدار خوارزمية البحث
        /// </summary>
        public string SearchAlgorithmVersion { get; set; } = "2.0";
    }

    /// <summary>
    /// بيانات مصطلح البحث النصي
    /// </summary>
    public class TextSearchTerm
    {
        /// <summary>
        /// المصطلح
        /// </summary>
        public string Term { get; set; } = string.Empty;

        /// <summary>
        /// تكرار المصطلح
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات التي تحتوي على هذا المصطلح
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();

        /// <summary>
        /// وزن المصطلح (أهمية)
        /// </summary>
        public double Weight { get; set; } = 1.0;

        /// <summary>
        /// فئة المصطلح (name, description, amenity, etc.)
        /// </summary>
        public string Category { get; set; } = "general";

        /// <summary>
        /// المرادفات
        /// </summary>
        public List<string> Synonyms { get; set; } = new();
    }

    #endregion

    #region Supporting Models

    /// <summary>
    /// نقطة جغرافية
    /// </summary>
    public class GeoPoint
    {
        /// <summary>
        /// خط العرض
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// خط الطول
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// الارتفاع (اختياري)
        /// </summary>
        public double? Altitude { get; set; }
    }

    /// <summary>
    /// حدود جغرافية
    /// </summary>
    public class GeoBounds
    {
        /// <summary>
        /// النقطة الشمالية الشرقية
        /// </summary>
        public GeoPoint NorthEast { get; set; } = new();

        /// <summary>
        /// النقطة الجنوبية الغربية
        /// </summary>
        public GeoPoint SouthWest { get; set; } = new();
    }

    /// <summary>
    /// معلم جغرافي
    /// </summary>
    public class Landmark
    {
        /// <summary>
        /// اسم المعلم
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// نوع المعلم (mall, airport, hospital, etc.)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// الموقع
        /// </summary>
        public GeoPoint Location { get; set; } = new();

        /// <summary>
        /// المسافة بالكيلومتر
        /// </summary>
        public double DistanceKm { get; set; }

        /// <summary>
        /// درجة الأهمية (1-10)
        /// </summary>
        public int Importance { get; set; } = 5;
    }

    /// <summary>
    /// معلومات السعر
    /// </summary>
    public class PriceInfo
    {
        /// <summary>
        /// المبلغ
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "SAR";

        /// <summary>
        /// نوع التسعير (per_night, per_hour, fixed, etc.)
        /// </summary>
        public string PricingType { get; set; } = "per_night";
    }

    /// <summary>
    /// بيانات موسمية
    /// </summary>
    public class SeasonalInfo
    {
        /// <summary>
        /// الموسم
        /// </summary>
        public string Season { get; set; } = string.Empty;

        /// <summary>
        /// الأشهر المتأثرة
        /// </summary>
        public List<int> Months { get; set; } = new();

        /// <summary>
        /// متوسط درجة الحرارة
        /// </summary>
        public double AverageTemperature { get; set; }

        /// <summary>
        /// الأحداث الشائعة
        /// </summary>
        public List<string> PopularEvents { get; set; } = new();

        /// <summary>
        /// تأثير على الأسعار (زيادة/نقصان)
        /// </summary>
        public double PriceImpactFactor { get; set; } = 1.0;
    }

    /// <summary>
    /// حي شائع
    /// </summary>
    public class PopularDistrict
    {
        /// <summary>
        /// اسم الحي
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// عدد الكيانات في الحي
        /// </summary>
        public int PropertyCount { get; set; }

        /// <summary>
        /// متوسط الأسعار في الحي
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// النوع الشائع للكيانات
        /// </summary>
        public string PopularPropertyType { get; set; } = string.Empty;
    }

    /// <summary>
    /// إحصائيات موسمية
    /// </summary>
    public class SeasonalStatistics
    {
        /// <summary>
        /// متوسط الأسعار في الموسم
        /// </summary>
        public decimal AveragePriceInSeason { get; set; }

        /// <summary>
        /// معدل الإشغال في الموسم
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// متوسط مدة الإقامة
        /// </summary>
        public double AverageStayDuration { get; set; }

        /// <summary>
        /// تغيير في الطلب مقارنة بالمتوسط السنوي
        /// </summary>
        public double DemandChangePercentage { get; set; }
    }

    #endregion

    #region Extended Models for Complex Operations

    /// <summary>
    /// بيانات سعر المدينة
    /// </summary>
    public class CityPriceData
    {
        /// <summary>
        /// عدد الكيانات في المدينة
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعر
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات التسعير الموسمي
    /// </summary>
    public class SeasonalPricingData
    {
        /// <summary>
        /// موسم الذروة
        /// </summary>
        public SeasonPriceData Peak { get; set; } = new();

        /// <summary>
        /// الموسم المنخفض
        /// </summary>
        public SeasonPriceData Off { get; set; } = new();

        /// <summary>
        /// الموسم المتوسط
        /// </summary>
        public SeasonPriceData Regular { get; set; } = new();
    }

    /// <summary>
    /// بيانات سعر الموسم
    /// </summary>
    public class SeasonPriceData
    {
        /// <summary>
        /// الأشهر المتأثرة
        /// </summary>
        public List<string> Months { get; set; } = new();

        /// <summary>
        /// متوسط الزيادة أو النقصان
        /// </summary>
        public double AverageChange { get; set; }

        /// <summary>
        /// عدد الكيانات المتأثرة
        /// </summary>
        public int AffectedProperties { get; set; }
    }

    /// <summary>
    /// بيانات اتجاهات الأسعار
    /// </summary>
    public class PriceTrendsData
    {
        /// <summary>
        /// الاتجاه العام (increasing, decreasing, stable)
        /// </summary>
        public string OverallTrend { get; set; } = "stable";

        /// <summary>
        /// متوسط التغيير الشهري
        /// </summary>
        public double MonthlyChangePercentage { get; set; }

        /// <summary>
        /// أعلى وأقل شهر من ناحية الأسعار
        /// </summary>
        public Dictionary<string, string> PriceExtremes { get; set; } = new();
    }

    /// <summary>
    /// بيانات مرفق المدينة
    /// </summary>
    public class CityAmenityData
    {
        /// <summary>
        /// عدد الكيانات في المدينة التي تحتوي على المرفق
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعر
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// الشريحة المستهدفة الشائعة
        /// </summary>
        public List<string> PopularWith { get; set; } = new();

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات المرفق المرتبط
    /// </summary>
    public class RelatedAmenityData
    {
        /// <summary>
        /// عدد الكيانات التي تحتوي على كلا المرفقين
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// معامل الارتباط (0.0 - 1.0)
        /// </summary>
        public double Correlation { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات التوفر الموسمي
    /// </summary>
    public class SeasonalAvailabilityData
    {
        /// <summary>
        /// بيانات الصيف
        /// </summary>
        public SeasonAvailabilityInfo Summer { get; set; } = new();

        /// <summary>
        /// بيانات الشتاء
        /// </summary>
        public SeasonAvailabilityInfo Winter { get; set; } = new();

        /// <summary>
        /// بيانات الربيع
        /// </summary>
        public SeasonAvailabilityInfo Spring { get; set; } = new();

        /// <summary>
        /// بيانات الخريف
        /// </summary>
        public SeasonAvailabilityInfo Autumn { get; set; } = new();
    }

    /// <summary>
    /// معلومات توفر الموسم
    /// </summary>
    public class SeasonAvailabilityInfo
    {
        /// <summary>
        /// معدل التوفر (0.0 - 1.0)
        /// </summary>
        public double AvailabilityRate { get; set; }

        /// <summary>
        /// عامل زيادة الشعبية
        /// </summary>
        public double PopularityBoost { get; set; } = 1.0;
    }

    /// <summary>
    /// بيانات التقييم النجمي
    /// </summary>
    public class StarRatingData
    {
        /// <summary>
        /// عدد الكيانات
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعر
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// متوسط التقييم
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات نوع المدينة
    /// </summary>
    public class CityTypeData
    {
        /// <summary>
        /// عدد الكيانات من هذا النوع في المدينة
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعر
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// أهم المرافق لهذا النوع في هذه المدينة
        /// </summary>
        public List<string> TopAmenities { get; set; } = new();

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات توفر المرفق
    /// </summary>
    public class AmenityAvailabilityData
    {
        /// <summary>
        /// معدل التوفر (0.0 - 1.0)
        /// </summary>
        public double Availability { get; set; }

        /// <summary>
        /// عدد الكيانات التي تحتوي على المرفق
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// أهم الكيانات التي تحتوي على هذا المرفق
        /// </summary>
        public List<int> TopProperties { get; set; } = new();
    }

    /// <summary>
    /// بيانات تفصيل نوع الوحدة
    /// </summary>
    public class UnitTypeBreakdownData
    {
        /// <summary>
        /// عدد الوحدات من هذا النوع
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// متوسط السعة
        /// </summary>
        public int AverageCapacity { get; set; }

        /// <summary>
        /// متوسط السعر
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// عدد الكيانات التي تحتوي على هذا النوع
        /// </summary>
        public int Properties { get; set; }
    }

    /// <summary>
    /// بيانات السياسات الشائعة
    /// </summary>
    public class CommonPoliciesData
    {
        /// <summary>
        /// سياسة الإلغاء الأكثر شيوعاً
        /// </summary>
        public string MostCommonCancellation { get; set; } = string.Empty;

        /// <summary>
        /// متوسط وقت تسجيل الوصول
        /// </summary>
        public string AverageCheckInTime { get; set; } = "15:00";

        /// <summary>
        /// متوسط وقت تسجيل المغادرة
        /// </summary>
        public string AverageCheckOutTime { get; set; } = "11:00";

        /// <summary>
        /// نسبة الكيانات التي تسمح بالحيوانات الأليفة
        /// </summary>
        public double PetFriendlyPercentage { get; set; }
    }

    /// <summary>
    /// بيانات التوفر اليومي
    /// </summary>
    public class DailyAvailabilityData
    {
        /// <summary>
        /// عدد الكيانات المتاحة
        /// </summary>
        public int AvailableProperties { get; set; }

        /// <summary>
        /// عدد الوحدات المتاحة
        /// </summary>
        public int AvailableUnits { get; set; }

        /// <summary>
        /// متوسط السعر لليوم
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات المتاحة
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// بيانات توفر نهاية الأسبوع
    /// </summary>
    public class WeekendAvailabilityData
    {
        /// <summary>
        /// بيانات نهايات الأسبوع
        /// </summary>
        public WeekendData Weekends { get; set; } = new();

        /// <summary>
        /// بيانات أيام الأسبوع
        /// </summary>
        public WeekendData Weekdays { get; set; } = new();
    }

    /// <summary>
    /// بيانات نهاية الأسبوع
    /// </summary>
    public class WeekendData
    {
        /// <summary>
        /// متوسط عدد الكيانات المتاحة
        /// </summary>
        public int AverageAvailable { get; set; }

        /// <summary>
        /// عامل تغيير السعر
        /// </summary>
        public double PriceChange { get; set; }

        /// <summary>
        /// المدن الأكثر شيوعاً
        /// </summary>
        public List<string> PopularCities { get; set; } = new();
    }

    /// <summary>
    /// بيانات توفر نطاق الأسعار
    /// </summary>
    public class PriceRangeAvailabilityData
    {
        /// <summary>
        /// عدد الكيانات المتاحة
        /// </summary>
        public int AvailableCount { get; set; }

        /// <summary>
        /// معدل الإشغال
        /// </summary>
        public double OccupancyRate { get; set; }

        /// <summary>
        /// قائمة معرفات الكيانات
        /// </summary>
        public List<int> PropertyIds { get; set; } = new();
    }

    /// <summary>
    /// توقعات التوفر
    /// </summary>
    public class AvailabilityForecast
    {
        /// <summary>
        /// التاريخ المتوقع
        /// </summary>
        public DateTime ForecastDate { get; set; }

        /// <summary>
        /// التوفر المتوقع
        /// </summary>
        public double PredictedAvailability { get; set; }

        /// <summary>
        /// السعر المتوقع
        /// </summary>
        public decimal PredictedAveragePrice { get; set; }

        /// <summary>
        /// مستوى الثقة في التوقع (0.0 - 1.0)
        /// </summary>
        public double Confidence { get; set; }
    }

    /// <summary>
    /// كلمات مفتاحية للفئة
    /// </summary>
    public class CategoryKeywords
    {
        /// <summary>
        /// الفئة
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// الكلمات المفتاحية
        /// </summary>
        public List<string> Keywords { get; set; } = new();

        /// <summary>
        /// وزن الفئة
        /// </summary>
        public double Weight { get; set; } = 1.0;
    }

    /// <summary>
    /// معلومات حول كيان مفرد للاستخدام في التحديث التلقائي
    /// </summary>
    public class PropertyInfo
    {
        public Guid Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string PropertyTypeId { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> AmenityIds { get; set; } = new();
    }

    /// <summary>
    /// معلومات حول وحدة مفردة للاستخدام في التحديث التلقائي
    /// </summary>
    public class UnitInfo
    {
        public Guid Id { get; set; }
        public Guid? PropertyId { get; set; }
        public decimal? BasePrice { get; set; }
    }

    #endregion
}