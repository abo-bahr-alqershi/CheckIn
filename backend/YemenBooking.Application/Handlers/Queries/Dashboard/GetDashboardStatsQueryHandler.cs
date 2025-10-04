using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;
using YemenBooking.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام إحصائيات لوحة التحكم
    /// Handler for dashboard statistics query
    /// </summary>
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardStatsQueryHandler(
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // عدد المستخدمين غير المؤكدين
            var users = await _userRepository.GetAllUsersAsync(cancellationToken);
            var unverifiedCount = users.Count(u => !u.EmailConfirmed);

            // عدد الكيانات غير المعتمدة
            var properties = await _propertyRepository.GetPendingPropertiesAsync(cancellationToken);
            var unapprovedCount = properties.Count();

            // عدد الحجوزات غير المؤكدة (Pending)
            var pendingBookings = await _bookingRepository.GetBookingsByStatusAsync(BookingStatus.Pending, cancellationToken);
            var unconfirmedBookingsCount = pendingBookings.Count();

            // عدد الاشعارات غير المقروءة للمستخدم الحالي
            var userId = _currentUserService.UserId;
            var unreadNotifications = await _notificationRepository.GetUnreadNotificationsCountAsync(userId, cancellationToken);

            return new DashboardStatsDto
            {
                UnverifiedUsers = unverifiedCount,
                UnapprovedProperties = unapprovedCount,
                UnconfirmedBookings = unconfirmedBookingsCount,
                UnreadNotifications = unreadNotifications
            };
        }
    }
} 