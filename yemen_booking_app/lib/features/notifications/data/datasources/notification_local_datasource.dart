import '../../../../services/local_storage_service.dart';
import '../models/notification_model.dart';

abstract class NotificationLocalDataSource {
  Future<List<NotificationModel>> getCachedNotifications();
  Future<void> cacheNotifications(List<NotificationModel> notifications);
  Future<void> clearCache();
  Future<Map<String, bool>> getNotificationSettings();
  Future<void> saveNotificationSettings(Map<String, bool> settings);
}

class NotificationLocalDataSourceImpl implements NotificationLocalDataSource {
  final LocalStorageService localStorage;

  NotificationLocalDataSourceImpl({required this.localStorage});

  @override
  Future<List<NotificationModel>> getCachedNotifications() async {
    // TODO: Implement local storage
    return [];
  }

  @override
  Future<void> cacheNotifications(List<NotificationModel> notifications) async {
    // TODO: Implement local storage
  }

  @override
  Future<void> clearCache() async {
    // TODO: Implement local storage
  }

  @override
  Future<Map<String, bool>> getNotificationSettings() async {
    // TODO: Implement local storage
    return {};
  }

  @override
  Future<void> saveNotificationSettings(Map<String, bool> settings) async {
    // TODO: Implement local storage
  }
}