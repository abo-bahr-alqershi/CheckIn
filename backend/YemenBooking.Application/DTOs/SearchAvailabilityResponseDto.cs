using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// Response DTO for availability search
    /// </summary>
    public class SearchAvailabilityResponseDto
    {
        public IEnumerable<UnitAvailabilityDetailDto> Availabilities { get; set; } = new List<UnitAvailabilityDetailDto>();
        public IEnumerable<object> Conflicts { get; set; } = new List<object>();
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }
}