using System.Collections.Generic;
using YemenBooking.Application.DTOs.Management;

namespace YemenBooking.Application.DTOs.Management
{
    public class UnitManagementDataDto
    {
        public BaseUnitDto Unit { get; set; }
        public string CurrentAvailability { get; set; }
        public IEnumerable<PricingRuleDto> ActivePricingRules { get; set; }
        public IEnumerable<UpcomingBookingDto> UpcomingBookings { get; set; }
        public IEnumerable<AvailabilityCalendarDto> AvailabilityCalendar { get; set; }
    }
} 