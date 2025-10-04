using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.MobileApp.Statistics;

/// <summary>
/// استعلام الحصول على إحصائيات المستخدم
/// Query to get user statistics
/// </summary>
public class GetUserStatisticsQuery : IRequest<ResultDto<UserStatisticsDto>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
}