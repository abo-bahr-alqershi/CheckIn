using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.PropertyTypes
{
    /// <summary>
    /// استعلام للحصول على جميع أنواع الوحدات
    /// Query to get all unit types
    /// </summary>
    public class GetAllUnitTypesQuery : IRequest<PaginatedResult<UnitTypeDto>>
    {
        /// <summary>
        /// رقم الصفحة
        /// Page number
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// مصطلح البحث (اختياري)
        /// Search term (optional)
        /// </summary>
        public string? SearchTerm { get; set; }
    }
} 