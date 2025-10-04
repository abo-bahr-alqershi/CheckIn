using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Dashboard
{
    /// <summary>
    /// استعلام للحصول على أفضل الكيانات أداءً بناءً على عدد الحجوزات
    /// Query to retrieve top performing properties based on booking count
    /// </summary>
    public class GetTopPerformingPropertiesQuery : IRequest<IEnumerable<PropertyDto>>
    {
        /// <summary>
        /// عدد الكيانات المطلوب جلبها
        /// Number of top properties to retrieve
        /// </summary>
        public int Count { get; set; }

        public GetTopPerformingPropertiesQuery(int count)
        {
            Count = count;
        }
    }
} 