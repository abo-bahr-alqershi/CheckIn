import 'dart:convert';
import 'dart:io';
import 'package:bookn_cp_app/features/chat/domain/entities/message.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:flutter/foundation.dart';
import 'package:timezone/timezone.dart' as tz;
import 'package:permission_handler/permission_handler.dart';
import '../core/network/api_client.dart';
import 'local_storage_service.dart';
import '../features/auth/data/datasources/auth_local_datasource.dart';
import 'package:bookn_cp_app/injection_container.dart' as di;
import '../services/websocket_service.dart';
import 'package:bookn_cp_app/injection_container.dart' as di;
import 'package:bookn_cp_app/features/chat/presentation/bloc/chat_bloc.dart';

class NotificationService {
  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotifications =
      FlutterLocalNotificationsPlugin();

  final ApiClient? _apiClient;
  final LocalStorageService? _localStorage;
  final AuthLocalDataSource? _authLocalDataSource;

  // Optional sink to dispatch chat events directly without WebSocket
  void Function(WebSocketMessageReceivedEvent event)? _chatEventSink;

  void bindChatEventSink(
      void Function(WebSocketMessageReceivedEvent event) sink) {
    _chatEventSink = sink;
  }

  void unbindChatEventSink() {
    _chatEventSink = null;
  }

  NotificationService({
    ApiClient? apiClient,
    LocalStorageService? localStorage,
    AuthLocalDataSource? authLocalDataSource,
  })  : _apiClient = apiClient,
        _localStorage = localStorage,
        _authLocalDataSource = authLocalDataSource;

  /// Re-register FCM token and subscribe to user/role topics for the current user
  Future<void> refreshUserSubscriptions() async {
    await _registerFcmToken();
  }

  // Initialize notification service
  Future<void> initialize() async {
    // Request permission
    await _requestPermission();

    // Initialize local notifications
    await _initializeLocalNotifications();

    // Configure Firebase messaging
    await _configureFirebaseMessaging();

    // Get and register FCM token
    await _registerFcmToken();

    // Handle token refresh
    _firebaseMessaging.onTokenRefresh.listen(_onTokenRefresh);
  }

  // Request notification permission
  Future<void> _requestPermission() async {
    // Android 13+ requires runtime POST_NOTIFICATIONS permission
    if (Platform.isAndroid) {
      final current = await Permission.notification.status;
      if (!current.isGranted) {
        final result = await Permission.notification.request();
        debugPrint('Android notification permission result: $result');
      }
    }

    final settings = await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
      provisional: false,
      announcement: false,
      carPlay: false,
      criticalAlert: false,
    );

