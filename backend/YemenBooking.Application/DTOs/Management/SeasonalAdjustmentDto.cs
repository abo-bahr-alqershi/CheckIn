namespace YemenBooking.Application.DTOs.Management
{
    public class SeasonalAdjustmentDto
    {
        public string Period { get; set; }
        public decimal AverageAdjustment { get; set; }
        public decimal RevenueImpact { get; set; }
    }
} 