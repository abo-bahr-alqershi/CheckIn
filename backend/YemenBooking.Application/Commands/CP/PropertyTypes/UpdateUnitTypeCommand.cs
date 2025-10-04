using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyTypes;

/// <summary>
/// أمر لتحديث نوع الوحدة
/// Command to update a unit type
/// </summary>
public class UpdateUnitTypeCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type identifier
    /// </summary>
    public Guid UnitTypeId { get; set; }

    /// <summary>
    /// الاسم الجديد لنوع الوحدة
    /// New unit type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// الحد الأقصى للسعة الجديدة
    /// New maximum capacity
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// ايقونة لنوع الوحدة
    /// Icon for the unit type
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// هذا النوع يحتوي على بالغين
    /// This type has adults
    /// </summary>
    public bool IsHasAdults { get; set; } = false;

    /// <summary>
    /// هذا النوع يحتوي على أطفال
    /// This type has children
    /// </summary>
    public bool IsHasChildren { get; set; } = false;

    /// <summary>
    /// هذا النوع يحتوي على أيام متعددة
    /// This type has multiple days
    /// </summary>
    public bool IsMultiDays { get; set; } = false;

    /// <summary>
    /// هذا النوع يحتاج لتحديد الساعة
    /// This type requires determining the hour
    /// </summary>
    public bool IsRequiredToDetermineTheHour { get; set; } = false;

} 