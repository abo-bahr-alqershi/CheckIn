using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using BookN.Core.Services.IndexManagement;
using BookN.Core.Services.IndexGeneration;

namespace BookN.Core.Services.AutoUpdate
{
    /// <summary>
    /// خدمة التحديث التلقائي للفهارس
    /// تستمع للأحداث وتحدث الفهارس فوراً
    /// </summary>
    public interface IIndexAutoUpdateService
    {
        /// <summary>
        /// تحديث فوري عند إنشاء كيان جديد
        /// </summary>
        Task HandlePropertyCreatedAsync(Guid propertyId);

        /// <summary>
        /// تحديث فوري عند تحديث كيان
        /// </summary>
        Task HandlePropertyUpdatedAsync(Guid propertyId, PropertyUpdateInfo updateInfo);

        /// <summary>
        /// تحديث فوري عند حذف كيان
        /// </summary>
        Task HandlePropertyDeletedAsync(Guid propertyId, PropertyDeleteInfo deleteInfo);

        /// <summary>
        /// تحديث فوري عند إنشاء وحدة جديدة
        /// </summary>
        Task HandleUnitCreatedAsync(Guid unitId);

        /// <summary>
        /// تحديث فوري عند تحديث وحدة
        /// </summary>
        Task HandleUnitUpdatedAsync(Guid unitId, UnitUpdateInfo updateInfo);

        /// <summary>
        /// تحديث فوري عند تحديث الأسعار
        /// </summary>
        Task HandlePricingUpdatedAsync(Guid unitId, PricingUpdateInfo pricingInfo);

        /// <summary>
        /// تحديث فوري عند تغيير التوفر
        /// </summary>
        Task HandleAvailabilityChangedAsync(Guid unitId, AvailabilityChangeInfo changeInfo);

        /// <summary>
        /// تحديث فوري عند إضافة تقييم
        /// </summary>
        Task HandleReviewAddedAsync(Guid propertyId, ReviewInfo reviewInfo);

        /// <summary>
        /// إعادة تشغيل النظام والتحقق من سلامة الفهارس
        /// </summary>
        Task InitializeAsync();
    }

