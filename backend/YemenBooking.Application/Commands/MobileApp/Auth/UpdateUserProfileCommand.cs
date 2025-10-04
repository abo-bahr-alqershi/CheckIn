using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using System;
using System.ComponentModel.DataAnnotations;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر تحديث ملف المستخدم الشخصي
    /// Command to update user profile
    /// </summary>
    public class UpdateUserProfileCommand : IRequest<ResultDto<UpdateUserProfileResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// الاسم الجديد (اختياري)
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// رقم الهاتف الجديد (اختياري)
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// صورة الملف الشخصي المحدثة (Base64)
    /// </summary>
    public string? ProfileImageBase64 { get; set; }
    }
}