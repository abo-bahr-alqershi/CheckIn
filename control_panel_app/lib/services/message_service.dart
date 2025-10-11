import 'package:flutter/material.dart';

/// MessageService provides a global way to display user-facing messages
/// (errors, warnings, info, success) via a single ScaffoldMessenger.
class MessageService {
  MessageService._();

  static final GlobalKey<ScaffoldMessengerState> scaffoldMessengerKey =
      GlobalKey<ScaffoldMessengerState>();

  static const Duration _defaultDuration = Duration(seconds: 4);

  static String? _lastMessage;
  static DateTime? _lastShownAt;

  static bool _shouldSuppress(String message) {
    final now = DateTime.now();
    if (_lastMessage == message && _lastShownAt != null) {
      final since = now.difference(_lastShownAt!);
      if (since.inMilliseconds < 1200) return true; // throttle duplicates
    }
    _lastMessage = message;
    _lastShownAt = now;
    return false;
  }

  static void _showSnackBar(
    String message, {
    Color? backgroundColor,
    SnackBarAction? action,
    Duration? duration,
    SnackBarBehavior behavior = SnackBarBehavior.floating,
    EdgeInsetsGeometry margin = const EdgeInsets.all(12),
  }) {
    if (message.trim().isEmpty) return;
    if (_shouldSuppress(message)) return;

    final messenger = scaffoldMessengerKey.currentState;
    if (messenger == null) {
      debugPrint('MessageService: ScaffoldMessenger not ready. Message: $message');
      return;
    }
    messenger.clearSnackBars();
    messenger.showSnackBar(
      SnackBar(
        content: Text(message, textAlign: TextAlign.start),
        backgroundColor: backgroundColor,
        behavior: behavior,
        margin: margin,
        duration: duration ?? _defaultDuration,
        action: action,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }

  static void showError(String message) {
    _showSnackBar(
      message,
      backgroundColor: const Color(0xFFDC2626),
    );
  }

  static void showSuccess(String message) {
    _showSnackBar(
      message,
      backgroundColor: const Color(0xFF059669),
    );
  }

  static void showInfo(String message) {
    _showSnackBar(
      message,
      backgroundColor: const Color(0xFF0284C7),
    );
  }

  static void showWarning(String message) {
    _showSnackBar(
      message,
      backgroundColor: const Color(0xFFF59E0B),
    );
  }
}
