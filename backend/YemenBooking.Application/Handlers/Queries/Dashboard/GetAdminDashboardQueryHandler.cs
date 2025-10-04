using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام بيانات لوحة تحكم المسؤول
    /// Handler for GetAdminDashboardQuery
    /// </summary>
    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReportingService _reportingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAdminDashboardQueryHandler> _logger;

        public GetAdminDashboardQueryHandler(
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            IReportingService reportingService,
            ICurrentUserService currentUserService,
            ILogger<GetAdminDashboardQueryHandler> logger)
        {
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _reportingService = reportingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Admin Dashboard Query for range {Start} - {End}", request.Range.StartDate, request.Range.EndDate);

            // Authorization: Admin only
            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null || !_currentUserService.UserRoles.Contains("Admin"))
            {
                _logger.LogWarning("Unauthorized access to admin dashboard");
                throw new UnauthorizedAccessException("يجب أن تكون مسؤولًا للوصول إلى لوحة التحكم");
            }

            // Fetch data
            var totalUsers = await _userRepository.CountAsync(cancellationToken);
            var totalProperties = await _propertyRepository.CountAsync(cancellationToken);
            var totalBookings = await _bookingRepository.GetTotalBookingsCountAsync(null, request.Range.StartDate, request.Range.EndDate);
            var totalRevenue = await _reportingService.CalculateRevenueAsync(request.Range.StartDate, request.Range.EndDate, null, cancellationToken);

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalProperties = totalProperties,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue
            };
        }
    }
} 