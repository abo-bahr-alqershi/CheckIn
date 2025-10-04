using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Users;

/// <summary>
/// استعلام للحصول على بيانات مستخدم باستخدام المعرف
/// Query to get user data by ID
/// </summary>
public class GetUserByIdQuery : IRequest<ResultDto<object>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }
} 