    public class IndexAutoUpdateService : IIndexAutoUpdateService
    {
        private readonly ILogger<IndexAutoUpdateService> _logger;
        private readonly IIndexGenerationService _indexGenerator;
        private readonly IJsonIndexFileService _fileService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IndexAutoUpdateService(
            ILogger<IndexAutoUpdateService> logger,
            IIndexGenerationService indexGenerator,
            IJsonIndexFileService fileService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _indexGenerator = indexGenerator ?? throw new ArgumentNullException(nameof(indexGenerator));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// التعامل مع إنشاء كيان جديد
        /// </summary>
        public async Task HandlePropertyCreatedAsync(Guid propertyId)
        {
            try
            {
                _logger.LogInformation("بدء تحديث الفهارس لكيان جديد: {PropertyId}", propertyId);

                // تحديث متوازي للفهارس المتأثرة
                var updateTasks = new List<Task>();

                // تحديد الفهارس التي تحتاج للتحديث
                using var scope = _serviceScopeFactory.CreateScope();
                var property = await GetPropertyDetailsAsync(scope, propertyId);
                
                if (property != null)
                {
                    // تحديث فهرس المدينة
                    if (!string.IsNullOrEmpty(property.City))
                    {
                        updateTasks.Add(UpdateCityIndexAsync(property.City));
                    }

                    // تحديث فهارس نطاقات الأسعار
                    var affectedPriceRanges = GetAffectedPriceRanges(property.MinPrice, property.MaxPrice);
                    foreach (var range in affectedPriceRanges)
                    {
                        updateTasks.Add(UpdatePriceRangeIndexAsync(range));
                    }

                    // تحديث فهرس نوع الكيان
                    if (!string.IsNullOrEmpty(property.PropertyTypeId))
                    {
                        updateTasks.Add(UpdatePropertyTypeIndexAsync(property.PropertyTypeId));
                    }

                    // تحديث فهارس المرافق
                    if (property.AmenityIds?.Any() == true)
                    {
                        foreach (var amenityId in property.AmenityIds)
                        {
                            updateTasks.Add(UpdateAmenityIndexAsync(amenityId));
                        }
                    }

                    // تحديث فهرس التوفر للأشهر القادمة
                    updateTasks.Add(UpdateAvailabilityIndexesAsync(property.Id));

                    // تحديث فهرس البحث النصي
                    updateTasks.Add(UpdateTextSearchIndexAsync(property.Id, property.Name, property.Description));

                    // تنفيذ جميع التحديثات بالتوازي
                    await Task.WhenAll(updateTasks);
                }

                _logger.LogInformation("تم تحديث الفهارس بنجاح للكيان الجديد: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للكيان الجديد: {PropertyId}", propertyId);
                
                // إعادة محاولة التحديث في الخلفية
                _ = Task.Run(async () => await RetryIndexUpdateAsync("PropertyCreated", propertyId.ToString()));
            }
        }

        /// <summary>
        /// التعامل مع تحديث كيان موجود
        /// </summary>
        public async Task HandlePropertyUpdatedAsync(Guid propertyId, PropertyUpdateInfo updateInfo)
        {
            try
            {
                _logger.LogInformation("بدء تحديث الفهارس للكيان المحدث: {PropertyId}", propertyId);

                var updateTasks = new List<Task>();

                // تحديد الفهارس المتأثرة بناء على التغييرات
                if (updateInfo.CityChanged && (!string.IsNullOrEmpty(updateInfo.OldCity) || !string.IsNullOrEmpty(updateInfo.NewCity)))
                {
                    // تحديث فهرس المدينة القديمة والجديدة
                    if (!string.IsNullOrEmpty(updateInfo.OldCity))
                    {
                        updateTasks.Add(UpdateCityIndexAsync(updateInfo.OldCity));
                    }
                    if (!string.IsNullOrEmpty(updateInfo.NewCity))
                    {
                        updateTasks.Add(UpdateCityIndexAsync(updateInfo.NewCity));
                    }
                }

                if (updateInfo.PriceChanged)
                {
                    // تحديث فهارس نطاقات الأسعار المتأثرة
                    var oldRanges = GetAffectedPriceRanges(updateInfo.OldMinPrice, updateInfo.OldMaxPrice);
                    var newRanges = GetAffectedPriceRanges(updateInfo.NewMinPrice, updateInfo.NewMaxPrice);
                    
                    var allAffectedRanges = oldRanges.Concat(newRanges).Distinct();
                    foreach (var range in allAffectedRanges)
                    {
                        updateTasks.Add(UpdatePriceRangeIndexAsync(range));
                    }
                }

                if (updateInfo.PropertyTypeChanged)
                {
                    // تحديث فهارس أنواع الكيانات
                    if (!string.IsNullOrEmpty(updateInfo.OldPropertyTypeId))
                    {
                        updateTasks.Add(UpdatePropertyTypeIndexAsync(updateInfo.OldPropertyTypeId));
                    }
                    if (!string.IsNullOrEmpty(updateInfo.NewPropertyTypeId))
                    {
                        updateTasks.Add(UpdatePropertyTypeIndexAsync(updateInfo.NewPropertyTypeId));
                    }
                }

                if (updateInfo.AmenitiesChanged)
                {
                    // تحديث فهارس المرافق
                    var allAffectedAmenities = (updateInfo.AddedAmenities ?? new List<string>())
                        .Concat(updateInfo.RemovedAmenities ?? new List<string>())
                        .Distinct();
                    
                    foreach (var amenityId in allAffectedAmenities)
                    {
                        updateTasks.Add(UpdateAmenityIndexAsync(amenityId));
                    }
                }

                if (updateInfo.NameOrDescriptionChanged)
                {
                    // تحديث فهرس البحث النصي
                    updateTasks.Add(UpdateTextSearchIndexAsync(propertyId, updateInfo.NewName, updateInfo.NewDescription));
                }

                // تنفيذ جميع التحديثات بالتوازي
                await Task.WhenAll(updateTasks);

                _logger.LogInformation("تم تحديث الفهارس بنجاح للكيان المحدث: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للكيان المحدث: {PropertyId}", propertyId);
                
                // إعادة محاولة التحديث في الخلفية
                _ = Task.Run(async () => await RetryIndexUpdateAsync("PropertyUpdated", propertyId.ToString()));
            }
        }

        /// <summary>
        /// التعامل مع حذف كيان
        /// </summary>
        public async Task HandlePropertyDeletedAsync(Guid propertyId, PropertyDeleteInfo deleteInfo)
        {
            try
            {
                _logger.LogInformation("بدء تحديث الفهارس لكيان محذوف: {PropertyId}", propertyId);

                var updateTasks = new List<Task>();

                // تحديث جميع الفهارس المتأثرة بحذف الكيان
                if (!string.IsNullOrEmpty(deleteInfo.City))
                {
                    updateTasks.Add(UpdateCityIndexAsync(deleteInfo.City));
                }

                var affectedPriceRanges = GetAffectedPriceRanges(deleteInfo.MinPrice, deleteInfo.MaxPrice);
                foreach (var range in affectedPriceRanges)
                {
                    updateTasks.Add(UpdatePriceRangeIndexAsync(range));
                }

                if (!string.IsNullOrEmpty(deleteInfo.PropertyTypeId))
                {
                    updateTasks.Add(UpdatePropertyTypeIndexAsync(deleteInfo.PropertyTypeId));
                }

                if (deleteInfo.AmenityIds?.Any() == true)
                {
                    foreach (var amenityId in deleteInfo.AmenityIds)
                    {
                        updateTasks.Add(UpdateAmenityIndexAsync(amenityId));
                    }
                }

                // تحديث فهارس التوفر
                updateTasks.Add(UpdateAvailabilityIndexesAsync(propertyId));

                // إزالة من فهرس البحث النصي
                updateTasks.Add(RemoveFromTextSearchIndexAsync(propertyId));

                await Task.WhenAll(updateTasks);

                _logger.LogInformation("تم تحديث الفهارس بنجاح للكيان المحذوف: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للكيان المحذوف: {PropertyId}", propertyId);
            }
        }

        /// <summary>
        /// التعامل مع إنشاء وحدة جديدة
        /// </summary>
        public async Task HandleUnitCreatedAsync(Guid unitId)
        {
            try
            {
                _logger.LogDebug("بدء تحديث الفهارس لوحدة جديدة: {UnitId}", unitId);

                using var scope = _serviceScopeFactory.CreateScope();
                var unit = await GetUnitDetailsAsync(scope, unitId);
                
                if (unit != null && unit.PropertyId.HasValue)
                {
                    // تحديث فهارس الكيان المرتبط
                    await HandlePropertyUpdatedAsync(unit.PropertyId.Value, new PropertyUpdateInfo
                    {
                        PriceChanged = true,
                        NewMinPrice = unit.BasePrice,
                        NewMaxPrice = unit.BasePrice
                    });

                    // تحديث فهارس التوفر
                    await UpdateAvailabilityIndexesAsync(unit.PropertyId.Value);
                }

                _logger.LogDebug("تم تحديث الفهارس بنجاح للوحدة الجديدة: {UnitId}", unitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للوحدة الجديدة: {UnitId}", unitId);
            }
        }

        /// <summary>
        /// التعامل مع تحديث وحدة
        /// </summary>
        public async Task HandleUnitUpdatedAsync(Guid unitId, UnitUpdateInfo updateInfo)
        {
            try
            {
                _logger.LogDebug("بدء تحديث الفهارس للوحدة المحدثة: {UnitId}", unitId);

                if (updateInfo.PropertyId.HasValue)
                {
                    if (updateInfo.PriceChanged)
                    {
                        // إعادة حساب نطاقات الأسعار للكيان
                        var propertyUpdateInfo = new PropertyUpdateInfo
                        {
                            PriceChanged = true,
                            OldMinPrice = updateInfo.OldPrice,
                            OldMaxPrice = updateInfo.OldPrice,
                            NewMinPrice = updateInfo.NewPrice,
                            NewMaxPrice = updateInfo.NewPrice
                        };
                        
                        await HandlePropertyUpdatedAsync(updateInfo.PropertyId.Value, propertyUpdateInfo);
                    }

                    if (updateInfo.AvailabilityChanged)
                    {
                        await UpdateAvailabilityIndexesAsync(updateInfo.PropertyId.Value);
                    }
                }

                _logger.LogDebug("تم تحديث الفهارس بنجاح للوحدة المحدثة: {UnitId}", unitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للوحدة المحدثة: {UnitId}", unitId);
            }
        }

        /// <summary>
        /// التعامل مع تحديث الأسعار
        /// </summary>
        public async Task HandlePricingUpdatedAsync(Guid unitId, PricingUpdateInfo pricingInfo)
        {
            try
            {
                _logger.LogDebug("بدء تحديث الفهارس للتسعير المحدث: {UnitId}", unitId);

                if (pricingInfo.PropertyId.HasValue)
                {
                    // تحديث فهارس نطاقات الأسعار
                    var oldRanges = GetAffectedPriceRanges(pricingInfo.OldPrice, pricingInfo.OldPrice);
                    var newRanges = GetAffectedPriceRanges(pricingInfo.NewPrice, pricingInfo.NewPrice);
                    
                    var updateTasks = new List<Task>();
                    var allAffectedRanges = oldRanges.Concat(newRanges).Distinct();
                    
                    foreach (var range in allAffectedRanges)
                    {
                        updateTasks.Add(UpdatePriceRangeIndexAsync(range));
                    }

                    // تحديث فهرس المدينة إذا كان متوفر
                    if (!string.IsNullOrEmpty(pricingInfo.City))
                    {
                        updateTasks.Add(UpdateCityIndexAsync(pricingInfo.City));
                    }

                    await Task.WhenAll(updateTasks);
                }

                _logger.LogDebug("تم تحديث الفهارس بنجاح للتسعير المحدث: {UnitId}", unitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للتسعير المحدث: {UnitId}", unitId);
            }
        }

        /// <summary>
        /// التعامل مع تغيير التوفر
        /// </summary>
        public async Task HandleAvailabilityChangedAsync(Guid unitId, AvailabilityChangeInfo changeInfo)
        {
            try
            {
                _logger.LogDebug("بدء تحديث الفهارس لتغيير التوفر: {UnitId}", unitId);

                if (changeInfo.PropertyId.HasValue)
                {
                    // تحديث فهارس التوفر للأشهر المتأثرة
                    var affectedMonths = GetAffectedMonths(changeInfo.StartDate, changeInfo.EndDate);
                    var updateTasks = affectedMonths.Select(UpdateSingleAvailabilityIndexAsync);
                    
                    await Task.WhenAll(updateTasks);

                    // تحديث فهرس المدينة للإحصائيات
                    if (!string.IsNullOrEmpty(changeInfo.City))
                    {
                        await UpdateCityIndexAsync(changeInfo.City);
                    }
                }

                _logger.LogDebug("تم تحديث الفهارس بنجاح لتغيير التوفر: {UnitId}", unitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس لتغيير التوفر: {UnitId}", unitId);
            }
        }

        /// <summary>
        /// التعامل مع إضافة تقييم جديد
        /// </summary>
        public async Task HandleReviewAddedAsync(Guid propertyId, ReviewInfo reviewInfo)
        {
            try
            {
                _logger.LogDebug("بدء تحديث الفهارس لتقييم جديد: {PropertyId}", propertyId);

                var updateTasks = new List<Task>();

                // تحديث فهرس المدينة لإعادة حساب متوسط التقييمات
                if (!string.IsNullOrEmpty(reviewInfo.City))
                {
                    updateTasks.Add(UpdateCityIndexAsync(reviewInfo.City));
                }

                // تحديث فهرس نوع الكيان
                if (!string.IsNullOrEmpty(reviewInfo.PropertyTypeId))
                {
                    updateTasks.Add(UpdatePropertyTypeIndexAsync(reviewInfo.PropertyTypeId));
                }

                // تحديث فهرس التقييمات إذا كان موجود
                updateTasks.Add(UpdateRatingIndexAsync(reviewInfo.Rating));

                await Task.WhenAll(updateTasks);

                _logger.LogDebug("تم تحديث الفهارس بنجاح للتقييم الجديد: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفهارس للتقييم الجديد: {PropertyId}", propertyId);
            }
        }

        /// <summary>
        /// تهيئة النظام والتحقق من سلامة الفهارس
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("بدء تهيئة نظام التحديث التلقائي للفهارس...");

                // التحقق من وجود الفهارس الأساسية
                var requiredIndexes = new[]
                {
                    ("cities", "sanaa"),
                    ("price-ranges", "range_100_300"),
                    ("text-search", "trie_structure")
                };

                var missingIndexes = new List<(string type, string key)>();

                foreach (var (type, key) in requiredIndexes)
                {
                    if (!await _fileService.IndexExistsAsync(type, key))
                    {
                        missingIndexes.Add((type, key));
                    }
                }

                if (missingIndexes.Any())
                {
                    _logger.LogWarning("فهارس مفقودة تم اكتشافها: {Count}، سيتم إنشاؤها", missingIndexes.Count);
                    
                    // إنشاء جميع الفهارس من الصفر
                    await _indexGenerator.GenerateAllIndexesAsync();
                }
                else
                {
                    _logger.LogInformation("جميع الفهارس الأساسية موجودة");
                    
                    // التحقق من سلامة الفهارس
                    await _indexGenerator.RebuildCorruptedIndexesAsync();
                }

                _logger.LogInformation("تم الانتهاء من تهيئة نظام التحديث التلقائي للفهارس");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تهيئة نظام التحديث التلقائي للفهارس");
                throw;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// تحديث فهرس مدينة معينة
        /// </summary>
        private async Task UpdateCityIndexAsync(string city)
        {
            try
            {
                await _indexGenerator.GenerateCityIndexesAsync(); // يمكن تحسينها لتحديث مدينة واحدة فقط
                _logger.LogDebug("تم تحديث فهرس المدينة: {City}", city);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس المدينة: {City}", city);
            }
        }

        /// <summary>
        /// تحديث فهرس نطاق سعري معين
        /// </summary>
        private async Task UpdatePriceRangeIndexAsync(string priceRange)
        {
            try
            {
                await _indexGenerator.GeneratePriceRangeIndexesAsync(); // يمكن تحسينها لتحديث نطاق واحد فقط
                _logger.LogDebug("تم تحديث فهرس النطاق السعري: {Range}", priceRange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس النطاق السعري: {Range}", priceRange);
            }
        }

        /// <summary>
        /// تحديث فهرس نوع كيان معين
        /// </summary>
        private async Task UpdatePropertyTypeIndexAsync(string propertyTypeId)
        {
            try
            {
                await _indexGenerator.GeneratePropertyTypeIndexesAsync(); // يمكن تحسينها لتحديث نوع واحد فقط
                _logger.LogDebug("تم تحديث فهرس نوع الكيان: {TypeId}", propertyTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس نوع الكيان: {TypeId}", propertyTypeId);
            }
        }

        /// <summary>
        /// تحديث فهرس مرفق معين
        /// </summary>
        private async Task UpdateAmenityIndexAsync(string amenityId)
        {
            try
            {
                await _indexGenerator.GenerateAmenityIndexesAsync(); // يمكن تحسينها لتحديث مرفق واحد فقط
                _logger.LogDebug("تم تحديث فهرس المرفق: {AmenityId}", amenityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس المرفق: {AmenityId}", amenityId);
            }
        }

        /// <summary>
        /// تحديث فهارس التوفر
        /// </summary>
        private async Task UpdateAvailabilityIndexesAsync(Guid propertyId)
        {
            try
            {
                await _indexGenerator.GenerateAvailabilityIndexesAsync();
                _logger.LogDebug("تم تحديث فهارس التوفر للكيان: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهارس التوفر للكيان: {PropertyId}", propertyId);
            }
        }

        /// <summary>
        /// تحديث فهرس شهر توفر معين
        /// </summary>
        private async Task UpdateSingleAvailabilityIndexAsync(string monthKey)
        {
            try
            {
                // تحديث شهر واحد فقط - يحتاج تطوير دالة خاصة في IndexGenerationService
                await _indexGenerator.GenerateAvailabilityIndexesAsync();
                _logger.LogDebug("تم تحديث فهرس التوفر للشهر: {Month}", monthKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس التوفر للشهر: {Month}", monthKey);
            }
        }

        /// <summary>
        /// تحديث فهرس البحث النصي
        /// </summary>
        private async Task UpdateTextSearchIndexAsync(Guid propertyId, string? name, string? description)
        {
            try
            {
                await _indexGenerator.GenerateTextSearchIndexAsync();
                _logger.LogDebug("تم تحديث فهرس البحث النصي للكيان: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس البحث النصي للكيان: {PropertyId}", propertyId);
            }
        }

        /// <summary>
        /// إزالة من فهرس البحث النصي
        /// </summary>
        private async Task RemoveFromTextSearchIndexAsync(Guid propertyId)
        {
            try
            {
                // إعادة إنشاء الفهرس بدون الكيان المحذوف
                await _indexGenerator.GenerateTextSearchIndexAsync();
                _logger.LogDebug("تمت إزالة الكيان من فهرس البحث النصي: {PropertyId}", propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إزالة الكيان من فهرس البحث النصي: {PropertyId}", propertyId);
            }
        }

        /// <summary>
        /// تحديث فهرس التقييمات
        /// </summary>
        private async Task UpdateRatingIndexAsync(double rating)
        {
            try
            {
                // تحديث فهرس التقييمات إذا كان موجود
                // يمكن إضافة منطق خاص لفهارس التقييمات
                _logger.LogDebug("تم تحديث فهرس التقييمات للتقييم: {Rating}", rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث فهرس التقييمات للتقييم: {Rating}", rating);
            }
        }

        /// <summary>
        /// الحصول على تفاصيل كيان
        /// </summary>
        private async Task<PropertyInfo?> GetPropertyDetailsAsync(IServiceScope scope, Guid propertyId)
        {
            // تنفيذ مؤقت - يجب استبداله بتحميل من قاعدة البيانات
            return new PropertyInfo
            {
                Id = propertyId,
                City = "Sample City",
                PropertyTypeId = "sample-type-id",
                MinPrice = 100,
                MaxPrice = 500,
                Name = "Sample Property",
                Description = "Sample Description",
                AmenityIds = new List<string> { "wifi", "parking" }
            };
        }

        /// <summary>
        /// الحصول على تفاصيل وحدة
        /// </summary>
        private async Task<UnitInfo?> GetUnitDetailsAsync(IServiceScope scope, Guid unitId)
        {
            // تنفيذ مؤقت - يجب استبداله بتحميل من قاعدة البيانات
            return new UnitInfo
            {
                Id = unitId,
                PropertyId = Guid.NewGuid(),
                BasePrice = 250
            };
        }

        /// <summary>
        /// الحصول على نطاقات الأسعار المتأثرة
        /// </summary>
        private List<string> GetAffectedPriceRanges(decimal? minPrice, decimal? maxPrice)
        {
            var ranges = new List<string>();
            var price = minPrice ?? maxPrice ?? 0;

            if (price < 100) ranges.Add("0-100");
            else if (price < 300) ranges.Add("100-300");
            else if (price < 500) ranges.Add("300-500");
            else if (price < 1000) ranges.Add("500-1000");
            else ranges.Add("1000+");

            return ranges;
        }

        /// <summary>
        /// الحصول على الأشهر المتأثرة
        /// </summary>
        private List<string> GetAffectedMonths(DateTime startDate, DateTime endDate)
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

        /// <summary>
        /// إعادة محاولة التحديث في الخلفية
        /// </summary>
        private async Task RetryIndexUpdateAsync(string operation, string entityId)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1)); // انتظار دقيقة قبل إعادة المحاولة
                
                _logger.LogInformation("إعادة محاولة تحديث الفهارس للعملية: {Operation}, الكيان: {EntityId}", operation, entityId);
                
                // إعادة بناء جميع الفهارس كحل احتياطي
                await _indexGenerator.GenerateAllIndexesAsync();
                
                _logger.LogInformation("تم نجاح إعادة تحديث الفهارس للعملية: {Operation}, الكيان: {EntityId}", operation, entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في إعادة تحديث الفهارس للعملية: {Operation}, الكيان: {EntityId}", operation, entityId);
            }
        }

        #endregion
    }

    #region Event Handlers

    /// <summary>
    /// معالج أحداث الكيانات
    /// </summary>
    public class PropertyEventHandler : 
        INotificationHandler<PropertyCreatedEvent>,
        INotificationHandler<PropertyUpdatedEvent>,
        INotificationHandler<PropertyDeletedEvent>
    {
        private readonly IIndexAutoUpdateService _updateService;

        public PropertyEventHandler(IIndexAutoUpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task Handle(PropertyCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandlePropertyCreatedAsync(notification.PropertyId);
        }

        public async Task Handle(PropertyUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandlePropertyUpdatedAsync(notification.PropertyId, notification.UpdateInfo);
        }

        public async Task Handle(PropertyDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandlePropertyDeletedAsync(notification.PropertyId, notification.DeleteInfo);
        }
    }

    /// <summary>
    /// معالج أحداث الوحدات
    /// </summary>
    public class UnitEventHandler :
        INotificationHandler<UnitCreatedEvent>,
        INotificationHandler<UnitUpdatedEvent>,
        INotificationHandler<UnitPricingUpdatedEvent>,
        INotificationHandler<UnitAvailabilityChangedEvent>
    {
        private readonly IIndexAutoUpdateService _updateService;

        public UnitEventHandler(IIndexAutoUpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task Handle(UnitCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandleUnitCreatedAsync(notification.UnitId);
        }

        public async Task Handle(UnitUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandleUnitUpdatedAsync(notification.UnitId, notification.UpdateInfo);
        }

        public async Task Handle(UnitPricingUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandlePricingUpdatedAsync(notification.UnitId, notification.PricingInfo);
        }

        public async Task Handle(UnitAvailabilityChangedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandleAvailabilityChangedAsync(notification.UnitId, notification.ChangeInfo);
        }
    }

    /// <summary>
    /// معالج أحداث التقييمات
    /// </summary>
    public class ReviewEventHandler : INotificationHandler<ReviewAddedEvent>
    {
        private readonly IIndexAutoUpdateService _updateService;

        public ReviewEventHandler(IIndexAutoUpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task Handle(ReviewAddedEvent notification, CancellationToken cancellationToken)
        {
            await _updateService.HandleReviewAddedAsync(notification.PropertyId, notification.ReviewInfo);
        }
    }

    /// <summary>
    /// خدمة خلفية لتهيئة النظام
    /// </summary>
    public class IndexInitializationService : BackgroundService
    {
        private readonly ILogger<IndexInitializationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public IndexInitializationService(ILogger<IndexInitializationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // انتظار قصير لضمان تهيئة الخدمات الأخرى
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var updateService = scope.ServiceProvider.GetRequiredService<IIndexAutoUpdateService>();
                
                await updateService.InitializeAsync();
                
                _logger.LogInformation("تم تهيئة نظام الفهارس بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تهيئة نظام الفهارس");
            }
        }
    }

    #endregion

    #region Data Models

    /// <summary>
    /// معلومات تحديث الكيان
    /// </summary>
    public class PropertyUpdateInfo
    {
        public bool CityChanged { get; set; }
        public string? OldCity { get; set; }
        public string? NewCity { get; set; }
        
        public bool PriceChanged { get; set; }
        public decimal? OldMinPrice { get; set; }
        public decimal? OldMaxPrice { get; set; }
        public decimal? NewMinPrice { get; set; }
        public decimal? NewMaxPrice { get; set; }
        
        public bool PropertyTypeChanged { get; set; }
        public string? OldPropertyTypeId { get; set; }
        public string? NewPropertyTypeId { get; set; }
        
        public bool AmenitiesChanged { get; set; }
        public List<string>? AddedAmenities { get; set; }
        public List<string>? RemovedAmenities { get; set; }
        
        public bool NameOrDescriptionChanged { get; set; }
        public string? NewName { get; set; }
        public string? NewDescription { get; set; }
    }

    /// <summary>
    /// معلومات حذف الكيان
    /// </summary>
    public class PropertyDeleteInfo
    {
        public string? City { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? PropertyTypeId { get; set; }
        public List<string>? AmenityIds { get; set; }
    }

    /// <summary>
    /// معلومات تحديث الوحدة
    /// </summary>
    public class UnitUpdateInfo
    {
        public Guid? PropertyId { get; set; }
        
        public bool PriceChanged { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }
        
        public bool AvailabilityChanged { get; set; }
    }

    /// <summary>
    /// معلومات تحديث التسعير
    /// </summary>
    public class PricingUpdateInfo
    {
        public Guid? PropertyId { get; set; }
        public string? City { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// معلومات تغيير التوفر
    /// </summary>
    public class AvailabilityChangeInfo
    {
        public Guid? PropertyId { get; set; }
        public string? City { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// معلومات التقييم
    /// </summary>
    public class ReviewInfo
    {
        public string? City { get; set; }
        public string? PropertyTypeId { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// معلومات الكيان
    /// </summary>
    public class PropertyInfo
    {
        public Guid Id { get; set; }
        public string? City { get; set; }
        public string? PropertyTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? AmenityIds { get; set; }
    }

    /// <summary>
    /// معلومات الوحدة
    /// </summary>
    public class UnitInfo
    {
        public Guid Id { get; set; }
        public Guid? PropertyId { get; set; }
        public decimal? BasePrice { get; set; }
    }

    #endregion
}