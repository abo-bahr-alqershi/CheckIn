using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs.Management
{
    public class ExportRequestDto
    {
        public string Type { get; set; }
        public IEnumerable<Guid>? UnitIds { get; set; }
        public DateRangeRequestDto DateRange { get; set; }
    }
} 