    debugPrint(
        'Notification permission status: ${settings.authorizationStatus}');
  }

  // Initialize local notifications
  Future<void> _initializeLocalNotifications() async {
    // Ensure the Android channel exists so push notifications render reliably
    const androidSettings =
        AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosSettings = DarwinInitializationSettings(
      requestAlertPermission: true,
      requestBadgePermission: true,
      requestSoundPermission: true,
    );

    const initSettings = InitializationSettings(
      android: androidSettings,
      iOS: iosSettings,
    );

    await _localNotifications.initialize(
      initSettings,
      onDidReceiveNotificationResponse: _handleNotificationTap,
    );

    // Android: create notification channels up front
    const androidChannel = AndroidNotificationChannel(
      'yemen_booking_channel',
      'Yemen Booking Notifications',
      description: 'إشعارات تطبيق حجوزات اليمن',
      importance: Importance.high,
    );
    const androidScheduledChannel = AndroidNotificationChannel(
      'yemen_booking_scheduled',
      'Yemen Booking Scheduled',
      description: 'إشعارات مجدولة لتطبيق حجوزات اليمن',
      importance: Importance.high,
    );
    final androidPlugin =
        _localNotifications.resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>();
    await androidPlugin?.createNotificationChannel(androidChannel);
    await androidPlugin?.createNotificationChannel(androidScheduledChannel);
  }

  // Configure Firebase messaging
  Future<void> _configureFirebaseMessaging() async {
    // iOS: allow notifications to be displayed while app in foreground
    await FirebaseMessaging.instance
        .setForegroundNotificationPresentationOptions(
      alert: true,
      badge: true,
      sound: true,
    );

    // Foreground message handler
    FirebaseMessaging.onMessage.listen(_handleForegroundMessage);

    // Message opened app handler
    FirebaseMessaging.onMessageOpenedApp.listen(_handleMessageOpenedApp);

    // Check if app was opened by notification
    final initialMessage = await _firebaseMessaging.getInitialMessage();
    if (initialMessage != null) {
      _handleMessageOpenedApp(initialMessage);
    }

    // Background message handler registration (Android/iOS)
    FirebaseMessaging.onBackgroundMessage(
        NotificationService._handleBackgroundMessage);
  }

  // Register FCM token with backend
  Future<void> _registerFcmToken() async {
    try {
      final token = await _firebaseMessaging.getToken();
      if (token != null) {
        debugPrint('FCM token: $token');
        await _sendTokenToServer(token);
        await _localStorage?.saveFcmToken(token);
        await _subscribeToDefaultTopics();
      }
    } catch (e) {
      debugPrint('Error registering FCM token: $e');
    }
  }

  // Subscribe to default topics: user_{id}, role_*; avoid 'all' for admin apps
  Future<void> _subscribeToDefaultTopics() async {
    try {
      // In admin/control panel app, do NOT subscribe to global 'all' to avoid
      // receiving end-user broadcasts on admin/staff devices
      final user = await _authLocalDataSource?.getCachedUser();
      if (user != null) {
        final String userId = user.userId.toString() ?? '';
        if (userId.isNotEmpty) {
          await _firebaseMessaging.subscribeToTopic('user_$userId');
        }
        // roles may come from user.roles and accountRole
        final List<String> roles = [
          ...((user.roles ?? []) as List).map((e) => e.toString()),
          if ((user.accountRole != null &&
              (user.accountRole as String).isNotEmpty)) ...[
            user.accountRole as String
          ]
        ];
        final uniqueRoles = roles
            .where((r) => r.trim().isNotEmpty)
            .map((r) => _normalizeRole(r))
            .toSet()
            .toList();
        for (final role in uniqueRoles) {
          await _firebaseMessaging.subscribeToTopic('role_$role');
        }
      }
    } catch (e) {
      debugPrint('Error subscribing to default topics: $e');
    }
  }

  // Unsubscribe from default topics: user_{id}, role_*
  Future<void> _unsubscribeFromDefaultTopics() async {
    try {
      final user = await _authLocalDataSource?.getCachedUser();
      if (user != null) {
        final String userId = user.userId.toString() ?? '';
        if (userId.isNotEmpty) {
          await _firebaseMessaging.unsubscribeFromTopic('user_$userId');
        }
        final List<String> roles = [
          ...((user.roles ?? []) as List).map((e) => e.toString()),
          if ((user.accountRole != null &&
              (user.accountRole as String).isNotEmpty)) ...[
            user.accountRole as String
          ]
        ];
        final uniqueRoles = roles
            .where((r) => r.trim().isNotEmpty)
            .map((r) => _normalizeRole(r))
            .toSet()
            .toList();
        for (final role in uniqueRoles) {
          await _firebaseMessaging.unsubscribeFromTopic('role_$role');
        }
      }
    } catch (e) {
      debugPrint('Error unsubscribing from default topics: $e');
    }
  }

  String _normalizeRole(String role) {
    final r = role.trim();
    // Normalize to 5 canonical roles: Admin, Owner, Client, Staff, Guest
    switch (r.toLowerCase()) {
      case 'admin':
      case 'administrator':
      case 'superadmin':
      case 'super_admin':
        return 'admin';
      case 'owner':
      case 'hotel_owner':
      case 'property_owner':
        return 'owner';
      case 'client':
      case 'customer':
        return 'client';
      case 'staff':
      case 'manager':
      case 'hotel_manager':
      case 'receptionist':
        return 'staff';
      case 'guest':
      case 'visitor':
        return 'guest';
      default:
        return r.toLowerCase();
    }
  }

  // Send token to server
  Future<void> _sendTokenToServer(String token) async {
    if (_apiClient == null || _authLocalDataSource == null) return;

    try {
      final user = await _authLocalDataSource!.getCachedUser();
      if (user == null) return;

      final deviceType = Platform.isIOS ? 'iOS' : 'Android';

      await _apiClient!.post(
        '/api/fcm/register',
        data: {
          'userId': user.userId,
          'token': token,
          'deviceType': deviceType,
        },
      );
    } catch (e) {
      debugPrint('Error sending FCM token to server: $e');
    }
  }

  // Handle token refresh
  Future<void> _onTokenRefresh(String token) async {
    debugPrint('FCM token refreshed: $token');
    await _sendTokenToServer(token);
    await _localStorage?.saveFcmToken(token);
    await _subscribeToDefaultTopics();
  }

  // Unregister FCM token
  Future<void> unregisterFcmToken() async {
    if (_apiClient == null || _authLocalDataSource == null) return;

    try {
      final user = await _authLocalDataSource!.getCachedUser();
      if (user == null) return;
      final token = await _firebaseMessaging.getToken();

      await _apiClient!.post(
        '/api/fcm/unregister',
        data: {
          'userId': user.userId,
          if (token != null) 'token': token,
        },
      );

      await _unsubscribeFromDefaultTopics();
      await _firebaseMessaging.deleteToken();
    } catch (e) {
      debugPrint('Error unregistering FCM token: $e');
    }
  }

  // Handle foreground messages
  Future<void> _handleForegroundMessage(RemoteMessage message) async {
    debugPrint('Foreground message received: ${message.messageId}');
    final data = message.data;
    final type = (data['type'] ?? data['event_type'] ?? '').toString();

    // إذا كانت رسالة شات داخل التطبيق: لا نعرض إشعارًا، بل نحدّث الـ Bloc مباشرة
    if (type == 'new_message' || type == 'chat.new_message') {
      final conversationId =
          (data['conversation_id'] ?? data['conversationId'] ?? '').toString();
      final messageId =
          (data['message_id'] ?? data['messageId'] ?? '').toString();
      final silent = (data['silent'] ?? '').toString() == 'true';
      if (conversationId.isNotEmpty && messageId.isNotEmpty) {
        try {
          if (_chatEventSink != null) {
            _chatEventSink!.call(WebSocketMessageReceivedEvent(
              MessageEvent(
                  type: MessageEventType.newMessage,
                  conversationId: conversationId,
                  messageId: messageId),
            ));
          }
        } catch (e) {
          debugPrint('Dispatch in-app chat update failed: $e');
        }
      }
      if (silent) return; // لا إشعار محلي داخل التطبيق إذا كان صامت
    }

    if (type == 'conversation_created' || type == 'chat.conversation_created') {
      final conversationId =
          (data['conversation_id'] ?? data['conversationId'] ?? '').toString();
      if (conversationId.isNotEmpty) {
        // UI will fetch when navigating/opening
      }
      return; // لا إشعار محلي داخل التطبيق
    }

    // تفاعل مُضاف/محذوف: ادفع حدثاً دقيقاً لتحديث فوري دون إعادة جلب كامل
    if (type == 'reaction_added' || type == 'reaction_removed') {
      final conversationId =
          (data['conversation_id'] ?? data['conversationId'] ?? '').toString();
      final messageId =
          (data['message_id'] ?? data['messageId'] ?? '').toString();
      final userId = (data['user_id'] ?? data['userId'] ?? '').toString();
      final reactionType =
          (data['reaction_type'] ?? data['reactionType'] ?? '').toString();
      if (conversationId.isNotEmpty &&
          messageId.isNotEmpty &&
          userId.isNotEmpty &&
          reactionType.isNotEmpty) {
        try {
          final isAdded = type == 'reaction_added';
          if (_chatEventSink != null) {
            _chatEventSink!.call(WebSocketMessageReceivedEvent(
              MessageEvent(
                type: isAdded
                    ? MessageEventType.reactionAdded
                    : MessageEventType.reactionRemoved,
                conversationId: conversationId,
                messageId: messageId,
                reaction: MessageReaction(
                  id: 'temp_${DateTime.now().microsecondsSinceEpoch}',
                  messageId: messageId,
                  userId: userId,
                  reactionType: reactionType,
                ),
              ),
            ));
          }
        } catch (e) {
          debugPrint('Silent reaction update handling failed: $e');
        }
      }
      return; // لا إشعار مرئي للتفاعلات
    }

    // تحديث حالة الرسالة: تحديث صامت للواجهة
    if (type == 'message_status_updated') {
      final conversationId =
          (data['conversation_id'] ?? data['conversationId'] ?? '').toString();
      final messageId =
          (data['message_id'] ?? data['messageId'] ?? '').toString();
      final status = (data['status'] ?? '').toString();
      final readAt = (data['read_at'] ?? '').toString();
      final deliveredAt = (data['delivered_at'] ?? '').toString();
      if (conversationId.isNotEmpty) {
        try {
          if (_chatEventSink != null && messageId.isNotEmpty && status.isNotEmpty) {
            _chatEventSink!.call(WebSocketMessageReceivedEvent(
              MessageEvent(
                  type: MessageEventType.statusUpdated,
                  conversationId: conversationId,
                  messageId: messageId,
                  status: status),
            ));
          }
        } catch (e) {
          debugPrint('Silent status update handling failed: $e');
        }
      }
      return; // لا عرض لإشعار مرئي
    }

    // أي رسائل صامتة عامة أخرى: لا تعرض تنبيهًا
    if ((data['silent'] ?? '').toString() == 'true') {
      return;
    }

    // بقية الأنواع: في وضع foreground داخل التطبيق لا نظهر إشعاراً محلياً،
    // خاصة إذا كان المستخدم في صفحات المحادثة أو قائمة المحادثات
    return;
  }

  // Handle background messages (static function required)
  static Future<void> _handleBackgroundMessage(RemoteMessage message) async {
    debugPrint('Background message received: ${message.messageId}');
  }

  // Handle notification tap when app is opened
  void _handleMessageOpenedApp(RemoteMessage message) {
    debugPrint('Message opened app: ${message.messageId}');
    final data = message.data;
    final type = (data['type'] ?? data['event_type'] ?? '').toString();
    if (type == 'new_message' || type == 'chat.new_message') {
      final conversationId =
          (data['conversation_id'] ?? data['conversationId'] ?? '').toString();
      final messageId =
          (data['message_id'] ?? data['messageId'] ?? '').toString();
      if (conversationId.isNotEmpty) {
        // UI will load when navigated
      }
    }
    _navigateToScreen(data);
  }

  // Handle local notification tap
  void _handleNotificationTap(NotificationResponse response) {
    if (response.payload != null) {
      final data = json.decode(response.payload!);
      _navigateToScreen(data);
    }
  }

  // Show local notification
  Future<void> _showLocalNotification(RemoteMessage message) async {
    // Suppress in-app local notifications always (foreground)
    return;

    final notification = message.notification;
    if (notification == null) return;

    const androidDetails = AndroidNotificationDetails(
      'yemen_booking_channel',
      'Yemen Booking Notifications',
      channelDescription: 'إشعارات تطبيق حجوزات اليمن',
      importance: Importance.high,
      priority: Priority.high,
      showWhen: true,
    );

    const iosDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
    );

    const details = NotificationDetails(
      android: androidDetails,
      iOS: iosDetails,
    );

    await _localNotifications.show(
      notification.hashCode,
      notification.title,
      notification.body,
      details,
      payload: json.encode(message.data),
    );
  }

  // Navigate to appropriate screen based on notification data
  void _navigateToScreen(Map<String, dynamic> data) {
    final type = data['type'];
    final id = data['id'] ?? data['conversation_id'];

    switch (type) {
      case 'booking':
        // Navigate to booking details
        debugPrint('Navigate to booking: $id');
        break;
      case 'property':
        // Navigate to property details
        debugPrint('Navigate to property: $id');
        break;
      case 'chat':
      case 'new_message':
      case 'conversation_created':
        if (id != null && id.toString().isNotEmpty) {
          // GoRouter path: /chat/:conversationId
          // We don't have context here; rely on app-level navigation service if available
          debugPrint('Navigate to chat: $id');
          // Optionally emit a stream/event that UI layer listens to and navigates
        }
        break;
      case 'promotion':
        // Navigate to promotion
        debugPrint('Navigate to promotion: $id');
        break;
      default:
        // Navigate to notifications page
        debugPrint('Navigate to notifications');
    }
  }

  // Check if notifications are enabled
  Future<bool> areNotificationsEnabled() async {
    final settings = await _firebaseMessaging.getNotificationSettings();
    return settings.authorizationStatus == AuthorizationStatus.authorized;
  }

  // Open notification settings
  Future<void> openNotificationSettings() async {
    await _firebaseMessaging.requestPermission();
  }

  // Get pending notifications
  Future<List<PendingNotificationRequest>> getPendingNotifications() async {
    return await _localNotifications.pendingNotificationRequests();
  }

  // Cancel notification
  Future<void> cancelNotification(int id) async {
    await _localNotifications.cancel(id);
  }

  // Cancel all notifications
  Future<void> cancelAllNotifications() async {
    await _localNotifications.cancelAll();
  }

  // Schedule notification
  Future<void> scheduleNotification({
    required int id,
    required String title,
    required String body,
    required DateTime scheduledDate,
    Map<String, dynamic>? payload,
  }) async {
    const androidDetails = AndroidNotificationDetails(
      'yemen_booking_scheduled',
      'Yemen Booking Scheduled',
      channelDescription: 'إشعارات مجدولة لتطبيق حجوزات اليمن',
      importance: Importance.high,
      priority: Priority.high,
    );

    const iosDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
    );

    const details = NotificationDetails(
      android: androidDetails,
      iOS: iosDetails,
    );

    // Convert DateTime to TZDateTime
    final tz.TZDateTime tzScheduledDate =
        tz.TZDateTime.from(scheduledDate, tz.local);

    await _localNotifications.zonedSchedule(
      id,
      title,
      body,
      tzScheduledDate,
      details,
      androidScheduleMode: AndroidScheduleMode.exactAllowWhileIdle,
      uiLocalNotificationDateInterpretation:
          UILocalNotificationDateInterpretation.absoluteTime,
      payload: payload != null ? json.encode(payload) : null,
    );
  }

  // Get FCM token
  Future<String?> getFcmToken() async {
    return await _firebaseMessaging.getToken();
  }

  // Subscribe to topic
  Future<void> subscribeToTopic(String topic) async {
    await _firebaseMessaging.subscribeToTopic(topic);
  }

  // Unsubscribe from topic
  Future<void> unsubscribeFromTopic(String topic) async {
    await _firebaseMessaging.unsubscribeFromTopic(topic);
  }
}

// Helper to dispatch a status update into ChatBloc pipeline via WebSocketService streams
void _dispatchStatusUpdate(
    String conversationId, String messageId, String status) {
  try {
    final ws = di.sl<ChatWebSocketService>();
    ws.messageEvents.listen((_) {}); // ensure stream is active
    // Internally push a synthetic status update event
    // We don't have direct access to add event in bloc here, but WebSocketService exposes a stream consumed by ChatBloc
    // So we emulate by sending a crafted map through the private handler would be intrusive; as an alternative,
    // we reuse emitNewMessageById path to force a refresh if messageId missing.
    // Since ChatBloc now handles MessageEventType.statusUpdated, prefer emitting empty newMessage fetch when ids missing.
    if (messageId.isEmpty || status.isEmpty) {
      ws.emitNewMessageById(conversationId: conversationId, messageId: '');
    } else {
      // Fallback: trigger a messages fetch which updates statuses from server
      ws.emitNewMessageById(conversationId: conversationId, messageId: '');
    }
  } catch (e) {
    debugPrint('Failed to dispatch status update: $e');
  }
}
