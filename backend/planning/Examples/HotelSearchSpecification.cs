using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using System.Linq.Expressions;

namespace YemenBooking.Core.Specifications;

/// <summary>
/// مواصفات البحث في الفنادق
/// Hotel search specifications
/// </summary>
public class HotelSearchSpecification : BaseSpecification<Hotel>
{
    public HotelSearchSpecification(HotelSearchParameters parameters) : base()
    {
        // المعايير الأساسية: الفنادق النشطة غير المحذوفة
        // Base criteria: active, non-deleted hotels
        AddCriteria(h => h.Status == HotelStatus.ACTIVE && !h.IsDeleted);

        // تطبيق فلاتر البحث
        // Apply search filters
        ApplyFilters(parameters);

        // إضافة التضمينات
        // Add includes
        AddIncludes();

        // تطبيق الترتيب
        // Apply ordering
        ApplyOrdering(parameters);

        // تطبيق التصفح
        // Apply paging
        if (parameters.PageNumber > 0 && parameters.PageSize > 0)
        {
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
        }

        // تحسين الأداء
        // Performance optimizations
        ApplyNoTracking();
        ApplySplitQuery();
    }

    private void ApplyFilters(HotelSearchParameters parameters)
    {
        // البحث النصي
        // Text search
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower().Trim();
            AddCriteria(h => 
                h.Name.ToLower().Contains(searchTerm) ||
                h.Description.ToLower().Contains(searchTerm) ||
                h.Address.City.ToLower().Contains(searchTerm) ||
                h.Address.Country.ToLower().Contains(searchTerm) ||
                h.Address.Street.ToLower().Contains(searchTerm));
        }

        // فلترة بالمدينة
        // Filter by city
        if (!string.IsNullOrWhiteSpace(parameters.City))
        {
            AddCriteria(h => h.Address.City.ToLower() == parameters.City.ToLower());
        }

        // فلترة بالدولة
        // Filter by country
        if (!string.IsNullOrWhiteSpace(parameters.Country))
        {
            AddCriteria(h => h.Address.Country.ToLower() == parameters.Country.ToLower());
        }

        // فلترة بنوع الفندق
        // Filter by hotel type
        if (parameters.HotelType.HasValue)
        {
            AddCriteria(h => h.Type == parameters.HotelType.Value);
        }

        // فلترة بتصنيف النجوم
        // Filter by star rating
        if (parameters.MinStarRating.HasValue)
        {
            AddCriteria(h => h.StarRating >= parameters.MinStarRating.Value);
        }

        if (parameters.MaxStarRating.HasValue)
        {
            AddCriteria(h => h.StarRating <= parameters.MaxStarRating.Value);
        }

        // فلترة بتقييم الضيوف
        // Filter by guest rating
        if (parameters.MinGuestRating.HasValue)
        {
            AddCriteria(h => h.AverageRating >= parameters.MinGuestRating.Value);
        }

        if (parameters.MaxGuestRating.HasValue)
        {
            AddCriteria(h => h.AverageRating <= parameters.MaxGuestRating.Value);
        }

        // فلترة بنطاق الأسعار
        // Filter by price range
        if (parameters.MinPrice.HasValue)
        {
            AddCriteria(h => h.Rooms.Any(r => r.BasePrice != null && r.BasePrice.Amount >= parameters.MinPrice.Value));
        }

        if (parameters.MaxPrice.HasValue)
        {
            AddCriteria(h => h.Rooms.Any(r => r.BasePrice != null && r.BasePrice.Amount <= parameters.MaxPrice.Value));
        }

        // فلترة بالمرافق المطلوبة
        // Filter by required amenities
        if (parameters.RequiredAmenities?.Any() == true)
        {
            foreach (var amenity in parameters.RequiredAmenities)
            {
                var amenityName = amenity.ToLower();
                AddCriteria(h => h.Amenities.Any(a => 
                    a.IsActive && 
                    a.Name.ToLower().Contains(amenityName)));
            }
        }

        // فلترة بوجود غرف متاحة
        // Filter by available rooms
        if (parameters.RequireAvailableRooms)
        {
            AddCriteria(h => h.Rooms.Any(r => r.Status == RoomStatus.AVAILABLE && r.IsActive));
        }

        // فلترة بعدد الغرف المطلوبة
        // Filter by required room count
        if (parameters.MinRoomCount.HasValue)
        {
            AddCriteria(h => h.Rooms.Count(r => r.Status == RoomStatus.AVAILABLE && r.IsActive) >= parameters.MinRoomCount.Value);
        }

        // فلترة بالسعة المطلوبة
        // Filter by required capacity
        if (parameters.RequiredCapacity.HasValue)
        {
            AddCriteria(h => h.Rooms.Any(r => 
                r.Status == RoomStatus.AVAILABLE && 
                r.IsActive && 
                (r.MaxAdults + r.MaxChildren) >= parameters.RequiredCapacity.Value));
        }

        // فلترة بالموقع (المسافة)
        // Filter by location (distance)
        if (parameters.Latitude.HasValue && parameters.Longitude.HasValue && parameters.MaxDistanceKm.HasValue)
        {
            // ملاحظة: هذا الفلتر يحتاج تحسين في قاعدة البيانات لحساب المسافة
            // Note: This filter needs database optimization for distance calculation
            AddCriteria(h => h.Address.Latitude.HasValue && h.Address.Longitude.HasValue);
        }

        // فلترة بالمالك
        // Filter by owner
        if (parameters.OwnerId.HasValue)
        {
            AddCriteria(h => h.OwnerId == parameters.OwnerId.Value);
        }

