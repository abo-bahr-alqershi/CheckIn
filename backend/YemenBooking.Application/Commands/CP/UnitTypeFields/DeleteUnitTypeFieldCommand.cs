namespace YemenBooking.Application.Commands.UnitTypeFields;

using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// حذف حقل ديناميكي من نوع الكيان
/// Delete dynamic field from property type
/// </summary>
public class DeleteUnitTypeFieldCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحقل
    /// FieldId
    /// </summary>
    public string FieldId { get; set; }
} 