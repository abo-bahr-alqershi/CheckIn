using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.General;
using System;

namespace YemenBooking.Application.Queries.MobileApp.General;

/// <summary>
/// استعلام الحصول على الشروط والأحكام
/// Query to get terms and conditions
/// </summary>
public class GetTermsAndConditionsQuery : IRequest<ResultDto<LegalDocumentDto>>
{
    /// <summary>
    /// اللغة المطلوبة
    /// </summary>
    public string Language { get; set; } = "ar";
}