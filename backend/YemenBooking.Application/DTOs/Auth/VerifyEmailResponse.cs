namespace YemenBooking.Application.DTOs.Auth;

/// <summary>
/// استجابة تأكيد البريد الإلكتروني
/// Email verification response
/// </summary>
public class VerifyEmailResponse
{
    /// <summary>
    /// نجاح التأكيد
    /// Verification success
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// رسالة النتيجة
    /// Result message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
