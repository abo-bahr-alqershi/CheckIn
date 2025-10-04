using System;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على إحصائيات المستخدم مدى الحياة
/// Query to get user lifetime statistics
/// </summary>
public class GetUserLifetimeStatsQuery : IRequest<ResultDto<UserLifetimeStatsDto>>
{
    /// <summary>
    /// معرف المستخدم
    /// User identifier
    /// </summary>
    public Guid UserId { get; set; }

    public GetUserLifetimeStatsQuery(Guid userId)
    {
        UserId = userId;
    }
} 