using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Reports;

namespace YemenBooking.Application.Commands.MobileApp.Reports;

/// <summary>
/// أمر الإبلاغ عن كيان
/// Command to report property
/// </summary>
public class ReportPropertyCommand : IRequest<ResultDto<ReportPropertyResponse>>
{
    /// <summary>
    /// معرف المستخدم المبلغ
    /// </summary>
    public Guid ReporterUserId { get; set; }
    
    /// <summary>
    /// معرف الكيان المبلغ عنه
    /// </summary>
    public Guid ReportedPropertyId { get; set; }
    
    /// <summary>
    /// سبب البلاغ
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// الوصف التفصيلي للمشكلة
    /// </summary>
    public string Description { get; set; } = string.Empty;
}