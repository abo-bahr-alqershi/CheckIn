using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO لإنشاء إتاحات مجمعة
    /// Bulk create unit availabilities request DTO
    /// </summary>
    public class BulkAvailabilityRequestDto
    {
        public IEnumerable<CreateAvailabilityRequestDto> Requests { get; set; }
    }
} 