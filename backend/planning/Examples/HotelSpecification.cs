using System.Linq.Expressions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;

namespace YemenBooking.Core.Specifications;

/// <summary>
/// مواصفات البحث في الفنادق
/// Hotel search specifications
/// </summary>
public static class HotelSpecification
{
    /// <summary>
    /// بحث بالاسم أو الوصف
    /// Search by name or description
    /// </summary>
    public static Expression<Func<Hotel, bool>> SearchByText(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return h => true;
            
        var lowerSearchTerm = searchTerm.ToLower();
        return h => h.Name.ToLower().Contains(lowerSearchTerm) || 
                   h.Description.ToLower().Contains(lowerSearchTerm);
    }
    
    /// <summary>
    /// فلترة بالمدينة
    /// Filter by city
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByCity(string? city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return h => true;
            
        return h => h.Address != null && h.Address.City.ToLower() == city.ToLower();
    }
    
    /// <summary>
    /// فلترة بالحالة
    /// Filter by status
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByStatus(HotelStatus? status)
    {
        if (!status.HasValue)
            return h => true;
            
        return h => h.Status == status.Value;
    }
    
    /// <summary>
    /// فلترة بالنوع
    /// Filter by type
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByType(HotelType? type)
    {
        if (!type.HasValue)
            return h => true;
            
        return h => h.Type == type.Value;
    }
    
    /// <summary>
    /// فلترة بالتقييم
    /// Filter by rating
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByRating(int? minRating, int? maxRating)
    {
        return h => (!minRating.HasValue || h.StarRating >= minRating.Value) &&
                   (!maxRating.HasValue || h.StarRating <= maxRating.Value);
    }
    
    /// <summary>
    /// فلترة بنطاق الأسعار
    /// Filter by price range
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByPriceRange(decimal? minPrice, decimal? maxPrice)
    {
        return h => (!minPrice.HasValue || h.Rooms.Any(r => r.BasePrice != null && r.BasePrice.Amount >= minPrice.Value)) &&
                   (!maxPrice.HasValue || h.Rooms.Any(r => r.BasePrice != null && r.BasePrice.Amount <= maxPrice.Value));
    }
    
    /// <summary>
    /// فلترة بالمالك
    /// Filter by owner
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByOwner(Guid? ownerId)
    {
        if (!ownerId.HasValue)
            return h => true;
            
        return h => h.OwnerId == ownerId.Value;
    }
    
    /// <summary>
    /// فلترة بوجود غرف متاحة
    /// Filter by available rooms
    /// </summary>
    public static Expression<Func<Hotel, bool>> FilterByAvailableRooms(bool? hasAvailableRooms)
    {
        if (!hasAvailableRooms.HasValue)
            return h => true;
            
        if (hasAvailableRooms.Value)
            return h => h.Rooms.Any(r => r.Status == RoomStatus.AVAILABLE);
        else
            return h => !h.Rooms.Any(r => r.Status == RoomStatus.AVAILABLE);
    }
}