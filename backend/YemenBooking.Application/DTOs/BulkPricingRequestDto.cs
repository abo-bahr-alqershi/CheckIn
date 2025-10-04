using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO لإنشاء قواعد تسعير مجمعة
    /// Bulk create pricing rules request DTO
    /// </summary>
    public class BulkPricingRequestDto
    {
        public IEnumerable<CreatePricingRequestDto> Requests { get; set; }
    }
} 