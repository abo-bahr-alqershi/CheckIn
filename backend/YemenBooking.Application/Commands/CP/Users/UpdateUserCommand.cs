using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتحديث بيانات المستخدم
/// Command to update user information
/// </summary>
public class UpdateUserCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// الاسم الكامل المحدث للمستخدم
    /// Updated full name of the user
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// البريد الإلكتروني المحدث للمستخدم
    /// Updated email address of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// رقم الهاتف المحدث للمستخدم
    /// Updated phone number of the user
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// صورة الملف الشخصي المحدثة للمستخدم
    /// Updated profile image of the user
    /// </summary>
    public string? ProfileImage { get; set; }
} 