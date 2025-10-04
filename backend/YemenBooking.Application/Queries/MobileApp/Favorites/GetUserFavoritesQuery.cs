using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.MobileApp.Favorites;

/// <summary>
/// استعلام الحصول على قائمة المفضلات للمستخدم
/// Query to get user favorites
/// </summary>
public class GetUserFavoritesQuery : IRequest<ResultDto<UserFavoritesResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// رقم الصفحة
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// حجم الصفحة
    /// </summary>
    public int PageSize { get; set; } = 10;
}