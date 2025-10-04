using System;
using MediatR;
using YemenBooking.Application.DTOs.Dashboard;

namespace YemenBooking.Application.Queries.Dashboard
{
    /// <summary>
    /// استعلام للحصول على بيانات لوحة تحكم المسؤول ضمن نطاق زمني
    /// Query to retrieve admin dashboard data within a date range
    /// </summary>
    public class GetAdminDashboardQuery : IRequest<AdminDashboardDto>
    {
        /// <summary>
        /// النطاق الزمني
        /// Date range for the dashboard data
        /// </summary>
        public DateRangeDto Range { get; set; }

        public GetAdminDashboardQuery(DateRangeDto range)
        {
            Range = range;
        }
    }
} 