using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MediatR;
using BookN.Core.Services.IndexManagement;

namespace BookN.Core.Services.IndexGeneration
{
    /// <summary>
    /// خدمة إنشاء وتحديث الفهارس المسبقة
    /// تقوم بتحليل البيانات وإنشاء ملفات JSON محسنة للبحث السريع
    /// </summary>
    public interface IIndexGenerationService
    {
        /// <summary>
        /// إنشاء جميع الفهارس من الصفر
        /// </summary>
        Task GenerateAllIndexesAsync();

        /// <summary>
        /// إنشاء فهرس المدن
        /// </summary>
        Task GenerateCityIndexesAsync();

        /// <summary>
        /// إنشاء فهرس نطاقات الأسعار
        /// </summary>
        Task GeneratePriceRangeIndexesAsync();

        /// <summary>
        /// إنشاء فهرس المرافق
        /// </summary>
        Task GenerateAmenityIndexesAsync();

        /// <summary>
        /// إنشاء فهرس أنواع الكيانات
        /// </summary>
        Task GeneratePropertyTypeIndexesAsync();

        /// <summary>
        /// إنشاء فهرس التوفر
        /// </summary>
        Task GenerateAvailabilityIndexesAsync();

        /// <summary>
        /// إنشاء فهرس البحث النصي
        /// </summary>
        Task GenerateTextSearchIndexAsync();

        /// <summary>
        /// تحديث فهرس معين عند تغيير كيان
        /// </summary>
        Task UpdatePropertyIndexesAsync(Guid propertyId);

        /// <summary>
        /// تحديث فهرس معين عند تغيير وحدة
        /// </summary>
        Task UpdateUnitIndexesAsync(Guid unitId);

        /// <summary>
        /// إعادة بناء الفهارس التالفة
        /// </summary>
        Task RebuildCorruptedIndexesAsync();
    }

