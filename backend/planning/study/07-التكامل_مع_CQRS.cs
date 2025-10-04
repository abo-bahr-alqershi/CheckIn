using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using BookN.Core.Services.FastSearch;

namespace BookN.Core.Features.PropertySearch
{
    #region Query Models

    /// <summary>
    /// استعلام البحث المتقدم للكيانات
    /// يدعم جميع أنواع الفلاتر والترتيب والصفحات
    /// </summary>
    public class AdvancedPropertySearchQuery : IRequest<AdvancedPropertySearchResult>
    {
        /// <summary>
        /// المدينة المطلوبة
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// الحد الأدنى للسعر
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// الحد الأقصى للسعر
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// نوع الكيان
        /// </summary>
        public string? PropertyTypeId { get; set; }

        /// <summary>
        /// قائمة معرفات المرافق المطلوبة
        /// </summary>
        public List<string>? AmenityIds { get; set; }

        /// <summary>
        /// تاريخ الوصول
        /// </summary>
        public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// تاريخ المغادرة
        /// </summary>
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// الحد الأدنى للتقييم
        /// </summary>
        public double? MinRating { get; set; }

        /// <summary>
        /// الحد الأقصى للمسافة بالكيلومتر
        /// </summary>
        public int? MaxDistanceKm { get; set; }

        /// <summary>
        /// نقطة الموقع المرجعية للبحث الجغرافي
        /// </summary>
        public GeoPoint? LocationCenter { get; set; }

        /// <summary>
        /// طريقة الترتيب (price, rating, distance, name, popularity)
        /// </summary>
        public string SortBy { get; set; } = "relevance";

        /// <summary>
        /// ترتيب تصاعدي أم تنازلي
        /// </summary>
        public bool IsAscending { get; set; } = false;

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// الحقول المطلوبة في النتيجة (لتحسين الأداء)
        /// </summary>
        public List<string>? IncludeFields { get; set; }

        /// <summary>
        /// فلاتر الحقول الديناميكية
        /// JSON بصيغة: {"FieldId": ["Value1", "Value2"]}
        /// </summary>
        public Dictionary<string, List<string>>? DynamicFilters { get; set; }

        /// <summary>
        /// فلاتر الحقول الأساسية
        /// JSON بصيغة: {"FieldId": ["Value1", "Value2"]}
        /// </summary>
        public Dictionary<string, List<string>>? PrimaryFilters { get; set; }

        /// <summary>
        /// معرف المستخدم للتخصيص والتوصيات
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// تضمين الإحصائيات في النتيجة
        /// </summary>
        public bool IncludeStatistics { get; set; } = false;

        /// <summary>
        /// تضمين الفلاتر المتاحة في النتيجة
        /// </summary>
        public bool IncludeAvailableFilters { get; set; } = false;
    }

    /// <summary>
    /// استعلام البحث النصي السريع
    /// </summary>
    public class FastTextSearchQuery : IRequest<FastTextSearchResult>
    {
        /// <summary>
        /// نص البحث
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// العدد الأقصى للنتائج
        /// </summary>
        public int MaxResults { get; set; } = 50;

        /// <summary>
        /// اللغة المطلوبة
        /// </summary>
        public string Language { get; set; } = "ar";

        /// <summary>
        /// فلاتر إضافية للبحث النصي
        /// </summary>
        public SearchCriteria? AdditionalFilters { get; set; }

        /// <summary>
        /// تضمين اقتراحات التصحيح
        /// </summary>
        public bool IncludeCorrections { get; set; } = true;

        /// <summary>
        /// تضمين الكلمات المميزة في النتيجة
        /// </summary>
        public bool HighlightMatches { get; set; } = true;
    }

    /// <summary>
    /// استعلام البحث بالموقع الجغرافي
    /// </summary>
    public class LocationBasedSearchQuery : IRequest<LocationSearchResult>
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
        /// نطاق البحث بالكيلومتر
        /// </summary>
        public double RadiusKm { get; set; } = 10.0;

        /// <summary>
        /// فلاتر إضافية للبحث الجغرافي
        /// </summary>
        public SearchCriteria? AdditionalCriteria { get; set; }

        /// <summary>
        /// العدد الأقصى للنتائج
        /// </summary>
        public int MaxResults { get; set; } = 100;

