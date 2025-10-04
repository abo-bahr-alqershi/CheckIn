using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Users;

/// <summary>
/// استعلام للحصول على بيانات المستخدم الحالي
/// Query to get current logged-in user data
/// </summary>
public class GetCurrentUserQuery : IRequest<ResultDto<UserDto>>
{
} 