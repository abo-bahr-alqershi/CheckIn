using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using BookN.Core.Services.IndexManagement;

namespace BookN.Core.Services.FastSearch
{
    /// <summary>
    /// خدمة البحث السريع باستخدام الفهارس المسبقة
    /// تتولى تطبيق الفلاتر المعقدة بسرعة فائقة
    /// </summary>
    public interface IFastSearchService
    {
        /// <summary>
        /// البحث السريع باستخدام معايير متعددة
        /// </summary>
        Task<SearchResult> SearchAsync(SearchCriteria criteria);

        /// <summary>
        /// البحث النصي السريع
        /// </summary>
        Task<TextSearchResult> TextSearchAsync(string query, int maxResults = 50);

        /// <summary>
        /// الحصول على اقتراحات البحث
        /// </summary>
        Task<List<string>> GetSearchSuggestionsAsync(string partialQuery, int maxSuggestions = 10);

        /// <summary>
        /// البحث بالموقع الجغرافي
        /// </summary>
        Task<LocationSearchResult> SearchByLocationAsync(double latitude, double longitude, double radiusKm, SearchCriteria? additionalCriteria = null);

        /// <summary>
        /// الحصول على فلاتر متاحة للمعايير الحالية
        /// </summary>
        Task<AvailableFilters> GetAvailableFiltersAsync(SearchCriteria criteria);

        /// <summary>
        /// إحصائيات البحث للمعايير المحددة
        /// </summary>
        Task<SearchStatistics> GetSearchStatisticsAsync(SearchCriteria criteria);
    }

    public class FastSearchService : IFastSearchService
    {
        private readonly ILogger<FastSearchService> _logger;
        private readonly IJsonIndexFileService _fileService;
        private readonly IMemoryCache _cache;
        private readonly SearchConfiguration _config;

