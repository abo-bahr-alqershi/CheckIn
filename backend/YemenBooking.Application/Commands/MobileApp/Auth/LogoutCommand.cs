using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using System;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر تسجيل خروج المستخدم
    /// Command to logout user
    /// </summary>
    public class LogoutCommand : IRequest<ResultDto<LogoutResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// رمز التحديث الحالي (لإلغائه)
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
        /// <summary>
        /// تسجيل الخروج من جميع الأجهزة
        /// </summary>
        public bool LogoutFromAllDevices { get; set; } = false;
    }
}