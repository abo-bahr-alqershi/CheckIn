# City Images Deduplication and Optional Unique Index

هذا المستند يوضح كيفية تنظيف البيانات الحالية من التكرارات في جدول `PropertyImages` وربما إضافة قيد فريد اختياري لمنع التكرار مستقبلًا (مع اعتبار `IsDeleted`).

## 1) تنظيف التكرارات الحالية (SQL Server)

يبقي الاستعلام أدناه أفضل نسخة لكل زوج `(CityName, Url)` بناءً على أفضلية الصورة الرئيسية ثم ترتيب العرض ثم تاريخ الرفع، ويحذف البقية.

```sql
-- Keep the best record per (CityName, Url) when IsDeleted = 0
WITH ranked AS (
    SELECT
        ImageId,
        CityName,
        Url,
        ROW_NUMBER() OVER (
            PARTITION BY CityName, Url
            ORDER BY
                ISNULL(IsMainImage, 0) DESC,
                ISNULL(DisplayOrder, 2147483647),
                ISNULL(UploadedAt, '9999-12-31')
        ) AS rn
    FROM PropertyImages
    WHERE IsDeleted = 0 AND Url IS NOT NULL AND Url <> ''
)
DELETE FROM PropertyImages
WHERE ImageId IN (
    SELECT ImageId FROM ranked WHERE rn > 1
);
```

ملاحظات:
- يفضل توحيد الروابط قبل تشغيل التنظيف لتقليل التكرارات الشكلية (ترميز نسبة/المسارات/المسافات). التطبيق يقوم بالتوحيد، لكن يمكن إجراء توحيد يدوي إذا لزم.

## 2) قيد فريد اختياري لمنع التكرارات

يمكن إضافة فهرس فريد مفلتر لضمان فريدة `(CityName, Url)` عندما `IsDeleted = 0`. تأكد من التوافق مع إصدار SQL Server لديك.

```sql
-- Optional: filtered unique index to prevent duplicates when not deleted
CREATE UNIQUE INDEX UX_PropertyImages_City_Url_NotDeleted
ON dbo.PropertyImages (CityName, Url)
WHERE IsDeleted = 0 AND Url IS NOT NULL AND Url <> '';
```

تحذير: إذا كان لديك بيانات متضاربة حالياً، سيُفشل إنشاء الفهرس. قم بتشغيل تنظيف البيانات أولًا.

## 3) سياسة التوحيد (Normalization) في التطبيق

التطبيق يقوم بالتالي:
- توحيد روابط الصور إلى مسارات نسبية للخادم تبدأ بـ `/` (مثل: `/uploads/cities/...`).
- إزالة الفراغات الخارجية وتفكيك ترميز النسبة `Uri.UnescapeDataString`.
- إلغاء الازدواجية بشكل غير حساس لحالة الأحرف.

هذا يمنع أخطاء "An item with the same key has already been added" عند بناء القواميس.

## 4) أين تم تطبيق الإصلاحات في الكود

- backend: `YemenBooking.Infrastructure/Services/CitySettingsService.cs`
  - تم إلغاء الازدواجية وتوحيد الروابط في `GetCitiesAsync` و`SaveCitiesAsync` قبل استخدام أي قاموس.
- control_panel_app: `lib/features/admin_cities/presentation/pages/city_form_page.dart`
  - تم إلغاء الازدواجية على جانب العميل قبل إرسال البيانات، مع الحفاظ على ترتيب الصور.

## 5) اختبار بعد الإصلاح

1. جرّب تعديل مدينة (مثال: تعز) وإضافة صورة جديدة.
2. احفظ وتأكد من عدم ظهور الاستثناء السابق.
3. تحقق من أن ترتيب الصور والصورة الرئيسية صحيحان.
4. إن أردت، نفّذ SQL التنظيف مرة واحدة لضمان خلو الجدول من التكرارات التاريخية.

