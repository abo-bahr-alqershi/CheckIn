import 'package:flutter/foundation.dart' show kIsWeb;
import '../constants/api_constants.dart';

class ImageUtils {
  static String resolveUrl(String? url) {
    if (url == null || url.trim().isEmpty) return '';
    final trimmed = url.trim();
    final lower = trimmed.toLowerCase();
    if (lower.startsWith('http://') || lower.startsWith('https://')) {
      // If running on web over HTTPS, avoid mixed content by upgrading scheme when same host
      if (kIsWeb) {
        try {
          final currentOrigin = Uri.base.origin; // e.g., https://host[:port]
          final u = Uri.parse(trimmed);
          final originUri = Uri.parse(currentOrigin);
          if (u.scheme == 'http' && originUri.scheme == 'https' && u.host == originUri.host && u.port == originUri.port) {
            return u.replace(scheme: 'https').toString();
          }
        } catch (_) {}
      }
      return trimmed;
    }
    // Ensure single slash between base and path
    final String base = kIsWeb
        ? Uri.base.origin // match the app's origin to avoid mixed-content on web
        : (ApiConstants.imageBaseUrl.endsWith('/')
            ? ApiConstants.imageBaseUrl.substring(0, ApiConstants.imageBaseUrl.length - 1)
            : ApiConstants.imageBaseUrl);
    final path = trimmed.startsWith('/') ? trimmed : '/$trimmed';
    return '$base$path';
  }
}