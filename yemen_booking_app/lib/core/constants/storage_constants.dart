class StorageConstants {
  StorageConstants._();
  
  // Secure Storage Keys
  static const String accessToken = 'access_token';
  static const String refreshToken = 'refresh_token';
  static const String userId = 'user_id';
  static const String userEmail = 'user_email';
  
  // Shared Preferences Keys
  static const String firstLaunch = 'first_launch';
  static const String language = 'language';
  static const String theme = 'theme';
  static const String notificationsEnabled = 'notifications_enabled';
  static const String biometricEnabled = 'biometric_enabled';
  static const String rememberMe = 'remember_me';
  static const String searchHistory = 'search_history';
  static const String recentProperties = 'recent_properties';
  
  // Cache Keys Prefixes
  static const String propertyCachePrefix = 'property_';
  static const String userCachePrefix = 'user_';
  static const String bookingCachePrefix = 'booking_';
  static const String imageCachePrefix = 'image_';
  
  // Database Names
  static const String mainDatabase = 'yemen_booking.db';
  static const String cacheDatabase = 'yemen_booking_cache.db';
  
  // Table Names
  static const String favoritesTable = 'favorites';
  static const String messagesTable = 'messages';
  static const String notificationsTable = 'notifications';
}