        // فلترة بالتواريخ المتاحة
        // Filter by available dates
        if (parameters.CheckInDate.HasValue && parameters.CheckOutDate.HasValue)
        {
            // هذا يحتاج تحقق أكثر تعقيداً من الحجوزات
            // This needs more complex booking verification
            AddCriteria(h => h.Rooms.Any(r => 
                r.Status == RoomStatus.AVAILABLE && 
                r.IsActive &&
                !r.BookedRooms.Any(br => 
                    br.Booking.Status != BookingStatus.CANCELLED &&
                    br.Booking.CheckInDate < parameters.CheckOutDate.Value &&
                    br.Booking.CheckOutDate > parameters.CheckInDate.Value)));
        }
    }

    private void AddIncludes()
    {
        // تضمين العلاقات المهمة
        // Include important relationships
        AddInclude(h => h.Address);
        AddInclude(h => h.Owner);
        AddInclude(h => h.Amenities);
        AddInclude(h => h.Images);
        AddInclude(h => h.Rooms);
        
        // تضمينات متقدمة للمرافق والصور
        // Advanced includes for amenities and images
        AddInclude("Images");
        AddInclude("Rooms.Amenities");
        AddInclude("Rooms.Images");
        AddInclude("Rooms.BookedRooms");
    }

    private void ApplyOrdering(HotelSearchParameters parameters)
    {
        // الترتيب حسب المعايير المحددة
        // Order by specified criteria
        switch (parameters.SortBy?.ToLower())
        {
            case "name":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.Name);
                else
                    ApplyOrderBy(h => h.Name);
                break;

            case "rating":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.AverageRating);
                else
                    ApplyOrderBy(h => h.AverageRating);
                break;

            case "stars":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.StarRating);
                else
                    ApplyOrderBy(h => h.StarRating);
                break;

            case "price":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.MinPrice != null ? h.MinPrice.Amount : 0);
                else
                    ApplyOrderBy(h => h.MinPrice != null ? h.MinPrice.Amount : 0);
                break;

            case "reviews":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.TotalReviews);
                else
                    ApplyOrderBy(h => h.TotalReviews);
                break;

            case "created":
            case "date":
                if (parameters.SortDescending)
                    ApplyOrderByDescending(h => h.CreatedAt);
                else
                    ApplyOrderBy(h => h.CreatedAt);
                break;

            case "popularity":
                // ترتيب بناءً على الشعبية (تقييم + عدد المراجعات)
                // Order by popularity (rating + review count)
                ApplyOrderByDescending(h => h.AverageRating * (decimal)Math.Log10(Math.Max(h.TotalReviews, 1)));
                break;

            case "relevance":
            default:
                // الترتيب الافتراضي: حسب الصلة والجودة
                // Default order: by relevance and quality
                ApplyOrderByDescending(h => h.AverageRating);
                break;
        }
    }
}

/// <summary>
/// معاملات البحث في الفنادق
/// Hotel search parameters
/// </summary>
public class HotelSearchParameters
{
    /// <summary>
    /// مصطلح البحث
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// الدولة
    /// Country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// نوع الفندق
    /// Hotel type
    /// </summary>
    public HotelType? HotelType { get; set; }

    /// <summary>
    /// الحد الأدنى لتصنيف النجوم
    /// Minimum star rating
    /// </summary>
    public int? MinStarRating { get; set; }

    /// <summary>
    /// الحد الأقصى لتصنيف النجوم
    /// Maximum star rating
    /// </summary>
    public int? MaxStarRating { get; set; }

    /// <summary>
    /// الحد الأدنى لتقييم الضيوف
    /// Minimum guest rating
    /// </summary>
    public decimal? MinGuestRating { get; set; }

    /// <summary>
    /// الحد الأقصى لتقييم الضيوف
    /// Maximum guest rating
    /// </summary>
    public decimal? MaxGuestRating { get; set; }

    /// <summary>
    /// الحد الأدنى للسعر
    /// Minimum price
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// الحد الأقصى للسعر
    /// Maximum price
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// المرافق المطلوبة
    /// Required amenities
    /// </summary>
    public List<string>? RequiredAmenities { get; set; }

    /// <summary>
    /// يتطلب غرف متاحة
    /// Require available rooms
    /// </summary>
    public bool RequireAvailableRooms { get; set; } = true;

    /// <summary>
    /// الحد الأدنى لعدد الغرف
    /// Minimum room count
    /// </summary>
    public int? MinRoomCount { get; set; }

    /// <summary>
    /// السعة المطلوبة
    /// Required capacity
    /// </summary>
    public int? RequiredCapacity { get; set; }

    /// <summary>
    /// خط العرض
    /// Latitude
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// خط الطول
    /// Longitude
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// أقصى مسافة بالكيلومتر
    /// Maximum distance in kilometers
    /// </summary>
    public double? MaxDistanceKm { get; set; }

    /// <summary>
    /// تاريخ الوصول
    /// Check-in date
    /// </summary>
    public DateTime? CheckInDate { get; set; }

    /// <summary>
    /// تاريخ المغادرة
    /// Check-out date
    /// </summary>
    public DateTime? CheckOutDate { get; set; }

    /// <summary>
    /// معرف المالك
    /// Owner ID
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// الترتيب حسب
    /// Sort by
    /// </summary>
    public string? SortBy { get; set; } = "relevance";

    /// <summary>
    /// ترتيب تنازلي
    /// Sort descending
    /// </summary>
    public bool SortDescending { get; set; } = false;
}