        public FastSearchService(
            ILogger<FastSearchService> logger,
            IJsonIndexFileService fileService,
            IMemoryCache cache,
            SearchConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// البحث السريع الرئيسي مع دعم جميع أنواع الفلاتر
        /// </summary>
        public async Task<SearchResult> SearchAsync(SearchCriteria criteria)
        {
            try
            {
                _logger.LogDebug("بدء البحث السريع بالمعايير: {@Criteria}", criteria);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // التحقق من وجود نتائج في الكاش
                var cacheKey = GenerateSearchCacheKey(criteria);
                if (_cache.TryGetValue(cacheKey, out SearchResult? cachedResult))
                {
                    _logger.LogDebug("تم استرجاع النتائج من الكاش في {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                    return cachedResult!;
                }

                // تنفيذ البحث باستخدام خوارزمية التقاطع
                var propertyIds = await ExecuteIntersectionSearchAsync(criteria);

                // تطبيق الترتيب والصفحات
                var sortedIds = await ApplySortingAsync(propertyIds, criteria.SortBy, criteria.IsAscending);
                var pagedIds = ApplyPaging(sortedIds, criteria.PageNumber, criteria.PageSize);

                // تحميل البيانات الأساسية للكيانات
                var properties = await LoadPropertyDetailsAsync(pagedIds);

                // إنشاء النتيجة النهائية
                var result = new SearchResult
                {
                    Properties = properties,
                    TotalCount = propertyIds.Count,
                    PageNumber = criteria.PageNumber,
                    PageSize = criteria.PageSize,
                    SearchTimeMs = stopwatch.ElapsedMilliseconds,
                    HasNextPage = (criteria.PageNumber * criteria.PageSize) < propertyIds.Count,
                    AppliedFilters = GetAppliedFiltersInfo(criteria)
                };

                // حفظ في الكاش
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.SearchCacheExpiryMinutes),
                    Priority = CacheItemPriority.Normal
                };
                _cache.Set(cacheKey, result, cacheOptions);

                _logger.LogDebug("تم الانتهاء من البحث في {ElapsedMs}ms، النتائج: {Count}", 
                    stopwatch.ElapsedMilliseconds, result.TotalCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنفيذ البحث السريع");
                throw;
            }
        }

        /// <summary>
        /// البحث النصي السريع باستخدام هيكل Trie
        /// </summary>
        public async Task<TextSearchResult> TextSearchAsync(string query, int maxResults = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return new TextSearchResult { Results = new List<TextSearchItem>(), TotalCount = 0 };
                }

                _logger.LogDebug("بدء البحث النصي للاستعلام: {Query}", query);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // تحليل الاستعلام وتنظيفه
                var searchTerms = ProcessSearchQuery(query);

                // تحميل فهرس البحث النصي
                var textIndex = await _fileService.ReadIndexAsync<TextSearchIndex>("text-search", "trie_structure");
                if (textIndex == null)
                {
                    _logger.LogWarning("فهرس البحث النصي غير متوفر");
                    return new TextSearchResult { Results = new List<TextSearchItem>(), TotalCount = 0 };
                }

                // البحث في الـ Trie للعثور على التطابقات
                var matchedProperties = new Dictionary<int, double>(); // PropertyId -> Score

                foreach (var term in searchTerms)
                {
                    var matches = SearchInTrie(textIndex.Trie, term);
                    foreach (var match in matches)
                    {
                        var score = CalculateRelevanceScore(term, match.Word, match.PropertyIds.Count);
                        foreach (var propertyId in match.PropertyIds)
                        {
                            if (matchedProperties.ContainsKey(propertyId))
                            {
                                matchedProperties[propertyId] += score;
                            }
                            else
                            {
                                matchedProperties[propertyId] = score;
                            }
                        }
                    }

                    // البحث في المرادفات
                    if (textIndex.Synonyms.TryGetValue(term, out var synonyms))
                    {
                        foreach (var synonym in synonyms)
                        {
                            var synonymMatches = SearchInTrie(textIndex.Trie, synonym);
                            foreach (var match in synonymMatches)
                            {
                                var synonymScore = CalculateRelevanceScore(synonym, match.Word, match.PropertyIds.Count) * 0.8; // خصم للمرادفات
                                foreach (var propertyId in match.PropertyIds)
                                {
                                    if (matchedProperties.ContainsKey(propertyId))
                                    {
                                        matchedProperties[propertyId] += synonymScore;
                                    }
                                    else
                                    {
                                        matchedProperties[propertyId] = synonymScore;
                                    }
                                }
                            }
                        }
                    }
                }

                // ترتيب النتائج حسب الصلة
                var sortedResults = matchedProperties
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(maxResults)
                    .ToList();

                // تحميل تفاصيل الكيانات
                var propertyDetails = await LoadPropertyDetailsAsync(sortedResults.Select(r => r.Key).ToList());

                var results = sortedResults.Select(r => new TextSearchItem
                {
                    PropertyId = r.Key,
                    Property = propertyDetails.FirstOrDefault(p => p.PropertyId == r.Key),
                    RelevanceScore = r.Value,
                    MatchedTerms = GetMatchedTermsForProperty(r.Key, searchTerms, textIndex)
                }).ToList();

                var textSearchResult = new TextSearchResult
                {
                    Results = results,
                    TotalCount = matchedProperties.Count,
                    SearchTimeMs = stopwatch.ElapsedMilliseconds,
                    SearchTerms = searchTerms,
                    SuggestedCorrections = await GetSearchCorrectionsAsync(query, textIndex)
                };

                _logger.LogDebug("تم الانتهاء من البحث النصي في {ElapsedMs}ms، النتائج: {Count}", 
                    stopwatch.ElapsedMilliseconds, results.Count);

                return textSearchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث النصي للاستعلام: {Query}", query);
                throw;
            }
        }

