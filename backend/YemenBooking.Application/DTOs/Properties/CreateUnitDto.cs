using System;

namespace YemenBooking.Application.DTOs.Properties
{
    /// <summary>
    /// DTO لإنشاء وحدة
    /// </summary>
    public class CreateUnitDto
    {
        /// <summary>
        /// معرف نوع الوحدة
        /// </summary>
        public Guid UnitTypeId { get; set; }

        /// <summary>
        /// رقم الوحدة
        /// </summary>
        public string UnitNumber { get; set; } = string.Empty;

        /// <summary>
        /// السعر الأساسي
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// العملة
        /// </summary>
        public string Currency { get; set; } = "YER";

        /// <summary>
        /// السعة القصوى
        /// </summary>
        public int MaxCapacity { get; set; }

        /// <summary>
        /// عدد غرف النوم
        /// </summary>
        public int? BedroomsCount { get; set; }

        /// <summary>
        /// عدد الحمامات
        /// </summary>
        public int? BathroomsCount { get; set; }

        /// <summary>
        /// المساحة بالمتر المربع
        /// </summary>
        public decimal? AreaSquareMeters { get; set; }

        /// <summary>
        /// هل متاحة
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// الحقول الديناميكية للوحدة
        /// </summary>
        public List<UnitDynamicFieldValueDto>? DynamicFields { get; set; }
    }
} 