using System.Collections.Generic;

namespace YemenBooking.Application.DTOs.Management
{
    public class ManagementPageResponseDto
    {
        public IEnumerable<UnitManagementDataDto> Units { get; set; }
        public ManagementSummaryDto Summary { get; set; }
    }
} 