        /// <summary>
        /// الحصول على اقتراحات البحث بناء على الإدخال الجزئي
        /// </summary>
        public async Task<List<string>> GetSearchSuggestionsAsync(string partialQuery, int maxSuggestions = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partialQuery) || partialQuery.Length < 2)
                {
                    return new List<string>();
                }

                var cacheKey = $"suggestions:{partialQuery.ToLowerInvariant()}";
                if (_cache.TryGetValue(cacheKey, out List<string>? cachedSuggestions))
                {
                    return cachedSuggestions!;
                }

                var textIndex = await _fileService.ReadIndexAsync<TextSearchIndex>("text-search", "trie_structure");
                if (textIndex == null)
                {
                    return new List<string>();
                }

                var suggestions = new List<string>();

                // البحث في الكلمات الشائعة
                var popularMatches = textIndex.PopularTerms
                    .Where(kvp => kvp.Key.StartsWith(partialQuery.ToLowerInvariant()))
                    .OrderByDescending(kvp => kvp.Value.Frequency)
                    .Take(maxSuggestions)
                    .Select(kvp => kvp.Key)
                    .ToList();

                suggestions.AddRange(popularMatches);

                // البحث في الـ Trie للحصول على تطابقات إضافية
                if (suggestions.Count < maxSuggestions)
                {
                    var trieMatches = GetTrieCompletions(textIndex.Trie, partialQuery.ToLowerInvariant())
                        .Where(term => !suggestions.Contains(term))
                        .Take(maxSuggestions - suggestions.Count);

                    suggestions.AddRange(trieMatches);
                }

                // حفظ في الكاش
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                    Priority = CacheItemPriority.Low
                };
                _cache.Set(cacheKey, suggestions, cacheOptions);

                return suggestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على اقتراحات البحث للاستعلام: {Query}", partialQuery);
                return new List<string>();
            }
        }

        /// <summary>
        /// البحث بالموقع الجغرافي مع فلاتر إضافية
        /// </summary>
        public async Task<LocationSearchResult> SearchByLocationAsync(
            double latitude, 
            double longitude, 
            double radiusKm, 
            SearchCriteria? additionalCriteria = null)
        {
            try
            {
                _logger.LogDebug("بدء البحث بالموقع: {Lat}, {Lng}, نطاق {Radius}كم", latitude, longitude, radiusKm);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // البحث الأولي باستخدام الفلاتر الإضافية
                List<int> basePropertyIds;
                if (additionalCriteria != null)
                {
                    basePropertyIds = await ExecuteIntersectionSearchAsync(additionalCriteria);
                }
                else
                {
                    // الحصول على جميع الكيانات النشطة
                    basePropertyIds = await GetAllActivePropertyIdsAsync();
                }

                // تطبيق فلتر الموقع الجغرافي
                var propertiesWithLocation = await LoadPropertiesWithLocationAsync(basePropertyIds);
                var nearbyProperties = FilterByLocation(propertiesWithLocation, latitude, longitude, radiusKm);

                // ترتيب حسب المسافة
                var sortedByDistance = nearbyProperties
                    .OrderBy(p => p.DistanceKm)
                    .ToList();

                var result = new LocationSearchResult
                {
                    Properties = sortedByDistance.Take(100).ToList(), // حد أقصى 100 نتيجة
                    TotalCount = nearbyProperties.Count,
                    SearchTimeMs = stopwatch.ElapsedMilliseconds,
                    SearchCenter = new GeoPoint { Latitude = latitude, Longitude = longitude },
                    SearchRadiusKm = radiusKm,
                    AverageDistance = nearbyProperties.Any() ? nearbyProperties.Average(p => p.DistanceKm) : 0
                };

                _logger.LogDebug("تم الانتهاء من البحث بالموقع في {ElapsedMs}ms، النتائج: {Count}", 
                    stopwatch.ElapsedMilliseconds, result.TotalCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث بالموقع");
                throw;
            }
        }

        /// <summary>
        /// الحصول على الفلاتر المتاحة للمعايير الحالية
        /// </summary>
        public async Task<AvailableFilters> GetAvailableFiltersAsync(SearchCriteria criteria)
        {
            try
            {
                _logger.LogDebug("الحصول على الفلاتر المتاحة للمعايير: {@Criteria}", criteria);

                // الحصول على الكيانات التي تطابق المعايير الحالية
                var matchingPropertyIds = await ExecuteIntersectionSearchAsync(criteria);

                // تحليل البيانات لاستخراج الفلاتر المتاحة
                var availableFilters = new AvailableFilters
                {
                    AvailableCities = await GetAvailableCitiesAsync(matchingPropertyIds),
                    AvailablePriceRanges = await GetAvailablePriceRangesAsync(matchingPropertyIds),
                    AvailableAmenities = await GetAvailableAmenitiesAsync(matchingPropertyIds),
                    AvailablePropertyTypes = await GetAvailablePropertyTypesAsync(matchingPropertyIds),
                    AvailableRatingRanges = await GetAvailableRatingRangesAsync(matchingPropertyIds)
                };

                return availableFilters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على الفلاتر المتاحة");
                throw;
            }
        }

        /// <summary>
        /// الحصول على إحصائيات البحث
        /// </summary>
        public async Task<SearchStatistics> GetSearchStatisticsAsync(SearchCriteria criteria)
        {
            try
            {
                var propertyIds = await ExecuteIntersectionSearchAsync(criteria);
                
                if (!propertyIds.Any())
                {
                    return new SearchStatistics();
                }

                // تحميل البيانات الأساسية لحساب الإحصائيات
                var properties = await LoadPropertyDetailsAsync(propertyIds.Take(1000).ToList()); // عينة للإحصائيات

                var statistics = new SearchStatistics
                {
                    TotalResults = propertyIds.Count,
                    AveragePrice = properties.Where(p => p.AveragePrice > 0).Average(p => p.AveragePrice),
                    MinPrice = properties.Where(p => p.AveragePrice > 0).Min(p => p.AveragePrice),
                    MaxPrice = properties.Where(p => p.AveragePrice > 0).Max(p => p.AveragePrice),
                    AverageRating = properties.Where(p => p.Rating > 0).Average(p => p.Rating),
                    TopCities = properties.GroupBy(p => p.City)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    PopularAmenities = await GetPopularAmenitiesForPropertiesAsync(propertyIds.Take(100).ToList())
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حساب إحصائيات البحث");
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// تنفيذ البحث باستخدام خوارزمية التقاطع
        /// </summary>
        private async Task<List<int>> ExecuteIntersectionSearchAsync(SearchCriteria criteria)
        {
            var filterSets = new List<HashSet<int>>();

            // فلتر المدينة
            if (!string.IsNullOrEmpty(criteria.City))
            {
                var cityIndex = await _fileService.ReadIndexAsync<CityIndex>("cities", criteria.City.ToLowerInvariant());
                if (cityIndex != null)
                {
                    var cityProperties = ExtractPropertyIdsFromCityIndex(cityIndex, criteria);
                    if (cityProperties.Any())
                    {
                        filterSets.Add(new HashSet<int>(cityProperties));
                    }
                }
            }

            // فلتر نطاق السعر
            if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
            {
                var priceRangeProperties = await GetPropertiesInPriceRangeAsync(criteria.MinPrice, criteria.MaxPrice);
                if (priceRangeProperties.Any())
                {
                    filterSets.Add(new HashSet<int>(priceRangeProperties));
                }
            }

            // فلتر المرافق
            if (criteria.AmenityIds?.Any() == true)
            {
                var amenityProperties = await GetPropertiesWithAmenitiesAsync(criteria.AmenityIds);
                if (amenityProperties.Any())
                {
                    filterSets.Add(new HashSet<int>(amenityProperties));
                }
            }

            // فلتر نوع الكيان
            if (!string.IsNullOrEmpty(criteria.PropertyTypeId))
            {
                var typeProperties = await GetPropertiesByTypeAsync(criteria.PropertyTypeId);
                if (typeProperties.Any())
                {
                    filterSets.Add(new HashSet<int>(typeProperties));
                }
            }

            // فلتر التوفر
            if (criteria.CheckInDate.HasValue && criteria.CheckOutDate.HasValue)
            {
                var availableProperties = await GetAvailablePropertiesAsync(criteria.CheckInDate.Value, criteria.CheckOutDate.Value);
                if (availableProperties.Any())
                {
                    filterSets.Add(new HashSet<int>(availableProperties));
                }
            }

            // تطبيق التقاطع
            if (!filterSets.Any())
            {
                return await GetAllActivePropertyIdsAsync();
            }

            var result = filterSets.First();
            foreach (var filterSet in filterSets.Skip(1))
            {
                result.IntersectWith(filterSet);
            }

            return result.ToList();
        }

        /// <summary>
        /// استخراج معرفات الكيانات من فهرس المدينة
        /// </summary>
        private List<int> ExtractPropertyIdsFromCityIndex(CityIndex cityIndex, SearchCriteria criteria)
        {
            var propertyIds = new List<int>();

            // إذا كان هناك نطاق سعري محدد
            if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
            {
                foreach (var priceRange in cityIndex.PriceRanges)
                {
                    if (IsPriceRangeMatch(priceRange.Key, criteria.MinPrice, criteria.MaxPrice))
                    {
                        propertyIds.AddRange(priceRange.Value.PropertyIds);
                    }
                }
            }
            else
            {
                // إضافة جميع الكيانات في المدينة
                foreach (var priceRange in cityIndex.PriceRanges)
                {
                    propertyIds.AddRange(priceRange.Value.PropertyIds);
                }
            }

            return propertyIds.Distinct().ToList();
        }

        /// <summary>
        /// الحصول على الكيانات في نطاق سعري معين
        /// </summary>
        private async Task<List<int>> GetPropertiesInPriceRangeAsync(decimal? minPrice, decimal? maxPrice)
        {
            var propertyIds = new List<int>();
            var priceRanges = GetPriceRangeKeys(minPrice, maxPrice);

            foreach (var rangeKey in priceRanges)
            {
                var priceIndex = await _fileService.ReadIndexAsync<PriceRangeIndex>("price-ranges", rangeKey);
                if (priceIndex != null)
                {
                    foreach (var cityData in priceIndex.CityBreakdown)
                    {
                        propertyIds.AddRange(cityData.Value.PropertyIds);
                    }
                }
            }

            return propertyIds.Distinct().ToList();
        }

        /// <summary>
        /// الحصول على الكيانات التي تحتوي على مرافق معينة
        /// </summary>
        private async Task<List<int>> GetPropertiesWithAmenitiesAsync(List<string> amenityIds)
        {
            var propertySets = new List<HashSet<int>>();

            foreach (var amenityId in amenityIds)
            {
                var amenityName = await GetAmenityNameByIdAsync(amenityId);
                if (!string.IsNullOrEmpty(amenityName))
                {
                    var amenityIndex = await _fileService.ReadIndexAsync<AmenityIndex>("amenities", amenityName.ToLowerInvariant());
                    if (amenityIndex != null)
                    {
                        var amenityProperties = new HashSet<int>();
                        foreach (var cityData in amenityIndex.CityDistribution)
                        {
                            amenityProperties.UnionWith(cityData.Value.PropertyIds);
                        }
                        propertySets.Add(amenityProperties);
                    }
                }
            }

            if (!propertySets.Any())
                return new List<int>();

            // تقاطع جميع المجموعات (الكيانات التي تحتوي على جميع المرافق المطلوبة)
            var result = propertySets.First();
            foreach (var set in propertySets.Skip(1))
            {
                result.IntersectWith(set);
            }

            return result.ToList();
        }

        /// <summary>
        /// الحصول على الكيانات حسب النوع
        /// </summary>
        private async Task<List<int>> GetPropertiesByTypeAsync(string propertyTypeId)
        {
            var typeName = await GetPropertyTypeNameByIdAsync(propertyTypeId);
            if (string.IsNullOrEmpty(typeName))
                return new List<int>();

            var typeIndex = await _fileService.ReadIndexAsync<PropertyTypeIndex>("property-types", typeName.ToLowerInvariant());
            if (typeIndex == null)
                return new List<int>();

            var propertyIds = new List<int>();
            foreach (var cityData in typeIndex.CityDistribution)
            {
                propertyIds.AddRange(cityData.Value.PropertyIds);
            }

            return propertyIds.Distinct().ToList();
        }

        /// <summary>
        /// الحصول على الكيانات المتاحة في فترة معينة
        /// </summary>
        private async Task<List<int>> GetAvailablePropertiesAsync(DateTime checkIn, DateTime checkOut)
        {
            var propertyIds = new List<int>();
            
            // تحديد الأشهر المتأثرة
            var affectedMonths = GetAffectedMonths(checkIn, checkOut);

            foreach (var monthKey in affectedMonths)
            {
                var availabilityIndex = await _fileService.ReadIndexAsync<AvailabilityIndex>("availability", monthKey);
                if (availabilityIndex != null)
                {
                    // البحث في التوفر اليومي
                    var dateKey = checkIn.ToString("yyyy-MM-dd");
                    if (availabilityIndex.DailyAvailability.TryGetValue(dateKey, out var dayData))
                    {
                        propertyIds.AddRange(dayData.AvailableIds);
                    }
                }
            }

            return propertyIds.Distinct().ToList();
        }

        /// <summary>
        /// تطبيق الترتيب على النتائج
        /// </summary>
        private async Task<List<int>> ApplySortingAsync(List<int> propertyIds, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrEmpty(sortBy) || sortBy == "relevance")
            {
                return propertyIds; // الترتيب الافتراضي حسب الصلة
            }

            // تحميل بيانات الترتيب
            var sortingData = await LoadSortingDataAsync(propertyIds, sortBy);

            return sortingData
                .OrderBy(kvp => isAscending ? kvp.Value : -kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// تطبيق الصفحات على النتائج
        /// </summary>
        private List<int> ApplyPaging(List<int> propertyIds, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            return propertyIds.Skip(skip).Take(pageSize).ToList();
        }

        /// <summary>
        /// تحميل تفاصيل الكيانات الأساسية
        /// </summary>
        private async Task<List<PropertySummary>> LoadPropertyDetailsAsync(List<int> propertyIds)
        {
            // هذه الطريقة تحتاج للتنفيذ باستخدام قاعدة البيانات أو فهارس إضافية
            // يمكن تحسينها لاحقاً باستخدام فهارس خاصة للبيانات الأساسية
            
            var properties = new List<PropertySummary>();
            
            foreach (var propertyId in propertyIds.Take(100)) // تحديد العدد لتجنب الحمولة الزائدة
            {
                // يمكن تحميل البيانات من فهرس خاص أو من قاعدة البيانات
                var property = await LoadSinglePropertySummaryAsync(propertyId);
                if (property != null)
                {
                    properties.Add(property);
                }
            }

            return properties;
        }

        /// <summary>
        /// تحميل ملخص كيان واحد
        /// </summary>
        private async Task<PropertySummary?> LoadSinglePropertySummaryAsync(int propertyId)
        {
            // تنفيذ مؤقت - يجب استبداله بتحميل من فهرس خاص أو قاعدة البيانات
            return new PropertySummary
            {
                PropertyId = propertyId,
                Name = $"Property {propertyId}",
                City = "Sample City",
                AveragePrice = 250.0m,
                Rating = 4.2,
                ImageUrl = "/images/default.jpg",
                Description = "Sample description",
                Amenities = new List<string> { "wifi", "parking" }
            };
        }

        /// <summary>
        /// إنشاء مفتاح كاش للبحث
        /// </summary>
        private string GenerateSearchCacheKey(SearchCriteria criteria)
        {
            var keyComponents = new List<string>
            {
                criteria.City ?? "any",
                $"price_{criteria.MinPrice}_{criteria.MaxPrice}",
                $"type_{criteria.PropertyTypeId ?? "any"}",
                $"amenities_{string.Join(",", criteria.AmenityIds ?? new List<string>())}",
                $"dates_{criteria.CheckInDate:yyyyMMdd}_{criteria.CheckOutDate:yyyyMMdd}",
                $"sort_{criteria.SortBy}_{criteria.IsAscending}",
                $"page_{criteria.PageNumber}_{criteria.PageSize}"
            };

            return $"search:{string.Join(":", keyComponents)}";
        }

        // إضافة باقي الدوال المساعدة...
        // (يمكن إضافة المزيد من الدوال المساعدة حسب الحاجة)

        #endregion
    }

    #region Data Models

    /// <summary>
    /// معايير البحث الشاملة
    /// </summary>
    public class SearchCriteria
    {
        public string? City { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? PropertyTypeId { get; set; }
        public List<string>? AmenityIds { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? SortBy { get; set; } = "relevance";
        public bool IsAscending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public double? MinRating { get; set; }
        public int? MaxDistance { get; set; } // بالكيلومتر
        public GeoPoint? LocationCenter { get; set; }
    }

    /// <summary>
    /// نتيجة البحث الرئيسية
    /// </summary>
    public class SearchResult
    {
        public List<PropertySummary> Properties { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public long SearchTimeMs { get; set; }
        public Dictionary<string, object> AppliedFilters { get; set; } = new();
    }

    /// <summary>
    /// نتيجة البحث النصي
    /// </summary>
    public class TextSearchResult
    {
        public List<TextSearchItem> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public long SearchTimeMs { get; set; }
        public List<string> SearchTerms { get; set; } = new();
        public List<string> SuggestedCorrections { get; set; } = new();
    }

    /// <summary>
    /// عنصر نتيجة البحث النصي
    /// </summary>
    public class TextSearchItem
    {
        public int PropertyId { get; set; }
        public PropertySummary? Property { get; set; }
        public double RelevanceScore { get; set; }
        public List<string> MatchedTerms { get; set; } = new();
        public string? HighlightedText { get; set; }
    }

    /// <summary>
    /// نتيجة البحث بالموقع
    /// </summary>
    public class LocationSearchResult
    {
        public List<PropertyWithDistance> Properties { get; set; } = new();
        public int TotalCount { get; set; }
        public long SearchTimeMs { get; set; }
        public GeoPoint SearchCenter { get; set; } = new();
        public double SearchRadiusKm { get; set; }
        public double AverageDistance { get; set; }
    }

    /// <summary>
    /// ملخص الكيان مع المسافة
    /// </summary>
    public class PropertyWithDistance : PropertySummary
    {
        public double DistanceKm { get; set; }
        public string? DistanceText { get; set; }
    }

    /// <summary>
    /// الفلاتر المتاحة
    /// </summary>
    public class AvailableFilters
    {
        public List<FilterOption> AvailableCities { get; set; } = new();
        public List<FilterOption> AvailablePriceRanges { get; set; } = new();
        public List<FilterOption> AvailableAmenities { get; set; } = new();
        public List<FilterOption> AvailablePropertyTypes { get; set; } = new();
        public List<FilterOption> AvailableRatingRanges { get; set; } = new();
    }

    /// <summary>
    /// خيار فلتر
    /// </summary>
    public class FilterOption
    {
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// إحصائيات البحث
    /// </summary>
    public class SearchStatistics
    {
        public int TotalResults { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> TopCities { get; set; } = new();
        public List<string> PopularAmenities { get; set; } = new();
    }

    /// <summary>
    /// ملخص الكيان
    /// </summary>
    public class PropertySummary
    {
        public int PropertyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal AveragePrice { get; set; }
        public double Rating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public GeoPoint? Location { get; set; }
    }

    /// <summary>
    /// نقطة جغرافية
    /// </summary>
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    /// <summary>
    /// إعدادات البحث
    /// </summary>
    public class SearchConfiguration
    {
        public int SearchCacheExpiryMinutes { get; set; } = 15;
        public int MaxSearchResults { get; set; } = 1000;
        public int DefaultPageSize { get; set; } = 20;
        public double DefaultSearchRadiusKm { get; set; } = 10.0;
    }

    #endregion
}