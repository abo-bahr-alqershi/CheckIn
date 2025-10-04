using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة الإشعارات
    /// Notification service implementation
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IFirebaseService _firebaseService;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEmailService emailService,
            ISmsService smsService,
            IFirebaseService firebaseService,
            IBookingRepository bookingRepository,
            IPropertyRepository propertyRepository,
            IUserRepository userRepository,
            ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _firebaseService = firebaseService;
            _bookingRepository = bookingRepository;
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendGuestNoteNotificationAsync(GuestNoteNotification notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار ملاحظة الضيف: {@Notification}", notification);
            var booking = await _bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
            var userId = booking?.UserId ?? Guid.Empty;
            var title = notification.NoteTitle ?? "ملاحظة جديدة";
            var content = notification.NoteContent;
            if (!string.IsNullOrEmpty(notification.GuestEmail))
                await _emailService.SendEmailAsync(notification.GuestEmail, title, content, false, cancellationToken);
            if (booking?.User?.Phone is string phone && !string.IsNullOrEmpty(phone))
                await _smsService.SendSmsAsync(phone, $"{title}: {content}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{userId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendHotelNoteNotificationAsync(HotelNoteNotification notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار ملاحظة الفندق: {@Notification}", notification);
            var prop = await _propertyRepository.GetPropertyByIdAsync(notification.HotelId, cancellationToken);
            var owner = prop != null ? await _userRepository.GetUserByIdAsync(prop.OwnerId, cancellationToken) : null;
            var title = notification.NoteTitle ?? "ملاحظة من الفندق";
            var content = notification.NoteContent;
            if (owner != null)
            {
                await _emailService.SendEmailAsync(owner.Email, title, content, false, cancellationToken);
                if (!string.IsNullOrEmpty(owner.Phone))
                    await _smsService.SendSmsAsync(owner.Phone, $"{title}: {content}", cancellationToken);
                await _firebaseService.SendNotificationAsync($"user_{owner.Id}", title, content, null, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task SendHighPriorityNoteNotificationAsync(HighPriorityNoteNotification notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار عالية الأولوية: {@Notification}", notification);
            var title = notification.NoteTitle ?? "ملاحظة هامة";
            var content = notification.NoteContent;
            // إرسال إلى جميع المشرفين (أو مجموعة معينة)
            await _emailService.SendEmailAsync("admin@yemenbooking.com", title, content, false, cancellationToken);
            await _smsService.SendSmsAsync("+0000000000", $"{title}: {content}", cancellationToken);
            await _firebaseService.SendNotificationAsync("admins", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendBookingConfirmationAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال تأكيد الحجز: {BookingId}", booking.Id);
            var title = "تم تأكيد حجزك";
            var content = $"مرحباً {booking.User.Name}, تم تأكيد حجزك رقم {booking.Id}.\nالتحقق في: {booking.CheckIn:yyyy-MM-dd}, الخروج: {booking.CheckOut:yyyy-MM-dd}";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, $"{title}: رقم الحجز {booking.Id}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendCheckInNotificationAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار تسجيل الوصول: {BookingId}", booking.Id);
            var title = "تذكير تسجيل الوصول";
            var content = $"مرحباً {booking.User.Name}, موعد تسجيل الوصول لحجزك رقم {booking.Id} هو {booking.CheckIn:yyyy-MM-dd}.";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, $"{title}: {booking.CheckIn:yyyy-MM-dd}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendCheckOutNotificationAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار تسجيل المغادرة: {BookingId}", booking.Id);
            var title = "تذكير تسجيل المغادرة";
            var content = $"مرحباً {booking.User.Name}, موعد تسجيل المغادرة لحجزك رقم {booking.Id} هو {booking.CheckOut:yyyy-MM-dd}.";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, $"{title}: {booking.CheckOut:yyyy-MM-dd}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendBookingCancellationAsync(Booking booking, string reason, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار إلغاء الحجز: {BookingId} بسبب {Reason}", booking.Id, reason);
            var title = "تم إلغاء حجزك";
            var content = $"مرحباً {booking.User.Name}, تم إلغاء حجزك رقم {booking.Id}. السبب: {reason}.";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, $"{title}: {reason}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendBookingReminderAsync(Booking booking, int daysBefore, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال تذكير بالحجز للمستخدم بعد {DaysBefore} يومًا: {BookingId}", daysBefore, booking.Id);
            var title = $"تذكير بحجزك بعد {daysBefore} يومًا";
            var content = $"مرحباً {booking.User.Name}, هذا تذكير بأن موعد حجزك رقم {booking.Id} بعد {daysBefore} يومًا.";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, title, cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendPaymentNotificationAsync(Booking booking, decimal amount, PaymentStatus status, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار دفع: {BookingId}, المبلغ: {Amount}, الحالة: {Status}", booking.Id, amount, status);
            var title = "تأكيد الدفع";
            var content = $"مرحباً {booking.User.Name}, تم استلام دفعة بمبلغ {amount:C} لحجزك رقم {booking.Id}. الحالة: {status}.";
            await _emailService.SendEmailAsync(booking.User.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(booking.User.Phone))
                await _smsService.SendSmsAsync(booking.User.Phone, $"{title}: {amount:C}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{booking.UserId}", title, content, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendRoomAvailabilityChangedNotificationAsync(RoomAvailabilityChangedNotification notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار تغيير توفر الغرفة: {@Notification}", notification);
            var title = "تغير توفر الغرفة";
            var content = $"الغرفة {notification.RoomNumber} متاحة من {notification.FromDate:yyyy-MM-dd} إلى {notification.ToDate:yyyy-MM-dd}.";
            await NotifyByIdsAsync(notification.UpdatedByUserId.GetValueOrDefault(), title, content, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendRoomStatusChangedNotificationAsync(RoomStatusChangedNotification notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار تغيير حالة الغرفة: {@Notification}", notification);
            var title = "تغير حالة الغرفة";
            var content = $"الغرفة {notification.RoomNumber} تم تغيير حالتها من {notification.PreviousStatus} إلى {notification.NewStatus}. السبب: {notification.Reason}.";
            await NotifyByIdsAsync(notification.ChangedByUserId, title, content, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendBookingAffectedByRoomStatusChangeNotificationAsync(Guid bookingId, Guid roomId, string newRoomStatus, string reason, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إشعار تأثر الحجز {BookingId} بسبب تغيير حالة الغرفة {RoomId} إلى {Status}، السبب: {Reason}", bookingId, roomId, newRoomStatus, reason);
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId, cancellationToken);
            var userId = booking?.UserId ?? Guid.Empty;
            var title = "تأثر حجزك بتغير حالة الغرفة";
            var content = $"تم تغيير حالة الغرفة {roomId} إلى {newRoomStatus} مما أثر على حجزك رقم {bookingId}. السبب: {reason}.";
            await NotifyByIdsAsync(userId, title, content, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendCheckOutConfirmationAsync(Guid bookingId, Guid customerId)
        {
            _logger.LogInformation("إرسال تأكيد تسجيل المغادرة للحجز {BookingId} للمستخدم {CustomerId}", bookingId, customerId);
            var title = "تأكيد تسجيل المغادرة";
            var content = $"تم تأكيد تسجيل مغادرة حجزك رقم {bookingId}. شكراً لاستخدامك YemenBooking.";
            await NotifyByIdsAsync(customerId, title, content, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SendHotelCheckOutNotificationAsync(Guid bookingId, Guid hotelId)
        {
            _logger.LogInformation("إرسال إشعار تسجيل المغادرة للفندق للحجز {BookingId} للفندق {HotelId}", bookingId, hotelId);
            var prop = await _propertyRepository.GetPropertyByIdAsync(hotelId, CancellationToken.None);
            var ownerId = prop?.OwnerId ?? Guid.Empty;
            var title = "تأكيد تسجيل مغادرة الضيف";
            var content = $"الضيف أنهى تسجيل مغادرة الحجز رقم {bookingId}.";
            await NotifyByIdsAsync(ownerId, title, content, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SendCheckInConfirmationAsync(Guid bookingId, Guid customerId, string wifiPassword)
        {
            _logger.LogInformation("إرسال تأكيد تسجيل الوصول للحجز {BookingId} للمستخدم {CustomerId} مع كلمة واي فاي {WifiPassword}", bookingId, customerId, wifiPassword);
            var title = "تأكيد تسجيل الوصول";
            var content = $"تم تأكيد تسجيل وصولك الحجز رقم {bookingId}. كلمة الواي فاي: {wifiPassword}.";
            await NotifyByIdsAsync(customerId, title, content, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SendHotelCheckInNotificationAsync(Guid bookingId, Guid hotelId)
        {
            _logger.LogInformation("إرسال إشعار تسجيل الوصول للفندق للحجز {BookingId} للفندق {HotelId}", bookingId, hotelId);
            var prop = await _propertyRepository.GetPropertyByIdAsync(hotelId, CancellationToken.None);
            var ownerId = prop?.OwnerId ?? Guid.Empty;
            var title = "تأكيد تسجيل وصول الضيف";
            var content = $"الضيف قام بتسجيل الوصول للحجز رقم {bookingId}.";
            await NotifyByIdsAsync(ownerId, title, content, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SendAsync(YemenBooking.Core.Notifications.NotificationRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال إشعار عام: {@Request}", request);
            // Generic notification: send based on user ID and message
            await NotifyByIdsAsync(request.UserId, request.Title, request.Message, cancellationToken);
        }

        private async Task NotifyByIdsAsync(Guid userId, string title, string content, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) return;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null) return;
            await _emailService.SendEmailAsync(user.Email, title, content, false, cancellationToken);
            if (!string.IsNullOrEmpty(user.Phone))
                await _smsService.SendSmsAsync(user.Phone, $"{title}: {content}", cancellationToken);
            await _firebaseService.SendNotificationAsync($"user_{userId}", title, content, null, cancellationToken);
        }
    }
} 