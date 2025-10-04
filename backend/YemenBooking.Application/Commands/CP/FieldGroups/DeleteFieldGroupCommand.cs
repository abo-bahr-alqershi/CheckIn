namespace YemenBooking.Application.Commands.FieldGroups;

using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// حذف مجموعة حقول
/// Delete field group
/// </summary>
public class DeleteFieldGroupCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف المجموعة
    /// GroupId
    /// </summary>
    public string GroupId { get; set; }
} 