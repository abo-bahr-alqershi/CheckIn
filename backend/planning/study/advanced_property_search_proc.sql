CREATE PROCEDURE [dbo].[sp_AdvancedPropertySearch]
    @PropertyTypeId UNIQUEIDENTIFIER = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @Currency NVARCHAR(10) = 'USD',
    @PrimaryFieldFilters NVARCHAR(MAX) = NULL, -- JSON string: {"FieldId1": ["Value1", "Value2"], "FieldId2": ["Value3"]}
    @FieldFilters NVARCHAR(MAX) = NULL, -- JSON string: {"FieldId1": ["Value1"], "FieldId2": ["Value2"]}
    @UnitTypeIds NVARCHAR(MAX) = NULL, -- JSON array: ["guid1", "guid2", "guid3"]
    @AmenityIds NVARCHAR(MAX) = NULL, -- JSON array: ["guid1", "guid2", "guid3"]
    @ServiceIds NVARCHAR(MAX) = NULL, -- JSON array: ["guid1", "guid2", "guid3"]
    @SortBy NVARCHAR(50) = 'name',
    @IsAscending BIT = 0,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- للأداء العالي في الاستعلامات القراءة
    
    -- إعدادات الصفحة
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- إنشاء CTE للوحدات المتاحة في الفترة المحددة
    WITH AvailableUnits AS (
        SELECT DISTINCT 
            u.UnitId,
            u.PropertyId,
            u.UnitTypeId,
            u.BasePrice_Amount,
            u.MaxCapacity
        FROM Units u
        WHERE u.IsDeleted = 0 
            AND u.IsActive = 1
            AND u.IsAvailable = 1
            AND (@FromDate IS NULL OR @ToDate IS NULL OR NOT EXISTS (
                SELECT 1 FROM UnitAvailability ua 
                WHERE ua.UnitId = u.UnitId 
                    AND ua.IsDeleted = 0
                    AND ua.Status = 'Booked'
                    AND (
                        (@FromDate BETWEEN ua.StartDate AND ua.EndDate) OR
                        (@ToDate BETWEEN ua.StartDate AND ua.EndDate) OR
                        (ua.StartDate BETWEEN @FromDate AND @ToDate)
                    )
            ))
    ),
    -- CTE للأسعار الفعالة مع قواعد التسعير
    UnitPricing AS (
        SELECT 
            au.UnitId,
            au.PropertyId,
            au.UnitTypeId,
            au.BasePrice_Amount,
            au.MaxCapacity,
            COALESCE(
                MIN(CASE 
                    WHEN pr.IsDeleted = 0 AND pr.IsActive = 1
                        AND (@FromDate IS NULL OR pr.StartDate <= @FromDate)
                        AND (@ToDate IS NULL OR pr.EndDate >= @ToDate)
                        AND (@Currency IS NULL OR pr.Currency = @Currency)
                    THEN pr.PriceAmount 
                    ELSE NULL 
                END), 
                au.BasePrice_Amount
            ) AS EffectivePrice
        FROM AvailableUnits au
        LEFT JOIN PricingRules pr ON pr.UnitId = au.UnitId
        GROUP BY au.UnitId, au.PropertyId, au.UnitTypeId, au.BasePrice_Amount, au.MaxCapacity
    ),
    -- CTE لفلترة الوحدات حسب النوع
    FilteredByUnitType AS (
        SELECT up.*
        FROM UnitPricing up
        WHERE (@UnitTypeIds IS NULL OR up.UnitTypeId IN (
            SELECT CAST(value AS UNIQUEIDENTIFIER)
            FROM OPENJSON(@UnitTypeIds)
        ))
    ),
    -- CTE لفلترة الكيانات حسب الحقول الديناميكية الأساسية
    FilteredByPrimaryFields AS (
        SELECT DISTINCT fbt.PropertyId
        FROM FilteredByUnitType fbt
        WHERE @PrimaryFieldFilters IS NULL
        OR NOT EXISTS (
            SELECT 1 FROM OPENJSON(@PrimaryFieldFilters) AS filters
            WHERE NOT EXISTS (
                SELECT 1 
                FROM UnitFieldValues ufv
                INNER JOIN UnitTypeFields utf ON ufv.UnitTypeFieldId = utf.FieldId
                WHERE ufv.UnitId = fbt.UnitId
                    AND ufv.IsDeleted = 0
                    AND utf.IsDeleted = 0
                    AND utf.IsPrimaryFilter = 1
                    AND utf.FieldId = CAST(filters.[key] AS UNIQUEIDENTIFIER)
                    AND ufv.FieldValue IN (
                        SELECT value FROM OPENJSON(filters.value)
                    )
            )
        )
    ),
    -- CTE لفلترة الكيانات حسب الحقول الديناميكية العادية
    FilteredByFields AS (
        SELECT DISTINCT fbpf.PropertyId
        FROM FilteredByPrimaryFields fbpf
        WHERE @FieldFilters IS NULL
        OR NOT EXISTS (
            SELECT 1 FROM OPENJSON(@FieldFilters) AS filters
            WHERE NOT EXISTS (
                SELECT 1 
                FROM UnitFieldValues ufv
                INNER JOIN UnitTypeFields utf ON ufv.UnitTypeFieldId = utf.FieldId
                INNER JOIN FilteredByUnitType fbt ON ufv.UnitId = fbt.UnitId
                WHERE fbt.PropertyId = fbpf.PropertyId
                    AND ufv.IsDeleted = 0
                    AND utf.IsDeleted = 0
                    AND utf.FieldId = CAST(filters.[key] AS UNIQUEIDENTIFIER)
                    AND ufv.FieldValue IN (
                        SELECT value FROM OPENJSON(filters.value)
                    )
            )
        )
    ),
    -- CTE لفلترة الكيانات حسب المرافق
    FilteredByAmenities AS (
        SELECT DISTINCT fbf.PropertyId
        FROM FilteredByFields fbf
        WHERE @AmenityIds IS NULL
        OR NOT EXISTS (
            SELECT value FROM OPENJSON(@AmenityIds)
            EXCEPT
            SELECT pta.AmenityId
            FROM PropertyAmenities pa
            INNER JOIN PropertyTypeAmenities pta ON pa.PtaId = pta.PtaId
            WHERE pa.PropertyId = fbf.PropertyId
                AND pa.IsDeleted = 0
                AND pa.IsActive = 1
                AND pa.IsAvailable = 1
                AND pta.IsDeleted = 0
                AND pta.IsActive = 1
        )
    ),
    -- CTE لفلترة الكيانات حسب الخدمات
    FilteredByServices AS (
        SELECT DISTINCT fba.PropertyId
        FROM FilteredByAmenities fba
        WHERE @ServiceIds IS NULL
        OR NOT EXISTS (
            SELECT value FROM OPENJSON(@ServiceIds)
            EXCEPT
            SELECT ps.ServiceId
            FROM PropertyServices ps
            WHERE ps.PropertyId = fba.PropertyId
                AND ps.IsDeleted = 0
                AND ps.IsActive = 1
        )
    ),
    -- CTE للكيانات النهائية مع الحد الأدنى والأقصى للأسعار
    FinalProperties AS (
        SELECT DISTINCT 
            fbs.PropertyId,
            MIN(fbt.EffectivePrice) AS MinEffectivePrice,
            MAX(fbt.EffectivePrice) AS MaxEffectivePrice,
            MIN(fbt.BasePrice_Amount) AS MinBasePrice,
            MAX(fbt.MaxCapacity) AS MaxCapacity
        FROM FilteredByServices fbs
        INNER JOIN FilteredByUnitType fbt ON fbs.PropertyId = fbt.PropertyId
        WHERE (@MinPrice IS NULL OR fbt.EffectivePrice >= @MinPrice)
            AND (@MaxPrice IS NULL OR fbt.EffectivePrice <= @MaxPrice)
        GROUP BY fbs.PropertyId
    ),
    -- CTE لحساب التقييمات
    PropertyRatings AS (
        SELECT 
            p.PropertyId,
            AVG(CAST((ISNULL(r.Cleanliness, 0) + ISNULL(r.Service, 0) + ISNULL(r.Location, 0) + ISNULL(r.Value, 0)) AS FLOAT) / 4.0) AS AverageRating,
            COUNT(r.Id) AS ReviewsCount
        FROM FinalProperties p
        LEFT JOIN Units u ON u.PropertyId = p.PropertyId AND u.IsDeleted = 0
        LEFT JOIN Bookings b ON b.UnitId = u.UnitId AND b.IsDeleted = 0
        LEFT JOIN Reviews r ON r.BookingId = b.BookingId AND r.IsDeleted = 0 AND r.IsActive = 1
        GROUP BY p.PropertyId
    ),
    -- CTE للحقول الديناميكية المعروضة في الكروت
    PropertyCardFields AS (
        SELECT 
            fp.PropertyId,
            STRING_AGG(
                CONCAT('"', utf.DisplayName, '":"', ISNULL(ufv.FieldValue, ''), '"'), 
                ','
            ) AS CardFieldsJson
        FROM FinalProperties fp
        INNER JOIN FilteredByUnitType fbt ON fp.PropertyId = fbt.PropertyId
        INNER JOIN UnitFieldValues ufv ON ufv.UnitId = fbt.UnitId
        INNER JOIN UnitTypeFields utf ON ufv.UnitTypeFieldId = utf.FieldId
        WHERE utf.ShowInCards = 1 
            AND utf.IsDeleted = 0
            AND ufv.IsDeleted = 0
            AND ufv.FieldValue IS NOT NULL
            AND ufv.FieldValue != ''
        GROUP BY fp.PropertyId
    ),
    -- CTE للنتائج النهائية
    ResultSet AS (
        SELECT 
            p.PropertyId,
            ISNULL(pi.Url, '') AS MainImageUrl,
            p.Name,
            ISNULL(pr.AverageRating, 0) AS AverageRating,
            ISNULL(pr.ReviewsCount, 0) AS ReviewsCount,
            p.StarRating,
            fp.MinBasePrice AS BasePrice,
            fp.MinEffectivePrice AS EffectivePrice,
            ISNULL('{' + pcf.CardFieldsJson + '}', '{}') AS CardFieldValues,
            p.Latitude,
            p.Longitude,
            -- حقول للترتيب
            CASE WHEN @SortBy = 'price' THEN fp.MinEffectivePrice ELSE NULL END AS SortPrice,
            CASE WHEN @SortBy = 'rating' THEN ISNULL(pr.AverageRating, 0) ELSE NULL END AS SortRating,
            CASE WHEN @SortBy = 'name' THEN p.Name ELSE NULL END AS SortName,
            CASE WHEN @SortBy = 'reviews' THEN ISNULL(pr.ReviewsCount, 0) ELSE NULL END AS SortReviews
        FROM FinalProperties fp
        INNER JOIN Properties p ON p.PropertyId = fp.PropertyId
        LEFT JOIN PropertyImages pi ON pi.PropertyId = p.PropertyId 
            AND pi.IsMainImage = 1 
            AND pi.IsDeleted = 0 
            AND pi.IsActive = 1
        LEFT JOIN PropertyRatings pr ON pr.PropertyId = p.PropertyId
        LEFT JOIN PropertyCardFields pcf ON pcf.PropertyId = p.PropertyId
        WHERE p.IsDeleted = 0 
            AND p.IsActive = 1 
            AND p.IsApproved = 1
            AND (@PropertyTypeId IS NULL OR p.TypeId = @PropertyTypeId)
    )
    
    -- إرجاع العدد الإجمالي أولاً
    SELECT COUNT(*) AS TotalCount FROM ResultSet;
    
    -- ثم إرجاع النتائج مع الترتيب والصفحة
    SELECT 
        PropertyId,
        MainImageUrl,
        Name,
        AverageRating,
        ReviewsCount,
        StarRating,
        BasePrice,
        EffectivePrice,
        CardFieldValues,
        Latitude,
        Longitude
    FROM ResultSet
    ORDER BY 
        CASE WHEN @SortBy = 'price' AND @IsAscending = 1 THEN SortPrice END ASC,
        CASE WHEN @SortBy = 'price' AND @IsAscending = 0 THEN SortPrice END DESC,
        CASE WHEN @SortBy = 'rating' AND @IsAscending = 1 THEN SortRating END ASC,
        CASE WHEN @SortBy = 'rating' AND @IsAscending = 0 THEN SortRating END DESC,
        CASE WHEN @SortBy = 'name' AND @IsAscending = 1 THEN SortName END ASC,
        CASE WHEN @SortBy = 'name' AND @IsAscending = 0 THEN SortName END DESC,
        CASE WHEN @SortBy = 'reviews' AND @IsAscending = 1 THEN SortReviews END ASC,
        CASE WHEN @SortBy = 'reviews' AND @IsAscending = 0 THEN SortReviews END DESC,
        -- الترتيب الافتراضي
        EffectivePrice ASC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

END