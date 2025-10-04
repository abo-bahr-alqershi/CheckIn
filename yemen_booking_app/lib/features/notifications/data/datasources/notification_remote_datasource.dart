import '../../../../core/models/paginated_result.dart';
import '../../../../core/models/result_dto.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/utils/request_logger.dart';
import '../models/notification_model.dart';

abstract class NotificationRemoteDataSource {
  Future<PaginatedResult<NotificationModel>> getNotifications({
    int page = 1,
    int limit = 20,
    String? type,
  });

  Future<void> markAsRead(String notificationId);

  Future<void> markAllAsRead({String? userId});

  Future<void> dismissNotification(String notificationId);

  Future<Map<String, bool>> getNotificationSettings({String? userId});

  Future<void> updateNotificationSettings(Map<String, bool> settings, {String? userId});

  Future<int> getUnreadCount({String? userId});
}

class NotificationRemoteDataSourceImpl implements NotificationRemoteDataSource {
  final ApiClient apiClient;

  NotificationRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PaginatedResult<NotificationModel>> getNotifications({
    int page = 1,
    int limit = 20,
    String? type,
  }) async {
    const requestName = 'notifications.getNotifications';
    logRequestStart(requestName, details: {
      'page': page,
      'limit': limit,
      if (type != null) 'type': type,
    });
    try {
      final response = await apiClient.get(
        '/api/client/notifications',
        queryParameters: {
          'pageNumber': page,
          'pageSize': limit,
          if (type != null) 'type': type,
        },
      );
      logRequestSuccess(requestName, statusCode: response.statusCode);

      if (response.statusCode == 200) {
        final result = ResultDto.fromJson(response.data as Map<String, dynamic>, (map) => map);
        final dynamic rawData = result.data;
        final Map<String, dynamic> dataMap =
            rawData is Map<String, dynamic> ? rawData : <String, dynamic>{};

        final List<dynamic> rawItems = (dataMap['items'] as List<dynamic>?) ??
            (dataMap['data'] as List<dynamic>?) ??
            <dynamic>[];

        final items = rawItems
            .where((e) => e != null)
            .map((e) => NotificationModel.fromJson(
                e is Map<String, dynamic> ? e : <String, dynamic>{}))
            .toList();

        final int pageNumber = (dataMap['pageNumber'] as int?) ??
            (dataMap['page'] as int?) ??
            page;
        final int pageSize = (dataMap['pageSize'] as int?) ??
            (dataMap['limit'] as int?) ??
            limit;
        final int totalCount = (dataMap['totalCount'] as int?) ??
            (dataMap['total'] as int?) ??
            items.length;

        return PaginatedResult<NotificationModel>(
          items: items,
          pageNumber: pageNumber,
          pageSize: pageSize,
          totalCount: totalCount,
        );
      }

      return const PaginatedResult(items: [], pageNumber: 1, pageSize: 20, totalCount: 0);
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }

  @override
  Future<void> markAsRead(String notificationId) async {
    const requestName = 'notifications.markAsRead';
    logRequestStart(requestName, details: {'notificationId': notificationId});
    try {
      await apiClient.put(
        '/api/client/notifications/mark-as-read',
        data: {
          'notificationId': notificationId,
        },
      );
      logRequestSuccess(requestName);
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }

  @override
  Future<void> markAllAsRead({String? userId}) async {
    const requestName = 'notifications.markAllAsRead';
    logRequestStart(requestName, details: {'userId': userId ?? ''});
    try {
      await apiClient.put(
        '/api/client/notifications/mark-all-as-read',
        data: {
          if (userId != null) 'userId': userId,
        },
      );
      logRequestSuccess(requestName);
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }

  @override
  Future<void> dismissNotification(String notificationId) async {
    const requestName = 'notifications.dismissNotification';
    logRequestStart(requestName, details: {'notificationId': notificationId});
    try {
      await apiClient.delete(
        '/api/client/notifications/dismiss',
        data: {
          'notificationId': notificationId,
        },
      );
      logRequestSuccess(requestName);
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }

  @override
  Future<Map<String, bool>> getNotificationSettings({String? userId}) async {
    const requestName = 'notifications.getNotificationSettings';
    logRequestStart(requestName, details: {'userId': userId ?? ''});
    logRequestSuccess(requestName);
    return {};
  }

  @override
  Future<void> updateNotificationSettings(Map<String, bool> settings, {String? userId}) async {
    const requestName = 'notifications.updateNotificationSettings';
    logRequestStart(requestName, details: {'userId': userId ?? ''});
    try {
      await apiClient.put(
        '/api/client/notifications/settings',
        data: {
          if (userId != null) 'userId': userId,
          'settings': settings,
        },
      );
      logRequestSuccess(requestName);
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }

  @override
  Future<int> getUnreadCount({String? userId}) async {
    const requestName = 'notifications.getUnreadCount';
    logRequestStart(requestName, details: {'userId': userId ?? ''});
    try {
      final response = await apiClient.get(
        '/api/client/notifications/summary',
        queryParameters: {
          if (userId != null) 'userId': userId,
        },
      );
      logRequestSuccess(requestName, statusCode: response.statusCode);
      if (response.statusCode == 200) {
        final result = ResultDto.fromJson(response.data, (json) => json);
        final data = result.data ?? {};
        return (data['unreadCount'] as int?) ?? 0;
      }
      return 0;
    } catch (e, s) {
      logRequestError(requestName, e, stackTrace: s);
      rethrow;
    }
  }
}