    public class IndexGenerationService : IIndexGenerationService
    {
        private readonly ILogger<IndexGenerationService> _logger;
        private readonly IJsonIndexFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public IndexGenerationService(
            ILogger<IndexGenerationService> logger,
            IJsonIndexFileService fileService,
            ApplicationDbContext context,
            IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// إنشاء جميع الفهارس من الصفر - عملية شاملة
        /// </summary>
        public async Task GenerateAllIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء جميع الفهارس...");

                // تنفيذ المهام بالتوازي لتسريع العملية
                var indexTasks = new List<Task>
                {
                    GenerateCityIndexesAsync(),
                    GeneratePriceRangeIndexesAsync(),
                    GenerateAmenityIndexesAsync(),
                    GeneratePropertyTypeIndexesAsync(),
                    GenerateAvailabilityIndexesAsync(),
                    GenerateTextSearchIndexAsync()
                };

                await Task.WhenAll(indexTasks);

                _logger.LogInformation("تم الانتهاء من إنشاء جميع الفهارس بنجاح");

                // إرسال إشعار بانتهاء إنشاء الفهارس
                await _mediator.Publish(new IndexesGeneratedNotification());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء الفهارس");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهارس المدن مع جميع التفاصيل الإحصائية
        /// </summary>
        public async Task GenerateCityIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهارس المدن...");

                // الحصول على جميع المدن الفريدة
                var cities = await _context.Properties
                    .Where(p => p.IsDeleted == false && p.IsActive == true && p.IsApproved == true)
                    .Select(p => p.City)
                    .Distinct()
                    .Where(city => !string.IsNullOrEmpty(city))
                    .ToListAsync();

                var cityIndexTasks = cities.Select(GenerateSingleCityIndexAsync);
                await Task.WhenAll(cityIndexTasks);

                _logger.LogInformation("تم إنشاء فهارس {Count} مدينة", cities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهارس المدن");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس مدينة واحدة بتفاصيل شاملة
        /// </summary>
        private async Task GenerateSingleCityIndexAsync(string city)
        {
            try
            {
                var cityProperties = await _context.Properties
                    .Include(p => p.Units)
                        .ThenInclude(u => u.UnitFieldValues)
                            .ThenInclude(ufv => ufv.UnitTypeField)
                    .Include(p => p.PropertyAmenities)
                        .ThenInclude(pa => pa.PropertyTypeAmenity)
                            .ThenInclude(pta => pta.Amenity)
                    .Include(p => p.Reviews)
                    .Where(p => p.City == city && p.IsDeleted == false && p.IsActive == true && p.IsApproved == true)
                    .ToListAsync();

                if (!cityProperties.Any())
                    return;

                // حساب الإحصائيات الأساسية
                var statistics = CalculateCityStatistics(cityProperties);

                // تجميع البيانات حسب نطاقات الأسعار
                var priceRanges = GroupPropertiesByPriceRanges(cityProperties);

                // تجميع البيانات حسب المرافق
                var amenityIntersections = GroupPropertiesByAmenities(cityProperties);

                // تجميع البيانات حسب أنواع الكيانات
                var propertyTypeDistribution = GroupPropertiesByType(cityProperties);

                // إنشاء فهرس التوفر للأشهر القادمة
                var availabilityCalendar = await GenerateAvailabilityCalendarAsync(cityProperties);

                var cityIndex = new CityIndex
                {
                    Metadata = new IndexMetadata
                    {
                        City = city.ToLowerInvariant(),
                        CityId = Guid.NewGuid(), // يمكن ربطه بجدول المدن إذا وجد
                        TotalProperties = cityProperties.Count,
                        LastUpdated = DateTime.UtcNow,
                        DataVersion = "1.2.5",
                        CompressionLevel = "optimal"
                    },
                    Statistics = statistics,
                    PriceRanges = priceRanges,
                    AmenityIntersections = amenityIntersections,
                    PropertyTypeDistribution = propertyTypeDistribution,
                    AvailabilityCalendar = availabilityCalendar
                };

                await _fileService.WriteIndexAsync("cities", city.ToLowerInvariant(), cityIndex);

                _logger.LogDebug("تم إنشاء فهرس المدينة: {City} ({Count} كيان)", city, cityProperties.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس المدينة: {City}", city);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهارس نطاقات الأسعار
        /// </summary>
        public async Task GeneratePriceRangeIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهارس نطاقات الأسعار...");

                var priceRanges = new[]
                {
                    new { Key = "0-100", Min = 0m, Max = 100m },
                    new { Key = "100-300", Min = 100m, Max = 300m },
                    new { Key = "300-500", Min = 300m, Max = 500m },
                    new { Key = "500-1000", Min = 500m, Max = 1000m },
                    new { Key = "1000+", Min = 1000m, Max = decimal.MaxValue }
                };

                var rangeIndexTasks = priceRanges.Select(range => 
                    GenerateSinglePriceRangeIndexAsync(range.Key, range.Min, range.Max));

                await Task.WhenAll(rangeIndexTasks);

                _logger.LogInformation("تم إنشاء فهارس {Count} نطاق سعري", priceRanges.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهارس نطاقات الأسعار");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس نطاق سعري واحد
        /// </summary>
        private async Task GenerateSinglePriceRangeIndexAsync(string rangeKey, decimal minPrice, decimal maxPrice)
        {
            try
            {
                var query = _context.Units
                    .Include(u => u.Property)
                    .Include(u => u.PricingRules)
                    .Where(u => u.IsDeleted == false && u.IsActive == true && u.IsAvailable == true)
                    .Where(u => u.Property.IsDeleted == false && u.Property.IsActive == true && u.Property.IsApproved == true);

                if (maxPrice != decimal.MaxValue)
                {
                    query = query.Where(u => 
                        decimal.Parse(u.BasePrice_Amount) >= minPrice && 
                        decimal.Parse(u.BasePrice_Amount) < maxPrice);
                }
                else
                {
                    query = query.Where(u => decimal.Parse(u.BasePrice_Amount) >= minPrice);
                }

                var unitsInRange = await query.ToListAsync();
                var propertiesInRange = unitsInRange.Select(u => u.Property).DistinctBy(p => p.PropertyId).ToList();

                if (!propertiesInRange.Any())
                    return;

                // تجميع البيانات حسب المدن
                var cityBreakdown = GroupPropertiesByCity(propertiesInRange);

                // تجميع البيانات حسب أنواع الكيانات
                var propertyTypeIntersection = GroupPropertiesByType(propertiesInRange);

                // تجميع البيانات حسب المرافق
                var amenityIntersection = GroupPropertiesByAmenities(propertiesInRange);

                // حساب التسعير الموسمي
                var seasonalPricing = CalculateSeasonalPricing(unitsInRange);

                var priceRangeIndex = new PriceRangeIndex
                {
                    Metadata = new PriceRangeMetadata
                    {
                        PriceRange = rangeKey,
                        Currency = "SAR",
                        TotalProperties = propertiesInRange.Count,
                        LastUpdated = DateTime.UtcNow,
                        IndexVersion = "2.1.0"
                    },
                    CityBreakdown = cityBreakdown,
                    PropertyTypeIntersection = propertyTypeIntersection,
                    AmenityIntersection = amenityIntersection,
                    SeasonalPricing = seasonalPricing
                };

                await _fileService.WriteIndexAsync("price-ranges", $"range_{rangeKey.Replace("-", "_").Replace("+", "_plus")}", priceRangeIndex);

                _logger.LogDebug("تم إنشاء فهرس النطاق السعري: {Range} ({Count} كيان)", rangeKey, propertiesInRange.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس النطاق السعري: {Range}", rangeKey);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهارس المرافق
        /// </summary>
        public async Task GenerateAmenityIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهارس المرافق...");

                var amenities = await _context.Amenities
                    .Where(a => a.IsDeleted == false && a.IsActive == true)
                    .ToListAsync();

                var amenityIndexTasks = amenities.Select(GenerateSingleAmenityIndexAsync);
                await Task.WhenAll(amenityIndexTasks);

                _logger.LogInformation("تم إنشاء فهارس {Count} مرفق", amenities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهارس المرافق");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس مرفق واحد
        /// </summary>
        private async Task GenerateSingleAmenityIndexAsync(Amenities amenity)
        {
            try
            {
                var propertiesWithAmenity = await _context.PropertyAmenities
                    .Include(pa => pa.Property)
                    .Include(pa => pa.PropertyTypeAmenity)
                    .Where(pa => pa.PropertyTypeAmenity.AmenityId == amenity.AmenityId.ToString())
                    .Where(pa => pa.IsDeleted == false && pa.IsActive == true && pa.IsAvailable == true)
                    .Where(pa => pa.Property.IsDeleted == false && pa.Property.IsActive == true && pa.Property.IsApproved == true)
                    .Select(pa => pa.Property)
                    .Distinct()
                    .ToListAsync();

                if (!propertiesWithAmenity.Any())
                    return;

                // توزيع المدن
                var cityDistribution = GroupPropertiesByCity(propertiesWithAmenity);

                // تقاطع نطاقات الأسعار
                var priceRangeIntersection = GroupPropertiesByPriceRanges(propertiesWithAmenity);

                // تقاطع أنواع الكيانات
                var propertyTypeIntersection = GroupPropertiesByType(propertiesWithAmenity);

                // المرافق المرتبطة
                var relatedAmenities = await CalculateRelatedAmenitiesAsync(amenity.AmenityId.ToString(), propertiesWithAmenity);

                // التوفر الموسمي
                var seasonalAvailability = CalculateSeasonalAvailability(propertiesWithAmenity);

                var amenityIndex = new AmenityIndex
                {
                    Metadata = new AmenityMetadata
                    {
                        AmenityId = amenity.AmenityId.ToString(),
                        AmenityName = amenity.Name,
                        AmenityNameAr = amenity.Name, // يمكن إضافة حقل للاسم العربي
                        Category = "general", // يمكن إضافة حقل الفئة
                        TotalProperties = propertiesWithAmenity.Count,
                        LastUpdated = DateTime.UtcNow
                    },
                    CityDistribution = cityDistribution,
                    PriceRangeIntersection = priceRangeIntersection,
                    PropertyTypeIntersection = propertyTypeIntersection,
                    RelatedAmenities = relatedAmenities,
                    SeasonalAvailability = seasonalAvailability
                };

                var fileName = amenity.Name.ToLowerInvariant().Replace(" ", "_").Replace("-", "_");
                await _fileService.WriteIndexAsync("amenities", fileName, amenityIndex);

                _logger.LogDebug("تم إنشاء فهرس المرفق: {Amenity} ({Count} كيان)", amenity.Name, propertiesWithAmenity.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس المرفق: {AmenityId}", amenity.AmenityId);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهارس أنواع الكيانات
        /// </summary>
        public async Task GeneratePropertyTypeIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهارس أنواع الكيانات...");

                var propertyTypes = await _context.PropertyTypes
                    .Where(pt => pt.IsDeleted == false && pt.IsActive == true)
                    .ToListAsync();

                var typeIndexTasks = propertyTypes.Select(GenerateSinglePropertyTypeIndexAsync);
                await Task.WhenAll(typeIndexTasks);

                _logger.LogInformation("تم إنشاء فهارس {Count} نوع كيان", propertyTypes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهارس أنواع الكيانات");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس نوع كيان واحد
        /// </summary>
        private async Task GenerateSinglePropertyTypeIndexAsync(PropertyTypes propertyType)
        {
            try
            {
                var propertiesOfType = await _context.Properties
                    .Include(p => p.Units)
                        .ThenInclude(u => u.UnitType)
                    .Where(p => p.TypeId == propertyType.TypeId.ToString())
                    .Where(p => p.IsDeleted == false && p.IsActive == true && p.IsApproved == true)
                    .ToListAsync();

                if (!propertiesOfType.Any())
                    return;

                // توزيع التقييمات النجمية
                var starRatingDistribution = GroupPropertiesByStarRating(propertiesOfType);

                // توزيع المدن
                var cityDistribution = GroupPropertiesByCity(propertiesOfType);

                // المرافق الشائعة
                var commonAmenities = await CalculateCommonAmenitiesAsync(propertiesOfType);

                // تفصيل أنواع الوحدات
                var unitTypeBreakdown = CalculateUnitTypeBreakdown(propertiesOfType);

                var propertyTypeIndex = new PropertyTypeIndex
                {
                    Metadata = new PropertyTypeMetadata
                    {
                        PropertyTypeId = propertyType.TypeId.ToString(),
                        TypeName = propertyType.Name,
                        TypeNameAr = propertyType.Name, // يمكن إضافة الاسم العربي
                        TotalProperties = propertiesOfType.Count,
                        LastUpdated = DateTime.UtcNow
                    },
                    StarRatingDistribution = starRatingDistribution,
                    CityDistribution = cityDistribution,
                    CommonAmenities = commonAmenities,
                    UnitTypeBreakdown = unitTypeBreakdown
                };

                var fileName = propertyType.Name.ToLowerInvariant().Replace(" ", "_");
                await _fileService.WriteIndexAsync("property-types", fileName, propertyTypeIndex);

                _logger.LogDebug("تم إنشاء فهرس نوع الكيان: {Type} ({Count} كيان)", propertyType.Name, propertiesOfType.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس نوع الكيان: {TypeId}", propertyType.TypeId);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهارس التوفر للأشهر القادمة
        /// </summary>
        public async Task GenerateAvailabilityIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهارس التوفر...");

                var currentDate = DateTime.UtcNow;
                var months = Enumerable.Range(0, 12)
                    .Select(i => currentDate.AddMonths(i))
                    .Select(date => new { Year = date.Year, Month = date.Month, Key = $"{date.Year}-{date.Month:D2}" });

                var availabilityTasks = months.Select(month => 
                    GenerateSingleAvailabilityIndexAsync(month.Key, month.Year, month.Month));

                await Task.WhenAll(availabilityTasks);

                _logger.LogInformation("تم إنشاء فهارس التوفر لـ 12 شهر");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهارس التوفر");
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس التوفر لشهر واحد
        /// </summary>
        private async Task GenerateSingleAvailabilityIndexAsync(string monthKey, int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // الحصول على جميع الوحدات المتاحة في هذا الشهر
                var availableUnits = await _context.Units
                    .Include(u => u.Property)
                    .Include(u => u.UnitAvailabilities)
                    .Where(u => u.IsDeleted == false && u.IsActive == true && u.IsAvailable == true)
                    .Where(u => u.Property.IsDeleted == false && u.Property.IsActive == true && u.Property.IsApproved == true)
                    .Where(u => !u.UnitAvailabilities.Any(ua => 
                        ua.Status == "Booked" && 
                        ua.IsDeleted == false &&
                        ((ua.StartDate <= startDate && ua.EndDate >= startDate) ||
                         (ua.StartDate <= endDate && ua.EndDate >= endDate) ||
                         (ua.StartDate >= startDate && ua.EndDate <= endDate))))
                    .ToListAsync();

                var availableProperties = availableUnits
                    .Select(u => u.Property)
                    .DistinctBy(p => p.PropertyId)
                    .ToList();

                if (!availableProperties.Any())
                    return;

                // حساب التوفر اليومي
                var dailyAvailability = CalculateDailyAvailability(availableProperties, startDate, endDate);

                // توفر نهايات الأسبوع مقابل أيام الأسبوع
                var weekendAvailability = CalculateWeekendAvailability(availableProperties, startDate, endDate);

                // توفر نطاقات الأسعار
                var priceRangeAvailability = GroupPropertiesByPriceRanges(availableProperties);

                var totalUnits = await _context.Units.CountAsync(u => u.IsDeleted == false);
                var occupancyRate = 1.0 - (double)availableUnits.Count / totalUnits;

                var availabilityIndex = new AvailabilityIndex
                {
                    Metadata = new AvailabilityMetadata
                    {
                        Month = monthKey,
                        TotalProperties = await _context.Properties.CountAsync(p => p.IsDeleted == false),
                        TotalUnits = totalUnits,
                        LastUpdated = DateTime.UtcNow,
                        OccupancyRate = occupancyRate
                    },
                    DailyAvailability = dailyAvailability,
                    WeekendAvailability = weekendAvailability,
                    PriceRangeAvailability = priceRangeAvailability
                };

                await _fileService.WriteIndexAsync("availability", monthKey, availabilityIndex);

                _logger.LogDebug("تم إنشاء فهرس التوفر للشهر: {Month} ({Count} كيان متاح)", monthKey, availableProperties.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس التوفر للشهر: {Month}", monthKey);
                throw;
            }
        }

        /// <summary>
        /// إنشاء فهرس البحث النصي باستخدام هيكل Trie
        /// </summary>
        public async Task GenerateTextSearchIndexAsync()
        {
            try
            {
                _logger.LogInformation("بدء إنشاء فهرس البحث النصي...");

                var properties = await _context.Properties
                    .Where(p => p.IsDeleted == false && p.IsActive == true && p.IsApproved == true)
                    .Select(p => new { p.PropertyId, p.Name, p.Description, p.City, p.Address })
                    .ToListAsync();

                var trie = new Dictionary<string, object>();
                var popularTerms = new Dictionary<string, TextSearchTerm>();
                var synonyms = new Dictionary<string, List<string>>();

                foreach (var property in properties)
                {
                    var texts = new[] { property.Name, property.Description, property.City, property.Address }
                        .Where(t => !string.IsNullOrEmpty(t));

                    foreach (var text in texts)
                    {
                        var words = ExtractSearchableWords(text);
                        foreach (var word in words)
                        {
                            AddWordToTrie(trie, word, int.Parse(property.PropertyId));
                            UpdatePopularTerms(popularTerms, word, int.Parse(property.PropertyId));
                        }
                    }
                }

                // إضافة المرادفات الشائعة
                AddCommonSynonyms(synonyms);

                var textSearchIndex = new TextSearchIndex
                {
                    Metadata = new TextSearchMetadata
                    {
                        TotalWords = popularTerms.Count,
                        TotalProperties = properties.Count,
                        LastUpdated = DateTime.UtcNow,
                        Language = "ar_SA"
                    },
                    Trie = trie,
                    PopularTerms = popularTerms,
                    Synonyms = synonyms
                };

                await _fileService.WriteIndexAsync("text-search", "trie_structure", textSearchIndex);

                _logger.LogInformation("تم إنشاء فهرس البحث النصي ({Count} كلمة)", popularTerms.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس البحث النصي");
                throw;
            }
        }

        /// <summary>
        /// تحديث الفهارس عند تغيير كيان
        /// </summary>
        public async Task UpdatePropertyIndexesAsync(Guid propertyId)
        {
            try
            {
                _logger.LogDebug("بدء تحديث فهارس الكيان: {PropertyId}", propertyId);

                var property = await _context.Properties
                    .Include(p => p.Units)
                    .Include(p => p.PropertyAmenities)
                    .FirstOrDefaultAsync(p => p.PropertyId == propertyId.ToString());

                if (property == null)
                {
                    _logger.LogWarning("لم يتم العثور على الكيان: {PropertyId}", propertyId);
                    return;
                }

                // تحديث فهرس المدينة
                if (!string.IsNullOrEmpty(property.City))
                {
                    await GenerateSingleCityIndexAsync(property.City);
                }

                // تحديث فهارس نطاقات الأسعار المتأثرة
                var prices = property.Units.Select(u => decimal.Parse(u.BasePrice_Amount ?? "0")).ToList();
                var affectedRanges = GetAffectedPriceRanges(prices);
                
                foreach (var range in affectedRanges)
                {
                    await GenerateSinglePriceRangeIndexAsync(range.Key, range.Min, range.Max);
                }

                // تحديث فهرس نوع الكيان
                if (!string.IsNullOrEmpty(property.TypeId))
                {
                    var propertyType = await _context.PropertyTypes
                        .FirstOrDefaultAsync(pt => pt.TypeId.ToString() == property.TypeId);
                    
                    if (propertyType != null)
                    {
                        await GenerateSinglePropertyTypeIndexAsync(propertyType);
                    }
                }

                _logger.LogDebug("تم تحديث فهارس الكيان: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهارس الكيان: {PropertyId}", propertyId);
                throw;
            }
        }

        /// <summary>
        /// تحديث الفهارس عند تغيير وحدة
        /// </summary>
        public async Task UpdateUnitIndexesAsync(Guid unitId)
        {
            try
            {
                _logger.LogDebug("بدء تحديث فهارس الوحدة: {UnitId}", unitId);

                var unit = await _context.Units
                    .Include(u => u.Property)
                    .FirstOrDefaultAsync(u => u.UnitId == unitId.ToString());

                if (unit?.Property != null)
                {
                    await UpdatePropertyIndexesAsync(Guid.Parse(unit.PropertyId));
                }

                _logger.LogDebug("تم تحديث فهارس الوحدة: {UnitId}", unitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهارس الوحدة: {UnitId}", unitId);
                throw;
            }
        }

        /// <summary>
        /// إعادة بناء الفهارس التالفة
        /// </summary>
        public async Task RebuildCorruptedIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء فحص وإعادة بناء الفهارس التالفة...");

                var indexTypes = new[] { "cities", "price-ranges", "amenities", "property-types", "availability" };
                var corruptedCount = 0;

                foreach (var indexType in indexTypes)
                {
                    try
                    {
                        var indexKeys = await _fileService.GetIndexKeysAsync(indexType);
                        
                        foreach (var key in indexKeys)
                        {
                            try
                            {
                                // محاولة قراءة الفهرس للتحقق من سلامته
                                var testRead = await _fileService.ReadIndexAsync<object>(indexType, key);
                                if (testRead == null)
                                {
                                    _logger.LogWarning("فهرس تالف تم اكتشافه: {IndexType}:{Key}", indexType, key);
                                    await _fileService.DeleteIndexAsync(indexType, key);
                                    corruptedCount++;
                                }
                            }
                            catch
                            {
                                _logger.LogWarning("فهرس تالف تم اكتشافه: {IndexType}:{Key}", indexType, key);
                                await _fileService.DeleteIndexAsync(indexType, key);
                                corruptedCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطأ في فحص نوع الفهرس: {IndexType}", indexType);
                    }
                }

                if (corruptedCount > 0)
                {
                    _logger.LogWarning("تم العثور على {Count} فهرس تالف، سيتم إعادة بناء جميع الفهارس", corruptedCount);
                    await GenerateAllIndexesAsync();
                }
                else
                {
                    _logger.LogInformation("جميع الفهارس سليمة");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إعادة بناء الفهارس التالفة");
                throw;
            }
        }

        #region Private Helper Methods

        private CityStatistics CalculateCityStatistics(List<Properties> properties)
        {
            var prices = properties.SelectMany(p => p.Units)
                .Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                .Select(u => decimal.Parse(u.BasePrice_Amount))
                .ToList();

            var reviews = properties.SelectMany(p => p.Reviews)
                .Where(r => r.IsDeleted == false)
                .ToList();

            return new CityStatistics
            {
                AveragePrice = prices.Any() ? prices.Average() : 0,
                MinPrice = prices.Any() ? prices.Min() : 0,
                MaxPrice = prices.Any() ? prices.Max() : 0,
                PopularAmenities = GetPopularAmenities(properties),
                AverageRating = reviews.Any() ? (double)reviews.Average(r => r.AverageRating) : 0,
                TotalReviews = reviews.Count
            };
        }

        private Dictionary<string, PriceRangeData> GroupPropertiesByPriceRanges(List<Properties> properties)
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
                    var prices = p.Units.Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                        .Select(u => decimal.Parse(u.BasePrice_Amount));
                    
                    return prices.Any(price => 
                        price >= rangeDef.Min && 
                        (rangeDef.Max == decimal.MaxValue ? true : price < rangeDef.Max));
                }).ToList();

                if (propertiesInRange.Any())
                {
                    ranges[rangeDef.Key] = new PriceRangeData
                    {
                        Count = propertiesInRange.Count,
                        PropertyIds = propertiesInRange.Take(10).Select(p => int.Parse(p.PropertyId)).ToList()
                    };
                }
            }

            return ranges;
        }

        private Dictionary<string, AmenityData> GroupPropertiesByAmenities(List<Properties> properties)
        {
            return properties
                .SelectMany(p => p.PropertyAmenities)
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .GroupBy(pa => pa.PropertyTypeAmenity?.Amenity?.Name ?? "Unknown")
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Key != "Unknown")
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => new AmenityData
                    {
                        Count = g.Count(),
                        PropertyIds = g.Select(pa => int.Parse(pa.PropertyId)).Distinct().Take(10).ToList()
                    });
        }

        private Dictionary<string, PropertyTypeData> GroupPropertiesByType(List<Properties> properties)
        {
            return properties
                .Where(p => !string.IsNullOrEmpty(p.TypeId))
                .GroupBy(p => GetPropertyTypeName(p.TypeId))
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => new PropertyTypeData
                    {
                        Count = g.Count(),
                        AveragePrice = CalculateAveragePrice(g.ToList()),
                        PropertyIds = g.Take(10).Select(p => int.Parse(p.PropertyId)).ToList()
                    });
        }

        private string GetPropertyTypeName(string typeId)
        {
            // يمكن تحسين هذا بإضافة كاش للأنواع
            var type = _context.PropertyTypes.FirstOrDefault(pt => pt.TypeId.ToString() == typeId);
            return type?.Name ?? "Unknown";
        }

        private decimal CalculateAveragePrice(List<Properties> properties)
        {
            var prices = properties.SelectMany(p => p.Units)
                .Where(u => !string.IsNullOrEmpty(u.BasePrice_Amount))
                .Select(u => decimal.Parse(u.BasePrice_Amount))
                .ToList();

            return prices.Any() ? prices.Average() : 0;
        }

        private List<string> GetPopularAmenities(List<Properties> properties)
        {
            return properties
                .SelectMany(p => p.PropertyAmenities)
                .Where(pa => pa.IsDeleted == false && pa.IsAvailable == true)
                .GroupBy(pa => pa.PropertyTypeAmenity?.Amenity?.Name)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key.ToLowerInvariant())
                .ToList();
        }

        private async Task<Dictionary<string, AvailabilityData>> GenerateAvailabilityCalendarAsync(List<Properties> properties)
        {
            var calendar = new Dictionary<string, AvailabilityData>();
            var currentDate = DateTime.UtcNow;

            for (int i = 0; i < 3; i++) // الثلاثة أشهر القادمة
            {
                var monthDate = currentDate.AddMonths(i);
                var monthKey = $"{monthDate.Year}-{monthDate.Month:D2}";

                var availableProperties = await GetAvailablePropertiesForMonth(properties, monthDate);

                calendar[monthKey] = new AvailabilityData
                {
                    AvailableCount = availableProperties.Count,
                    BookedCount = properties.Count - availableProperties.Count,
                    AvailableIds = availableProperties.Take(10).Select(p => int.Parse(p.PropertyId)).ToList()
                };
            }

            return calendar;
        }

        private async Task<List<Properties>> GetAvailablePropertiesForMonth(List<Properties> properties, DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var availableProperties = new List<Properties>();

            foreach (var property in properties)
            {
                var hasAvailableUnits = await _context.Units
                    .Where(u => u.PropertyId == property.PropertyId)
                    .Where(u => u.IsDeleted == false && u.IsActive == true && u.IsAvailable == true)
                    .Where(u => !u.UnitAvailabilities.Any(ua =>
                        ua.Status == "Booked" &&
                        ua.IsDeleted == false &&
                        ((ua.StartDate <= startDate && ua.EndDate >= startDate) ||
                         (ua.StartDate <= endDate && ua.EndDate >= endDate) ||
                         (ua.StartDate >= startDate && ua.EndDate <= endDate))))
                    .AnyAsync();

                if (hasAvailableUnits)
                {
                    availableProperties.Add(property);
                }
            }

            return availableProperties;
        }

        // إضافة باقي الدوال المساعدة...
        // (يمكن إضافة المزيد من الدوال المساعدة حسب الحاجة)

        #endregion
    }
}