        /// <summary>
        /// ترتيب حسب المسافة
        /// </summary>
        public bool SortByDistance { get; set; } = true;
    }

    /// <summary>
    /// استعلام اقتراحات البحث التلقائية
    /// </summary>
    public class SearchSuggestionsQuery : IRequest<SearchSuggestionsResult>
    {
        /// <summary>
        /// النص الجزئي للبحث
        /// </summary>
        public string PartialQuery { get; set; } = string.Empty;

        /// <summary>
        /// العدد الأقصى للاقتراحات
        /// </summary>
        public int MaxSuggestions { get; set; } = 10;

        /// <summary>
        /// نوع الاقتراحات (cities, properties, amenities, all)
        /// </summary>
        public string SuggestionType { get; set; } = "all";

        /// <summary>
        /// معرف المستخدم للاقتراحات الشخصية
        /// </summary>
        public string? UserId { get; set; }
    }

    /// <summary>
    /// استعلام الفلاتر المتاحة
    /// </summary>
    public class AvailableFiltersQuery : IRequest<AvailableFiltersResult>
    {
        /// <summary>
        /// المعايير الحالية للبحث
        /// </summary>
        public SearchCriteria CurrentCriteria { get; set; } = new();

        /// <summary>
        /// تضمين عدد النتائج لكل فلتر
        /// </summary>
        public bool IncludeCounts { get; set; } = true;

        /// <summary>
        /// الفلاتر المطلوبة فقط
        /// </summary>
        public List<string>? RequiredFilterTypes { get; set; }
    }

    #endregion

    #region Result Models

    /// <summary>
    /// نتيجة البحث المتقدم للكيانات
    /// </summary>
    public class AdvancedPropertySearchResult
    {
        /// <summary>
        /// قائمة الكيانات المطابقة
        /// </summary>
        public List<PropertySearchItem> Properties { get; set; } = new();

        /// <summary>
        /// العدد الإجمالي للنتائج
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// رقم الصفحة الحالية
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// العدد الإجمالي للصفحات
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// هل توجد صفحة تالية
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// هل توجد صفحة سابقة
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// وقت تنفيذ البحث بالميللي ثانية
        /// </summary>
        public long SearchTimeMs { get; set; }

        /// <summary>
        /// الفلاتر المطبقة
        /// </summary>
        public Dictionary<string, object> AppliedFilters { get; set; } = new();

        /// <summary>
        /// إحصائيات البحث
        /// </summary>
        public SearchStatistics? Statistics { get; set; }

        /// <summary>
        /// الفلاتر المتاحة للتحديد
        /// </summary>
        public AvailableFilters? AvailableFilters { get; set; }

        /// <summary>
        /// معلومات إضافية للتشخيص
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// نتيجة البحث النصي السريع
    /// </summary>
    public class FastTextSearchResult
    {
        /// <summary>
        /// نتائج البحث النصي
        /// </summary>
        public List<TextSearchResultItem> Results { get; set; } = new();

        /// <summary>
        /// العدد الإجمالي للمطابقات
        /// </summary>
        public int TotalMatches { get; set; }

        /// <summary>
        /// الكلمات المستخدمة في البحث
        /// </summary>
        public List<string> SearchTerms { get; set; } = new();

        /// <summary>
        /// اقتراحات التصحيح
        /// </summary>
        public List<string> SuggestedCorrections { get; set; } = new();

        /// <summary>
        /// وقت البحث بالميللي ثانية
        /// </summary>
        public long SearchTimeMs { get; set; }

        /// <summary>
        /// هل تم العثور على نتائج دقيقة
        /// </summary>
        public bool HasExactMatches { get; set; }
    }

    /// <summary>
    /// نتيجة اقتراحات البحث
    /// </summary>
    public class SearchSuggestionsResult
    {
        /// <summary>
        /// قائمة الاقتراحات
        /// </summary>
        public List<SearchSuggestion> Suggestions { get; set; } = new();

        /// <summary>
        /// الاقتراحات الشخصية للمستخدم
        /// </summary>
        public List<SearchSuggestion> PersonalizedSuggestions { get; set; } = new();

        /// <summary>
        /// الاقتراحات الشائعة
        /// </summary>
        public List<SearchSuggestion> PopularSuggestions { get; set; } = new();

        /// <summary>
        /// وقت الاستعلام بالميللي ثانية
        /// </summary>
        public long ResponseTimeMs { get; set; }
    }

    /// <summary>
    /// نتيجة الفلاتر المتاحة
    /// </summary>
    public class AvailableFiltersResult
    {
        /// <summary>
        /// الفلاتر المتاحة
        /// </summary>
        public AvailableFilters Filters { get; set; } = new();

        /// <summary>
        /// العدد الإجمالي للنتائج مع الفلاتر الحالية
        /// </summary>
        public int TotalResultsWithCurrentFilters { get; set; }

        /// <summary>
        /// وقت الاستعلام بالميللي ثانية
        /// </summary>
        public long ResponseTimeMs { get; set; }
    }

    #endregion

    #region Item Models

    /// <summary>
    /// عنصر نتيجة البحث للكيان
    /// </summary>
    public class PropertySearchItem
    {
        /// <summary>
        /// معرف الكيان
        /// </summary>
        public string PropertyId { get; set; } = string.Empty;

        /// <summary>
        /// اسم الكيان
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وصف مختصر
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// المدينة
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// العنوان الكامل
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// متوسط السعر
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
        /// متوسط التقييم
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// عدد التقييمات
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// تقييم النجوم
        /// </summary>
        public int StarRating { get; set; }

        /// <summary>
        /// الصورة الرئيسية
        /// </summary>
        public string MainImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// قائمة الصور
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// المرافق المتاحة
        /// </summary>
        public List<string> Amenities { get; set; } = new();

        /// <summary>
        /// الموقع الجغرافي
        /// </summary>
        public GeoPoint? Location { get; set; }

        /// <summary>
        /// المسافة من نقطة البحث (إذا كان البحث جغرافياً)
        /// </summary>
        public double? DistanceKm { get; set; }

        /// <summary>
        /// نص المسافة المنسق
        /// </summary>
        public string? DistanceText { get; set; }

        /// <summary>
        /// الحقول الديناميكية المعروضة
        /// </summary>
        public Dictionary<string, object> DynamicFields { get; set; } = new();

        /// <summary>
        /// درجة الصلة (للبحث النصي)
        /// </summary>
        public double RelevanceScore { get; set; }

        /// <summary>
        /// هل الكيان متاح في التواريخ المطلوبة
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// معلومات التوفر
        /// </summary>
        public AvailabilityInfo? Availability { get; set; }

        /// <summary>
        /// عروض خاصة إن وجدت
        /// </summary>
        public List<SpecialOffer> SpecialOffers { get; set; } = new();
    }

    /// <summary>
    /// عنصر نتيجة البحث النصي
    /// </summary>
    public class TextSearchResultItem
    {
        /// <summary>
        /// معرف الكيان
        /// </summary>
        public string PropertyId { get; set; } = string.Empty;

        /// <summary>
        /// اسم الكيان مع تمييز الكلمات المطابقة
        /// </summary>
        public string HighlightedName { get; set; } = string.Empty;

        /// <summary>
        /// الوصف مع تمييز الكلمات المطابقة
        /// </summary>
        public string HighlightedDescription { get; set; } = string.Empty;

        /// <summary>
        /// درجة الصلة
        /// </summary>
        public double RelevanceScore { get; set; }

        /// <summary>
        /// الكلمات المطابقة
        /// </summary>
        public List<string> MatchedTerms { get; set; } = new();

        /// <summary>
        /// نوع المطابقة (exact, partial, synonym)
        /// </summary>
        public string MatchType { get; set; } = string.Empty;

        /// <summary>
        /// معلومات الكيان الأساسية
        /// </summary>
        public PropertySearchItem PropertyInfo { get; set; } = new();
    }

    /// <summary>
    /// اقتراح البحث
    /// </summary>
    public class SearchSuggestion
    {
        /// <summary>
        /// النص المقترح
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// نوع الاقتراح (city, property, amenity, category)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// عدد النتائج المتوقعة
        /// </summary>
        public int ExpectedResults { get; set; }

        /// <summary>
        /// درجة الشيوع
        /// </summary>
        public int Popularity { get; set; }

        /// <summary>
        /// معرف الكيان إن وجد
        /// </summary>
        public string? EntityId { get; set; }

        /// <summary>
        /// معلومات إضافية
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// معلومات التوفر
    /// </summary>
    public class AvailabilityInfo
    {
        /// <summary>
        /// هل متاح في التواريخ المطلوبة
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// عدد الوحدات المتاحة
        /// </summary>
        public int AvailableUnits { get; set; }

        /// <summary>
        /// أقرب تاريخ متاح
        /// </summary>
        public DateTime? NextAvailableDate { get; set; }

        /// <summary>
        /// رسالة التوفر
        /// </summary>
        public string? AvailabilityMessage { get; set; }
    }

    /// <summary>
    /// عرض خاص
    /// </summary>
    public class SpecialOffer
    {
        /// <summary>
        /// اسم العرض
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف العرض
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// نسبة الخصم
        /// </summary>
        public decimal? DiscountPercentage { get; set; }

        /// <summary>
        /// مبلغ الخصم
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// تاريخ انتهاء العرض
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// شروط العرض
        /// </summary>
        public string? Terms { get; set; }
    }

    #endregion

    #region Query Handlers

    /// <summary>
    /// معالج استعلام البحث المتقدم
    /// </summary>
    public class AdvancedPropertySearchQueryHandler : IRequestHandler<AdvancedPropertySearchQuery, AdvancedPropertySearchResult>
    {
        private readonly ILogger<AdvancedPropertySearchQueryHandler> _logger;
        private readonly IFastSearchService _searchService;

        public AdvancedPropertySearchQueryHandler(
            ILogger<AdvancedPropertySearchQueryHandler> logger,
            IFastSearchService searchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        public async Task<AdvancedPropertySearchResult> Handle(AdvancedPropertySearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("تنفيذ البحث المتقدم للكيانات: {@Request}", request);

                // تحويل معايير البحث
                var searchCriteria = MapToSearchCriteria(request);

                // تنفيذ البحث السريع
                var searchResult = await _searchService.SearchAsync(searchCriteria);

                // تحميل الإحصائيات إذا كانت مطلوبة
                SearchStatistics? statistics = null;
                if (request.IncludeStatistics)
                {
                    statistics = await _searchService.GetSearchStatisticsAsync(searchCriteria);
                }

                // تحميل الفلاتر المتاحة إذا كانت مطلوبة
                AvailableFilters? availableFilters = null;
                if (request.IncludeAvailableFilters)
                {
                    availableFilters = await _searchService.GetAvailableFiltersAsync(searchCriteria);
                }

                // تحويل النتيجة
                var result = MapToAdvancedResult(searchResult, request, statistics, availableFilters);

                _logger.LogInformation("تم الانتهاء من البحث المتقدم، النتائج: {Count}, الوقت: {Time}ms", 
                    result.TotalCount, result.SearchTimeMs);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ البحث المتقدم للكيانات");
                throw;
            }
        }

        /// <summary>
        /// تحويل طلب البحث إلى معايير البحث الداخلية
        /// </summary>
        private SearchCriteria MapToSearchCriteria(AdvancedPropertySearchQuery request)
        {
            return new SearchCriteria
            {
                City = request.City,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                PropertyTypeId = request.PropertyTypeId,
                AmenityIds = request.AmenityIds,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                MinRating = request.MinRating,
                MaxDistance = request.MaxDistanceKm,
                LocationCenter = request.LocationCenter,
                SortBy = request.SortBy,
                IsAscending = request.IsAscending,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        /// <summary>
        /// تحويل نتيجة البحث الداخلية إلى نتيجة متقدمة
        /// </summary>
        private AdvancedPropertySearchResult MapToAdvancedResult(
            SearchResult searchResult,
            AdvancedPropertySearchQuery request,
            SearchStatistics? statistics,
            AvailableFilters? availableFilters)
        {
            var properties = searchResult.Properties.Select(MapToPropertySearchItem).ToList();

            return new AdvancedPropertySearchResult
            {
                Properties = properties,
                TotalCount = searchResult.TotalCount,
                CurrentPage = searchResult.PageNumber,
                PageSize = searchResult.PageSize,
                TotalPages = (int)Math.Ceiling((double)searchResult.TotalCount / searchResult.PageSize),
                HasNextPage = searchResult.HasNextPage,
                HasPreviousPage = searchResult.PageNumber > 1,
                SearchTimeMs = searchResult.SearchTimeMs,
                AppliedFilters = searchResult.AppliedFilters,
                Statistics = statistics,
                AvailableFilters = availableFilters,
                Metadata = new Dictionary<string, object>
                {
                    ["searchId"] = Guid.NewGuid().ToString(),
                    ["timestamp"] = DateTime.UtcNow,
                    ["userId"] = request.UserId,
                    ["cacheHit"] = searchResult.SearchTimeMs < 10 // تخمين بناء على السرعة
                }
            };
        }

        /// <summary>
        /// تحويل ملخص الكيان إلى عنصر نتيجة البحث
        /// </summary>
        private PropertySearchItem MapToPropertySearchItem(PropertySummary summary)
        {
            return new PropertySearchItem
            {
                PropertyId = summary.PropertyId.ToString(),
                Name = summary.Name,
                Description = summary.Description,
                City = summary.City,
                AveragePrice = summary.AveragePrice,
                MinPrice = summary.AveragePrice, // يمكن تحسينها
                MaxPrice = summary.AveragePrice, // يمكن تحسينها
                AverageRating = summary.Rating,
                MainImageUrl = summary.ImageUrl,
                Amenities = summary.Amenities,
                Location = summary.Location,
                IsAvailable = true, // يمكن تحسينها
                RelevanceScore = 1.0 // يمكن تحسينها
            };
        }
    }

    /// <summary>
    /// معالج استعلام البحث النصي السريع
    /// </summary>
    public class FastTextSearchQueryHandler : IRequestHandler<FastTextSearchQuery, FastTextSearchResult>
    {
        private readonly ILogger<FastTextSearchQueryHandler> _logger;
        private readonly IFastSearchService _searchService;

        public FastTextSearchQueryHandler(
            ILogger<FastTextSearchQueryHandler> logger,
            IFastSearchService searchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        public async Task<FastTextSearchResult> Handle(FastTextSearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("تنفيذ البحث النصي السريع: {Query}", request.SearchText);

                // تنفيذ البحث النصي
                var searchResult = await _searchService.TextSearchAsync(request.SearchText, request.MaxResults);

                // تحويل النتيجة
                var result = new FastTextSearchResult
                {
                    Results = searchResult.Results.Select(MapToTextSearchResultItem).ToList(),
                    TotalMatches = searchResult.TotalCount,
                    SearchTerms = searchResult.SearchTerms,
                    SuggestedCorrections = request.IncludeCorrections ? searchResult.SuggestedCorrections : new List<string>(),
                    SearchTimeMs = searchResult.SearchTimeMs,
                    HasExactMatches = searchResult.Results.Any(r => r.RelevanceScore > 0.9)
                };

                _logger.LogInformation("تم الانتهاء من البحث النصي، النتائج: {Count}, الوقت: {Time}ms", 
                    result.TotalMatches, result.SearchTimeMs);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ البحث النصي السريع للاستعلام: {Query}", request.SearchText);
                throw;
            }
        }

        /// <summary>
        /// تحويل عنصر البحث النصي الداخلي إلى عنصر نتيجة
        /// </summary>
        private TextSearchResultItem MapToTextSearchResultItem(TextSearchItem item)
        {
            return new TextSearchResultItem
            {
                PropertyId = item.PropertyId.ToString(),
                HighlightedName = item.Property?.Name ?? "",
                HighlightedDescription = item.Property?.Description ?? "",
                RelevanceScore = item.RelevanceScore,
                MatchedTerms = item.MatchedTerms,
                MatchType = item.RelevanceScore > 0.9 ? "exact" : item.RelevanceScore > 0.5 ? "partial" : "synonym",
                PropertyInfo = item.Property != null ? MapToPropertySearchItem(item.Property) : new PropertySearchItem()
            };
        }

        /// <summary>
        /// تحويل ملخص الكيان إلى عنصر نتيجة البحث
        /// </summary>
        private PropertySearchItem MapToPropertySearchItem(PropertySummary summary)
        {
            return new PropertySearchItem
            {
                PropertyId = summary.PropertyId.ToString(),
                Name = summary.Name,
                Description = summary.Description,
                City = summary.City,
                AveragePrice = summary.AveragePrice,
                AverageRating = summary.Rating,
                MainImageUrl = summary.ImageUrl,
                Amenities = summary.Amenities,
                Location = summary.Location
            };
        }
    }

    /// <summary>
    /// معالج استعلام البحث بالموقع الجغرافي
    /// </summary>
    public class LocationBasedSearchQueryHandler : IRequestHandler<LocationBasedSearchQuery, LocationSearchResult>
    {
        private readonly ILogger<LocationBasedSearchQueryHandler> _logger;
        private readonly IFastSearchService _searchService;

        public LocationBasedSearchQueryHandler(
            ILogger<LocationBasedSearchQueryHandler> logger,
            IFastSearchService searchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        public async Task<LocationSearchResult> Handle(LocationBasedSearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("تنفيذ البحث الجغرافي: {Lat}, {Lng}, نطاق {Radius}كم", 
                    request.Latitude, request.Longitude, request.RadiusKm);

                // تنفيذ البحث الجغرافي
                var searchResult = await _searchService.SearchByLocationAsync(
                    request.Latitude, 
                    request.Longitude, 
                    request.RadiusKm, 
                    request.AdditionalCriteria);

                _logger.LogInformation("تم الانتهاء من البحث الجغرافي، النتائج: {Count}, الوقت: {Time}ms", 
                    searchResult.TotalCount, searchResult.SearchTimeMs);

                return searchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ البحث الجغرافي");
                throw;
            }
        }
    }

    /// <summary>
    /// معالج استعلام اقتراحات البحث
    /// </summary>
    public class SearchSuggestionsQueryHandler : IRequestHandler<SearchSuggestionsQuery, SearchSuggestionsResult>
    {
        private readonly ILogger<SearchSuggestionsQueryHandler> _logger;
        private readonly IFastSearchService _searchService;

        public SearchSuggestionsQueryHandler(
            ILogger<SearchSuggestionsQueryHandler> logger,
            IFastSearchService searchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        public async Task<SearchSuggestionsResult> Handle(SearchSuggestionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("تنفيذ استعلام اقتراحات البحث: {Query}", request.PartialQuery);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // الحصول على الاقتراحات
                var suggestions = await _searchService.GetSearchSuggestionsAsync(request.PartialQuery, request.MaxSuggestions);

                // تحويل النتيجة
                var result = new SearchSuggestionsResult
                {
                    Suggestions = suggestions.Select(s => new SearchSuggestion
                    {
                        Text = s,
                        Type = "general",
                        ExpectedResults = 0, // يمكن تحسينها
                        Popularity = 0 // يمكن تحسينها
                    }).ToList(),
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ استعلام اقتراحات البحث للاستعلام: {Query}", request.PartialQuery);
                throw;
            }
        }
    }

    /// <summary>
    /// معالج استعلام الفلاتر المتاحة
    /// </summary>
    public class AvailableFiltersQueryHandler : IRequestHandler<AvailableFiltersQuery, AvailableFiltersResult>
    {
        private readonly ILogger<AvailableFiltersQueryHandler> _logger;
        private readonly IFastSearchService _searchService;

        public AvailableFiltersQueryHandler(
            ILogger<AvailableFiltersQueryHandler> logger,
            IFastSearchService searchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        public async Task<AvailableFiltersResult> Handle(AvailableFiltersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("تنفيذ استعلام الفلاتر المتاحة");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // الحصول على الفلاتر المتاحة
                var availableFilters = await _searchService.GetAvailableFiltersAsync(request.CurrentCriteria);

                // حساب العدد الإجمالي للنتائج مع الفلاتر الحالية
                var totalResults = 0;
                if (request.IncludeCounts)
                {
                    var searchResult = await _searchService.SearchAsync(request.CurrentCriteria);
                    totalResults = searchResult.TotalCount;
                }

                var result = new AvailableFiltersResult
                {
                    Filters = availableFilters,
                    TotalResultsWithCurrentFilters = totalResults,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ استعلام الفلاتر المتاحة");
                throw;
            }
        }
    }

    #endregion
}