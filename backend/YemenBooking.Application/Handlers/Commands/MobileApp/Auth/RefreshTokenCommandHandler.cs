using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth;

/// <summary>
/// معالج أمر تحديث رمز الوصول
/// Handler for refresh token command
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultDto<RefreshTokenResponse>>
{
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    /// <summary>
    /// منشئ معالج أمر تحديث رمز الوصول
    /// Constructor for refresh token command handler
    /// </summary>
    /// <param name="authService">خدمة المصادقة</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public RefreshTokenCommandHandler(
        IAuthenticationService authService,
        IUserRepository userRepository,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر تحديث رمز الوصول
    /// Handle refresh token command
    /// </summary>
    /// <param name="request">طلب تحديث رمز الوصول</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء عملية تحديث رمز الوصول");

            // التحقق من صحة البيانات المدخلة
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogWarning("محاولة تحديث رمز الوصول برمز تحديث فارغ");
                return ResultDto<RefreshTokenResponse>.Failed("رمز التحديث مطلوب", "REFRESH_TOKEN_REQUIRED");
            }

            if (string.IsNullOrWhiteSpace(request.AccessToken))
            {
                _logger.LogWarning("محاولة تحديث رمز الوصول برمز وصول فارغ");
                return ResultDto<RefreshTokenResponse>.Failed("رمز الوصول مطلوب", "ACCESS_TOKEN_REQUIRED");
            }

            _logger.LogInformation("بدء عملية تحديث رمز الوصول");
            
            try
            {
                var newTokens = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

                var response = new RefreshTokenResponse
                {
                    NewAccessToken = newTokens.AccessToken,
                    NewRefreshToken = newTokens.RefreshToken,
                    AccessTokenExpiry = newTokens.ExpiresAt,
                    RefreshTokenExpiry = newTokens.ExpiresAt.AddDays(0) // لا نملك قيمة منفصلة حالياً
                };

                _logger.LogInformation("تم تحديث رمز الوصول بنجاح للمستخدم: {UserId}", newTokens.UserId);
                return ResultDto<RefreshTokenResponse>.Ok(response, "تم تحديث رمز الوصول بنجاح");
            }
            catch (Exception refreshEx)
            {
                _logger.LogWarning(refreshEx, "فشل في تحديث رمز الوصول");
                return ResultDto<RefreshTokenResponse>.Failed("فشل تحديث رمز الوصول", "REFRESH_TOKEN_ERROR");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تحديث رمز الوصول");
            return ResultDto<RefreshTokenResponse>.Failed($"حدث خطأ أثناء تحديث رمز الوصول: {ex.Message}", "REFRESH_TOKEN_ERROR");
        }
    }
}
