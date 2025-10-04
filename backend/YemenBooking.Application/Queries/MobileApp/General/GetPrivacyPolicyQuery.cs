using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.General;

namespace YemenBooking.Application.Queries.MobileApp.General;

/// <summary>
/// استعلام الحصول على سياسة الخصوصية
/// Query to get privacy policy
/// </summary>
public class GetPrivacyPolicyQuery : IRequest<ResultDto<LegalDocumentDto>>
{
    /// <summary>
    /// اللغة المطلوبة
    /// </summary>
    public string Language { get; set; } = "ar";
}

// LegalDocumentDto تم تعريفه مسبقًا