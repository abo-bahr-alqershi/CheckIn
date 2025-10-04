using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BookN.Core.Events
{
    #region Property Events

    /// <summary>
    /// حدث إنشاء كيان جديد
    /// يتم إرساله عند إضافة كيان جديد لإطلاق تحديث الفهارس
    /// </summary>
    public class PropertyCreatedEvent : INotification
    {
        /// <summary>
        /// معرف الكيان الجديد
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات أساسية عن الكيان
        /// </summary>
        public PropertyEventInfo PropertyInfo { get; set; } = new();

        /// <summary>
        /// توقيت إنشاء الكيان
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي أنشأ الكيان
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// مصدر الحدث (api, import, migration, etc.)
        /// </summary>
        public string Source { get; set; } = "api";

        /// <summary>
        /// معلومات إضافية
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// حدث تحديث كيان موجود
    /// يتم إرساله عند تعديل كيان لإطلاق تحديث الفهارس المتأثرة
    /// </summary>
    public class PropertyUpdatedEvent : INotification
    {
        /// <summary>
        /// معرف الكيان المُحدث
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات التحديث التفصيلية
        /// </summary>
        public PropertyUpdateInfo UpdateInfo { get; set; } = new();

        /// <summary>
        /// القيم القديمة قبل التحديث
        /// </summary>
        public PropertyEventInfo OldValues { get; set; } = new();

        /// <summary>
        /// القيم الجديدة بعد التحديث
        /// </summary>
        public PropertyEventInfo NewValues { get; set; } = new();

        /// <summary>
        /// توقيت التحديث
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي حدث الكيان
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// نوع التحديث (manual, automatic, batch, etc.)
        /// </summary>
        public string UpdateType { get; set; } = "manual";

        /// <summary>
        /// الحقول التي تم تحديثها
        /// </summary>
        public List<string> UpdatedFields { get; set; } = new();
    }

    /// <summary>
    /// حدث حذف كيان
    /// يتم إرساله عند حذف كيان لإطلاق إزالته من الفهارس
    /// </summary>
    public class PropertyDeletedEvent : INotification
    {
        /// <summary>
        /// معرف الكيان المحذوف
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات الحذف
        /// </summary>
        public PropertyDeleteInfo DeleteInfo { get; set; } = new();

        /// <summary>
        /// القيم الأخيرة قبل الحذف (للتنظيف من الفهارس)
        /// </summary>
        public PropertyEventInfo LastValues { get; set; } = new();

        /// <summary>
        /// توقيت الحذف
        /// </summary>
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي حذف الكيان
        /// </summary>
        public string? DeletedBy { get; set; }

        /// <summary>
        /// نوع الحذف (soft, hard, cascade, etc.)
        /// </summary>
        public string DeleteType { get; set; } = "soft";

        /// <summary>
        /// سبب الحذف
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// حدث استعادة كيان محذوف
    /// يتم إرساله عند استعادة كيان من سلة المحذوفات
    /// </summary>
    public class PropertyRestoredEvent : INotification
    {
        /// <summary>
        /// معرف الكيان المستعاد
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات الكيان بعد الاستعادة
        /// </summary>
        public PropertyEventInfo PropertyInfo { get; set; } = new();

        /// <summary>
        /// توقيت الاستعادة
        /// </summary>
        public DateTime RestoredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي استعاد الكيان
        /// </summary>
        public string? RestoredBy { get; set; }
    }

    #endregion

    #region Unit Events

    /// <summary>
    /// حدث إنشاء وحدة جديدة
    /// </summary>
    public class UnitCreatedEvent : INotification
    {
        /// <summary>
        /// معرف الوحدة الجديدة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معرف الكيان المالك
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات الوحدة
        /// </summary>
        public UnitEventInfo UnitInfo { get; set; } = new();

        /// <summary>
        /// توقيت الإنشاء
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المنشئ
        /// </summary>
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// حدث تحديث وحدة موجودة
    /// </summary>
    public class UnitUpdatedEvent : INotification
    {
        /// <summary>
        /// معرف الوحدة المُحدثة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معلومات التحديث
        /// </summary>
        public UnitUpdateInfo UpdateInfo { get; set; } = new();

        /// <summary>
        /// القيم القديمة
        /// </summary>
        public UnitEventInfo OldValues { get; set; } = new();

        /// <summary>
        /// القيم الجديدة
        /// </summary>
        public UnitEventInfo NewValues { get; set; } = new();

        /// <summary>
        /// توقيت التحديث
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحدث
        /// </summary>
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// حدث حذف وحدة
    /// </summary>
    public class UnitDeletedEvent : INotification
    {
        /// <summary>
        /// معرف الوحدة المحذوفة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معرف الكيان المالك
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// القيم الأخيرة قبل الحذف
        /// </summary>
        public UnitEventInfo LastValues { get; set; } = new();

        /// <summary>
        /// توقيت الحذف
        /// </summary>
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحذف
        /// </summary>
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// حدث تحديث تسعير وحدة
    /// </summary>
    public class UnitPricingUpdatedEvent : INotification
    {
        /// <summary>
        /// معرف الوحدة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معلومات التسعير الجديد
        /// </summary>
        public PricingUpdateInfo PricingInfo { get; set; } = new();

        /// <summary>
        /// توقيت تحديث التسعير
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحدث
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// نوع تحديث التسعير (base_price, seasonal, promotional, etc.)
        /// </summary>
        public string PricingUpdateType { get; set; } = "base_price";
    }

    /// <summary>
    /// حدث تغيير توفر وحدة
    /// </summary>
    public class UnitAvailabilityChangedEvent : INotification
    {
        /// <summary>
        /// معرف الوحدة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معلومات تغيير التوفر
        /// </summary>
        public AvailabilityChangeInfo ChangeInfo { get; set; } = new();

        /// <summary>
        /// توقيت تغيير التوفر
        /// </summary>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحدث
        /// </summary>
        public string? ChangedBy { get; set; }

        /// <summary>
        /// سبب تغيير التوفر
        /// </summary>
        public string? ChangeReason { get; set; }
    }

    #endregion

    #region Review Events

    /// <summary>
    /// حدث إضافة تقييم جديد
    /// </summary>
    public class ReviewAddedEvent : INotification
    {
        /// <summary>
        /// معرف التقييم
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// معرف الكيان المُقيم
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معرف الحجز المرتبط
        /// </summary>
        public Guid? BookingId { get; set; }

        /// <summary>
        /// معلومات التقييم
        /// </summary>
        public ReviewInfo ReviewInfo { get; set; } = new();

        /// <summary>
        /// توقيت إضافة التقييم
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُقيم
        /// </summary>
        public string? ReviewedBy { get; set; }
    }

    /// <summary>
    /// حدث تحديث تقييم موجود
    /// </summary>
    public class ReviewUpdatedEvent : INotification
    {
        /// <summary>
        /// معرف التقييم
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات التقييم القديمة
        /// </summary>
        public ReviewInfo OldReviewInfo { get; set; } = new();

        /// <summary>
        /// معلومات التقييم الجديدة
        /// </summary>
        public ReviewInfo NewReviewInfo { get; set; } = new();

        /// <summary>
        /// توقيت التحديث
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحدث
        /// </summary>
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// حدث حذف تقييم
    /// </summary>
    public class ReviewDeletedEvent : INotification
    {
        /// <summary>
        /// معرف التقييم المحذوف
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات التقييم المحذوف
        /// </summary>
        public ReviewInfo DeletedReviewInfo { get; set; } = new();

        /// <summary>
        /// توقيت الحذف
        /// </summary>
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المُحذف
        /// </summary>
        public string? DeletedBy { get; set; }

        /// <summary>
        /// سبب الحذف
        /// </summary>
        public string? DeletionReason { get; set; }
    }

    #endregion

    #region Booking Events

    /// <summary>
    /// حدث إنشاء حجز جديد
    /// </summary>
    public class BookingCreatedEvent : INotification
    {
        /// <summary>
        /// معرف الحجز
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// معرف الوحدة المحجوزة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات الحجز
        /// </summary>
        public BookingInfo BookingInfo { get; set; } = new();

        /// <summary>
        /// توقيت الحجز
        /// </summary>
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي قام بالحجز
        /// </summary>
        public string? BookedBy { get; set; }
    }

    /// <summary>
    /// حدث إلغاء حجز
    /// </summary>
    public class BookingCancelledEvent : INotification
    {
        /// <summary>
        /// معرف الحجز الملغي
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// معرف الوحدة
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معلومات الحجز الملغي
        /// </summary>
        public BookingInfo BookingInfo { get; set; } = new();

        /// <summary>
        /// توقيت الإلغاء
        /// </summary>
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف من ألغى الحجز
        /// </summary>
        public string? CancelledBy { get; set; }

        /// <summary>
        /// سبب الإلغاء
        /// </summary>
        public string? CancellationReason { get; set; }
    }

    #endregion

    #region Index Events

    /// <summary>
    /// حدث اكتمال إنشاء جميع الفهارس
    /// </summary>
    public class IndexesGeneratedNotification : INotification
    {
        /// <summary>
        /// توقيت اكتمال الفهرسة
        /// </summary>
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// عدد الفهارس التي تم إنشاؤها
        /// </summary>
        public int GeneratedIndexCount { get; set; }

        /// <summary>
        /// الوقت المستغرق في الإنشاء
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// إحصائيات الفهارس
        /// </summary>
        public Dictionary<string, int> IndexStatistics { get; set; } = new();
    }

    /// <summary>
    /// حدث تحديث فهرس معين
    /// </summary>
    public class IndexUpdatedNotification : INotification
    {
        /// <summary>
        /// نوع الفهرس
        /// </summary>
        public string IndexType { get; set; } = string.Empty;

        /// <summary>
        /// مفتاح الفهرس
        /// </summary>
        public string IndexKey { get; set; } = string.Empty;

        /// <summary>
        /// توقيت التحديث
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// سبب التحديث
        /// </summary>
        public string UpdateReason { get; set; } = string.Empty;

        /// <summary>
        /// الوقت المستغرق في التحديث
        /// </summary>
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// حدث فشل تحديث فهرس
    /// </summary>
    public class IndexUpdateFailedNotification : INotification
    {
        /// <summary>
        /// نوع الفهرس
        /// </summary>
        public string IndexType { get; set; } = string.Empty;

        /// <summary>
        /// مفتاح الفهرس
        /// </summary>
        public string IndexKey { get; set; } = string.Empty;

        /// <summary>
        /// تفاصيل الخطأ
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// رمز الخطأ
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// توقيت حدوث الخطأ
        /// </summary>
        public DateTime FailedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// عدد محاولات إعادة التحديث
        /// </summary>
        public int RetryCount { get; set; }
    }

    #endregion

    #region Supporting Models

    /// <summary>
    /// معلومات أساسية عن الكيان للأحداث
    /// </summary>
    public class PropertyEventInfo
    {
        /// <summary>
        /// اسم الكيان
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وصف الكيان
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// المدينة
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// العنوان
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// نوع الكيان
        /// </summary>
        public string PropertyTypeId { get; set; } = string.Empty;

        /// <summary>
        /// المرافق
        /// </summary>
        public List<string> AmenityIds { get; set; } = new();

        /// <summary>
        /// الأسعار
        /// </summary>
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// الموقع الجغرافي
        /// </summary>
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        /// <summary>
        /// التقييم النجمي
        /// </summary>
        public int? StarRating { get; set; }

        /// <summary>
        /// حالة النشاط
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// حالة الموافقة
        /// </summary>
        public bool IsApproved { get; set; }
    }

    /// <summary>
    /// معلومات الوحدة للأحداث
    /// </summary>
    public class UnitEventInfo
    {
        /// <summary>
        /// اسم الوحدة
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// نوع الوحدة
        /// </summary>
        public string UnitTypeId { get; set; } = string.Empty;

        /// <summary>
        /// السعر الأساسي
        /// </summary>
        public decimal? BasePrice { get; set; }

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "SAR";

        /// <summary>
        /// السعة القصوى
        /// </summary>
        public int? MaxCapacity { get; set; }

        /// <summary>
        /// المميزات المخصصة
        /// </summary>
        public string? CustomFeatures { get; set; }

        /// <summary>
        /// حالة التوفر
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// طريقة التسعير
        /// </summary>
        public int PricingMethod { get; set; }
    }

    /// <summary>
    /// معلومات التقييم للأحداث
    /// </summary>
    public class ReviewInfo
    {
        /// <summary>
        /// المدينة (للفهرسة)
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// نوع الكيان (للفهرسة)
        /// </summary>
        public string PropertyTypeId { get; set; } = string.Empty;

        /// <summary>
        /// التقييم الإجمالي
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// تقييم النظافة
        /// </summary>
        public int? Cleanliness { get; set; }

        /// <summary>
        /// تقييم الخدمة
        /// </summary>
        public int? Service { get; set; }

        /// <summary>
        /// تقييم الموقع
        /// </summary>
        public int? Location { get; set; }

        /// <summary>
        /// تقييم القيمة مقابل المال
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// التعليق
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// هل في انتظار الموافقة
        /// </summary>
        public bool IsPendingApproval { get; set; }
    }

    /// <summary>
    /// معلومات الحجز للأحداث
    /// </summary>
    public class BookingInfo
    {
        /// <summary>
        /// تاريخ تسجيل الوصول
        /// </summary>
        public DateTime CheckIn { get; set; }

        /// <summary>
        /// تاريخ تسجيل المغادرة
        /// </summary>
        public DateTime CheckOut { get; set; }

        /// <summary>
        /// عدد الضيوف
        /// </summary>
        public int GuestsCount { get; set; }

        /// <summary>
        /// إجمالي السعر
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "SAR";

        /// <summary>
        /// حالة الحجز
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// مصدر الحجز
        /// </summary>
        public string? BookingSource { get; set; }

        /// <summary>
        /// هل حجز مباشر
        /// </summary>
        public bool IsWalkIn { get; set; }
    }

    /// <summary>
    /// معلومات تحديث التسعير للأحداث
    /// </summary>
    public class PricingUpdateInfo
    {
        /// <summary>
        /// معرف الكيان (للفهرسة)
        /// </summary>
        public Guid? PropertyId { get; set; }

        /// <summary>
        /// المدينة (للفهرسة)
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// السعر القديم
        /// </summary>
        public decimal? OldPrice { get; set; }

        /// <summary>
        /// السعر الجديد
        /// </summary>
        public decimal? NewPrice { get; set; }

        /// <summary>
        /// تاريخ بداية السعر
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية السعر
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// نوع السعر (base, seasonal, promotional, etc.)
        /// </summary>
        public string PriceType { get; set; } = "base";

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "SAR";
    }

    /// <summary>
    /// معلومات تغيير التوفر للأحداث
    /// </summary>
    public class AvailabilityChangeInfo
    {
        /// <summary>
        /// معرف الكيان (للفهرسة)
        /// </summary>
        public Guid? PropertyId { get; set; }

        /// <summary>
        /// المدينة (للفهرسة)
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// تاريخ البداية
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// الحالة الجديدة (Available, Booked, Blocked, etc.)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// سبب التغيير
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// ملاحظات إضافية
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// معرف الحجز المرتبط (إن وجد)
        /// </summary>
        public Guid? BookingId { get; set; }
    }

    /// <summary>
    /// معلومات تحديث الكيان للأحداث
    /// </summary>
    public class PropertyUpdateInfo
    {
        /// <summary>
        /// هل تغيرت المدينة
        /// </summary>
        public bool CityChanged { get; set; }
        public string? OldCity { get; set; }
        public string? NewCity { get; set; }

        /// <summary>
        /// هل تغيرت الأسعار
        /// </summary>
        public bool PriceChanged { get; set; }
        public decimal? OldMinPrice { get; set; }
        public decimal? OldMaxPrice { get; set; }
        public decimal? NewMinPrice { get; set; }
        public decimal? NewMaxPrice { get; set; }

        /// <summary>
        /// هل تغير نوع الكيان
        /// </summary>
        public bool PropertyTypeChanged { get; set; }
        public string? OldPropertyTypeId { get; set; }
        public string? NewPropertyTypeId { get; set; }

        /// <summary>
        /// هل تغيرت المرافق
        /// </summary>
        public bool AmenitiesChanged { get; set; }
        public List<string>? AddedAmenities { get; set; }
        public List<string>? RemovedAmenities { get; set; }

        /// <summary>
        /// هل تغير الاسم أو الوصف
        /// </summary>
        public bool NameOrDescriptionChanged { get; set; }
        public string? NewName { get; set; }
        public string? NewDescription { get; set; }

        /// <summary>
        /// هل تغير الموقع الجغرافي
        /// </summary>
        public bool LocationChanged { get; set; }
        public double? OldLatitude { get; set; }
        public double? OldLongitude { get; set; }
        public double? NewLatitude { get; set; }
        public double? NewLongitude { get; set; }

        /// <summary>
        /// هل تغيرت حالة النشاط أو الموافقة
        /// </summary>
        public bool StatusChanged { get; set; }
        public bool? OldIsActive { get; set; }
        public bool? NewIsActive { get; set; }
        public bool? OldIsApproved { get; set; }
        public bool? NewIsApproved { get; set; }
    }

    /// <summary>
    /// معلومات حذف الكيان للأحداث
    /// </summary>
    public class PropertyDeleteInfo
    {
        /// <summary>
        /// المدينة (للتنظيف من الفهارس)
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// الأسعار (للتنظيف من فهارس الأسعار)
        /// </summary>
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// نوع الكيان (للتنظيف من فهارس الأنواع)
        /// </summary>
        public string? PropertyTypeId { get; set; }

        /// <summary>
        /// المرافق (للتنظيف من فهارس المرافق)
        /// </summary>
        public List<string>? AmenityIds { get; set; }

        /// <summary>
        /// عدد الوحدات المرتبطة
        /// </summary>
        public int RelatedUnitsCount { get; set; }

        /// <summary>
        /// عدد الحجوزات النشطة (قد تمنع الحذف)
        /// </summary>
        public int ActiveBookingsCount { get; set; }
    }

    /// <summary>
    /// معلومات تحديث الوحدة للأحداث
    /// </summary>
    public class UnitUpdateInfo
    {
        /// <summary>
        /// معرف الكيان المالك
        /// </summary>
        public Guid? PropertyId { get; set; }

        /// <summary>
        /// هل تغير السعر
        /// </summary>
        public bool PriceChanged { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }

        /// <summary>
        /// هل تغير التوفر
        /// </summary>
        public bool AvailabilityChanged { get; set; }
        public bool? OldIsAvailable { get; set; }
        public bool? NewIsAvailable { get; set; }

        /// <summary>
        /// هل تغير نوع الوحدة
        /// </summary>
        public bool UnitTypeChanged { get; set; }
        public string? OldUnitTypeId { get; set; }
        public string? NewUnitTypeId { get; set; }

        /// <summary>
        /// هل تغيرت السعة
        /// </summary>
        public bool CapacityChanged { get; set; }
        public int? OldMaxCapacity { get; set; }
        public int? NewMaxCapacity { get; set; }

        /// <summary>
        /// هل تغيرت المميزات المخصصة
        /// </summary>
        public bool CustomFeaturesChanged { get; set; }
        public string? OldCustomFeatures { get; set; }
        public string? NewCustomFeatures { get; set; }
    }

    #endregion
}