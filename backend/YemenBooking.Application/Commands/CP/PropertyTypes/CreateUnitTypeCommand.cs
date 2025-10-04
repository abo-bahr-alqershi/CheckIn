using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyTypes
{
    /// <summary>
    /// أمر لإنشاء نوع وحدة جديد
    /// Command to create a new unit type
    /// </summary>
    public class CreateUnitTypeCommand : IRequest<ResultDto<Guid>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// اسم نوع الوحدة
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// الحد الأقصى للسعة
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
} 