using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookN.Core.Services.IndexManagement
{
    /// <summary>
    /// خدمة إدارة ملفات JSON للفهرسة المسبقة
    /// تتولى إنشاء وقراءة وتحديث وحذف ملفات الفهارس
    /// </summary>
    public interface IJsonIndexFileService
    {
        /// <summary>
        /// قراءة فهرس معين من الملف
        /// </summary>
        Task<T?> ReadIndexAsync<T>(string indexType, string indexKey) where T : class;
        
        /// <summary>
        /// كتابة فهرس إلى ملف
        /// </summary>
        Task WriteIndexAsync<T>(string indexType, string indexKey, T data) where T : class;
        
        /// <summary>
        /// حذف فهرس معين
        /// </summary>
        Task DeleteIndexAsync(string indexType, string indexKey);
        
        /// <summary>
        /// التحقق من وجود فهرس
        /// </summary>
        Task<bool> IndexExistsAsync(string indexType, string indexKey);
        
        /// <summary>
        /// الحصول على جميع مفاتيح فهرس معين
        /// </summary>
        Task<IEnumerable<string>> GetIndexKeysAsync(string indexType);
        
        /// <summary>
        /// ضغط وتحسين ملفات الفهارس
        /// </summary>
        Task OptimizeIndexFilesAsync();
        
        /// <summary>
        /// تنظيف الملفات القديمة والمنتهية الصلاحية
        /// </summary>
        Task CleanupExpiredIndexesAsync();
    }

    public class JsonIndexFileService : IJsonIndexFileService
    {
        private readonly ILogger<JsonIndexFileService> _logger;
        private readonly IndexConfiguration _config;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks;
        private readonly ConcurrentDictionary<string, (DateTime lastAccess, object data)> _hotCache;

        public JsonIndexFileService(
            ILogger<JsonIndexFileService> logger,
            IOptions<IndexConfiguration> config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
            _fileLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _hotCache = new ConcurrentDictionary<string, (DateTime, object)>();
            
            // إنشاء المجلدات الأساسية إذا لم تكن موجودة
            EnsureDirectoriesExist();
        }

        /// <summary>
        /// قراءة فهرس معين من الملف مع دعم التخزين المؤقت
        /// </summary>
        public async Task<T?> ReadIndexAsync<T>(string indexType, string indexKey) where T : class
        {
            try
            {
                var cacheKey = $"{indexType}:{indexKey}";
                
                // فحص الكاش الساخن أولاً
                if (_hotCache.TryGetValue(cacheKey, out var cachedItem))
                {
                    // تحديث وقت آخر وصول
                    _hotCache[cacheKey] = (DateTime.UtcNow, cachedItem.data);
                    
                    _logger.LogDebug("تم استرجاع الفهرس من الكاش الساخن: {IndexType}:{IndexKey}", indexType, indexKey);
                    return (T)cachedItem.data;
                }

                var filePath = GetIndexFilePath(indexType, indexKey);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("ملف الفهرس غير موجود: {FilePath}", filePath);
                    return null;
                }

                // الحصول على قفل الملف لضمان القراءة الآمنة
                var semaphore = _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync();

                try
                {
                    var jsonContent = await File.ReadAllTextAsync(filePath);
                    var data = JsonSerializer.Deserialize<T>(jsonContent, GetJsonSerializerOptions());

                    // إضافة إلى الكاش الساخن إذا كان الحجم مناسب
                    if (ShouldCacheInMemory(jsonContent.Length))
                    {
                        _hotCache[cacheKey] = (DateTime.UtcNow, data!);
                    }

                    _logger.LogDebug("تم قراءة الفهرس بنجاح: {IndexType}:{IndexKey}", indexType, indexKey);
                    return data;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في قراءة الفهرس: {IndexType}:{IndexKey}", indexType, indexKey);
                return null;
            }
        }

        /// <summary>
        /// كتابة فهرس إلى ملف مع ضمان سلامة البيانات
        /// </summary>
        public async Task WriteIndexAsync<T>(string indexType, string indexKey, T data) where T : class
        {
            try
            {
                var filePath = GetIndexFilePath(indexType, indexKey);
                var directory = Path.GetDirectoryName(filePath);
                
                // إنشاء المجلد إذا لم يكن موجود
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // الحصول على قفل الملف لضمان الكتابة الآمنة
                var semaphore = _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync();

                try
                {
                    // كتابة إلى ملف مؤقت أولاً لضمان سلامة البيانات
                    var tempFilePath = filePath + ".tmp";
                    var jsonContent = JsonSerializer.Serialize(data, GetJsonSerializerOptions());
                    
                    await File.WriteAllTextAsync(tempFilePath, jsonContent);
                    
                    // نقل الملف المؤقت إلى الملف الأصلي (عملية ذرية)
                    File.Move(tempFilePath, filePath, true);

                    // تحديث الكاش الساخن
                    var cacheKey = $"{indexType}:{indexKey}";
                    if (ShouldCacheInMemory(jsonContent.Length))
                    {
                        _hotCache[cacheKey] = (DateTime.UtcNow, data);
                    }
                    else
                    {
                        // إزالة من الكاش إذا كان كبير الحجم
                        _hotCache.TryRemove(cacheKey, out _);
                    }

                    _logger.LogDebug("تم حفظ الفهرس بنجاح: {IndexType}:{IndexKey}", indexType, indexKey);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حفظ الفهرس: {IndexType}:{IndexKey}", indexType, indexKey);
                throw;
            }
        }

        /// <summary>
        /// حذف فهرس معين مع تنظيف الكاش
        /// </summary>
        public async Task DeleteIndexAsync(string indexType, string indexKey)
        {
            try
            {
                var filePath = GetIndexFilePath(indexType, indexKey);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("محاولة حذف ملف غير موجود: {FilePath}", filePath);
                    return;
                }

                // الحصول على قفل الملف
                var semaphore = _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync();

                try
                {
                    File.Delete(filePath);
                    
                    // إزالة من الكاش الساخن
                    var cacheKey = $"{indexType}:{indexKey}";
                    _hotCache.TryRemove(cacheKey, out _);

                    _logger.LogDebug("تم حذف الفهرس بنجاح: {IndexType}:{IndexKey}", indexType, indexKey);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف الفهرس: {IndexType}:{IndexKey}", indexType, indexKey);
                throw;
            }
        }

        /// <summary>
        /// التحقق من وجود فهرس معين
        /// </summary>
        public async Task<bool> IndexExistsAsync(string indexType, string indexKey)
        {
            try
            {
                // فحص الكاش الساخن أولاً
                var cacheKey = $"{indexType}:{indexKey}";
                if (_hotCache.ContainsKey(cacheKey))
                {
                    return true;
                }

                var filePath = GetIndexFilePath(indexType, indexKey);
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في التحقق من وجود الفهرس: {IndexType}:{IndexKey}", indexType, indexKey);
                return false;
            }
        }

        /// <summary>
        /// الحصول على جميع مفاتيح فهرس معين
        /// </summary>
        public async Task<IEnumerable<string>> GetIndexKeysAsync(string indexType)
        {
            try
            {
                var indexDirectory = Path.Combine(_config.IndexFilesPath, "Properties", indexType);
                
                if (!Directory.Exists(indexDirectory))
                {
                    return Enumerable.Empty<string>();
                }

                var files = Directory.GetFiles(indexDirectory, "*.json", SearchOption.AllDirectories);
                var keys = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();

                _logger.LogDebug("تم العثور على {Count} مفتاح فهرس للنوع: {IndexType}", keys.Count, indexType);
                return keys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على مفاتيح الفهرس للنوع: {IndexType}", indexType);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// ضغط وتحسين ملفات الفهارس
        /// </summary>
        public async Task OptimizeIndexFilesAsync()
        {
            try
            {
                _logger.LogInformation("بدء عملية تحسين ملفات الفهارس...");

                var indexTypes = new[] { "cities", "price-ranges", "amenities", "property-types", "availability" };
                var tasks = indexTypes.Select(OptimizeIndexTypeAsync);
                
                await Task.WhenAll(tasks);

                // تنظيف الكاش الساخن من البيانات القديمة
                await CleanupHotCacheAsync();

                _logger.LogInformation("تم الانتهاء من تحسين ملفات الفهارس");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحسين ملفات الفهارس");
                throw;
            }
        }

        /// <summary>
        /// تنظيف الملفات القديمة والمنتهية الصلاحية
        /// </summary>
        public async Task CleanupExpiredIndexesAsync()
        {
            try
            {
                _logger.LogInformation("بدء عملية تنظيف الفهارس المنتهية الصلاحية...");

                var cutoffDate = DateTime.UtcNow.AddDays(-_config.IndexExpiryDays);
                var deletedCount = 0;

                await foreach (var filePath in GetAllIndexFilesAsync())
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.LastWriteTimeUtc < cutoffDate)
                    {
                        try
                        {
                            File.Delete(filePath);
                            deletedCount++;
                            
                            // إزالة من الكاش أيضاً
                            var relativePath = Path.GetRelativePath(_config.IndexFilesPath, filePath);
                            var cacheKey = relativePath.Replace(Path.DirectorySeparatorChar, ':').Replace(".json", "");
                            _hotCache.TryRemove(cacheKey, out _);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "فشل في حذف الملف المنتهي الصلاحية: {FilePath}", filePath);
                        }
                    }
                }

                _logger.LogInformation("تم حذف {Count} ملف فهرس منتهي الصلاحية", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تنظيف الفهارس المنتهية الصلاحية");
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// الحصول على مسار ملف الفهرس
        /// </summary>
        private string GetIndexFilePath(string indexType, string indexKey)
        {
            var fileName = $"{indexKey}.json";
            return Path.Combine(_config.IndexFilesPath, "Properties", indexType, fileName);
        }

        /// <summary>
        /// إعدادات تسلسل JSON
        /// </summary>
        private JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _config.PrettyPrintJson,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        /// <summary>
        /// تحديد ما إذا كانت البيانات يجب حفظها في الكاش الساخن
        /// </summary>
        private bool ShouldCacheInMemory(int dataSize)
        {
            return dataSize <= _config.MaxHotCacheItemSize && 
                   _hotCache.Count < _config.MaxHotCacheItems;
        }

        /// <summary>
        /// إنشاء المجلدات الأساسية
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            var indexTypes = new[] { "cities", "price-ranges", "amenities", "property-types", "availability", "text-search", "ratings" };
            
            foreach (var indexType in indexTypes)
            {
                var directory = Path.Combine(_config.IndexFilesPath, "Properties", indexType);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        /// <summary>
        /// تحسين نوع فهرس معين
        /// </summary>
        private async Task OptimizeIndexTypeAsync(string indexType)
        {
            var directory = Path.Combine(_config.IndexFilesPath, "Properties", indexType);
            if (!Directory.Exists(directory))
                return;

            var files = Directory.GetFiles(directory, "*.json");
            var optimizationTasks = files.Select(OptimizeFileAsync);
            
            await Task.WhenAll(optimizationTasks);
        }

        /// <summary>
        /// تحسين ملف واحد
        /// </summary>
        private async Task OptimizeFileAsync(string filePath)
        {
            try
            {
                var content = await File.ReadAllTextAsync(filePath);
                var document = JsonDocument.Parse(content);
                
                // إعادة تسلسل بتنسيق محسن
                var optimizedContent = JsonSerializer.Serialize(document.RootElement, GetJsonSerializerOptions());
                
                // كتابة فقط إذا كان هناك تحسن في الحجم
                if (optimizedContent.Length < content.Length)
                {
                    await File.WriteAllTextAsync(filePath, optimizedContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في تحسين الملف: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// الحصول على جميع ملفات الفهارس
        /// </summary>
        private async IAsyncEnumerable<string> GetAllIndexFilesAsync()
        {
            var indexDirectory = Path.Combine(_config.IndexFilesPath, "Properties");
            if (!Directory.Exists(indexDirectory))
                yield break;

            var files = Directory.GetFiles(indexDirectory, "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                yield return file;
            }
        }

        /// <summary>
        /// تنظيف الكاش الساخن من البيانات القديمة
        /// </summary>
        private async Task CleanupHotCacheAsync()
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-_config.HotCacheExpiryMinutes);
            var expiredKeys = _hotCache
                .Where(kvp => kvp.Value.lastAccess < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _hotCache.TryRemove(key, out _);
            }

            _logger.LogDebug("تم تنظيف {Count} عنصر من الكاش الساخن", expiredKeys.Count);
        }

        #endregion
    }

    /// <summary>
    /// إعدادات خدمة الفهرسة
    /// </summary>
    public class IndexConfiguration
    {
        /// <summary>
        /// مسار مجلد ملفات الفهارس
        /// </summary>
        public string IndexFilesPath { get; set; } = "IndexFiles";

        /// <summary>
        /// عدد أيام انتهاء صلاحية الفهارس
        /// </summary>
        public int IndexExpiryDays { get; set; } = 30;

        /// <summary>
        /// الحد الأقصى لحجم عنصر الكاش الساخن بالبايت
        /// </summary>
        public int MaxHotCacheItemSize { get; set; } = 1024 * 1024; // 1MB

        /// <summary>
        /// العدد الأقصى للعناصر في الكاش الساخن
        /// </summary>
        public int MaxHotCacheItems { get; set; } = 1000;

        /// <summary>
        /// مدة انتهاء صلاحية الكاش الساخن بالدقائق
        /// </summary>
        public int HotCacheExpiryMinutes { get; set; } = 30;

        /// <summary>
        /// تنسيق JSON بشكل جميل للقراءة
        /// </summary>
        public bool PrettyPrintJson { get; set; } = false;
    }
}