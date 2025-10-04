using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Amenities
{
    /// <summary>
    /// أمر لتخصيص مرفق لنوع الكيان
    /// Command to assign an amenity to a property type
    /// </summary>
    public class AssignAmenityToPropertyTypeCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// معرف المرفق
        /// </summary>
        public Guid AmenityId { get; set; }

        /// <summary>
        /// هل يكون المرفق افتراضياً
        /// </summary>
        public bool IsDefault { get; set; }
    }
} 