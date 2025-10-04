using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using System;

namespace YemenBooking.Application.Commands.MobileApp.Auth
{
    /// <summary>
    /// أمر تحديث رمز الوصول باستخدام رمز التحديث
    /// Command to refresh access token using refresh token
    /// </summary>
    public class RefreshTokenCommand : IRequest<ResultDto<RefreshTokenResponse>>
{
    /// <summary>
    /// رمز الوصول المنتهي الصلاحية
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
        /// <summary>
        /// رمز التحديث
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}