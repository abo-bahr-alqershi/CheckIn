import 'dart:async';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import '../../services/local_storage_service.dart';
import '../../services/message_service.dart';
import '../constants/storage_constants.dart';
import '../constants/api_constants.dart';
import '../localization/locale_manager.dart';
import '../bloc/app_bloc.dart';
import '../../features/auth/presentation/bloc/auth_event.dart';
import '../../injection_container.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';

class AuthInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    // Allow skipping auth header for specific requests
    if (options.extra['skipAuth'] == true) {
      return handler.next(options);
    }
    final localStorage = sl<LocalStorageService>();
    final token = localStorage.getData(StorageConstants.accessToken) as String?;
    
    if (token != null && token.isNotEmpty) {
      // Always overwrite Authorization with the latest token
      options.headers[ApiConstants.authorization] = '${ApiConstants.bearer} $token';
    }
    // propagate role/property context for backend if needed
    final accountRole = localStorage.getData(StorageConstants.accountRole)?.toString();
    final propertyId = localStorage.getData(StorageConstants.propertyId)?.toString();
    final propertyCurrency = localStorage.getData(StorageConstants.propertyCurrency)?.toString();
    if (accountRole != null && accountRole.isNotEmpty) {
      options.headers['X-Account-Role'] = accountRole;
    }
    if (propertyId != null && propertyId.isNotEmpty) {
      options.headers['X-Property-Id'] = propertyId;
    }
    if (propertyCurrency != null && propertyCurrency.isNotEmpty) {
      options.headers['X-Property-Currency'] = propertyCurrency;
    }
    
    // Add current language to headers
    final locale = LocaleManager.getCurrentLocale();
    options.headers[ApiConstants.acceptLanguage] = locale.languageCode;
    
    handler.next(options);
  }
}

/// Interceptor to surface server messages on successful HTTP responses
/// when backend embeds ResultDto with success/isSuccess flags.
class UserFeedbackInterceptor extends Interceptor {
  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    try {
      final extra = response.requestOptions.extra;
      final data = response.data;
      if (data is Map<String, dynamic>) {
        final hasSuccessKey = data.containsKey('success') || data.containsKey('isSuccess');
        final bool? isSuccess = data['success'] as bool? ?? data['isSuccess'] as bool?;
        final String message = _extractResponseMessage(data);

        if (hasSuccessKey && isSuccess == false) {
          final suppressed = (extra['suppressErrorToast'] == true);
          if (!suppressed && message.isNotEmpty) {
            MessageService.showError(message);
          }
        } else if (hasSuccessKey && isSuccess == true) {
          final showSuccess = (extra['showSuccessToast'] == true);
          if (showSuccess && message.isNotEmpty) {
            MessageService.showSuccess(message);
          }
        }
      }
    } catch (_) {}

    handler.next(response);
  }
}

String _extractResponseMessage(Map<String, dynamic> data) {
  final msg = (data['message'] ?? data['error'] ?? '').toString();
  if (msg.trim().isNotEmpty) return msg;
  final errors = data['errors'];
  if (errors is List) {
    final joined = errors.map((e) => e.toString()).join('\n');
    if (joined.trim().isNotEmpty) return joined;
  } else if (errors is Map) {
    final List<String> messages = [];
    errors.forEach((key, value) {
      if (value is List) {
        messages.addAll(value.map((e) => e.toString()));
      } else if (value != null) {
        messages.add(value.toString());
      }
    });
    final joined = messages.join('\n');
    if (joined.trim().isNotEmpty) return joined;
  }
  return '';
}

class ErrorInterceptor extends Interceptor {
  ErrorInterceptor(this._dio);

  final Dio _dio;
  static bool _isRefreshing = false;
  static Completer<void>? _refreshCompleter;

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    final int? status = err.response?.statusCode;
    final RequestOptions requestOptions = err.requestOptions;

    // Skip refresh flow for specific requests (e.g., refresh endpoint itself)
    if (requestOptions.extra['skipRefresh'] == true) {
      return handler.next(err);
    }

    // Show validation/server messages globally unless suppressed
    try {
      final suppressed = (requestOptions.extra['suppressErrorToast'] == true);
      if (!suppressed) {
        final message = _extractErrorMessage(err);
        if (message.isNotEmpty) {
          MessageService.showError(message);
        }
      }
    } catch (_) {}

    // Only handle 401 Unauthorized (token refresh flow)
    if (status == 401) {
      try {
        final localStorage = sl<LocalStorageService>();
        final String? refreshToken = localStorage.getData(StorageConstants.refreshToken) as String?;

        // If no refresh token, logout
        if (refreshToken == null || refreshToken.isEmpty) {
          await _forceLogout();
          return handler.next(err);
        }

        // If already retried once, avoid infinite loop
        if (requestOptions.extra['retried'] == true) {
          await _forceLogout();
          return handler.next(err);
        }

        // If a refresh is already happening, wait for it
        if (_isRefreshing) {
          try {
            await (_refreshCompleter ?? Completer<void>()..complete()).future;
          } catch (_) {}
        } else {
          // Start refresh
          _isRefreshing = true;
          _refreshCompleter = Completer<void>();
          try {
            await _refreshAccessToken(refreshToken);
            _refreshCompleter?.complete();
          } catch (e) {
            _refreshCompleter?.completeError(e);
            await _forceLogout();
            _isRefreshing = false;
            return handler.next(err);
          }
          _isRefreshing = false;
        }

        // Retry the original request with updated token
        final String? newAccess = localStorage.getData(StorageConstants.accessToken) as String?;
        if (newAccess == null || newAccess.isEmpty) {
          await _forceLogout();
          return handler.next(err);
        }

        final Options newOptions = Options(
          method: requestOptions.method,
          headers: {
            ...requestOptions.headers,
            ApiConstants.authorization: '${ApiConstants.bearer} $newAccess',
          },
          responseType: requestOptions.responseType,
          contentType: requestOptions.contentType,
          followRedirects: requestOptions.followRedirects,
          validateStatus: requestOptions.validateStatus,
          receiveDataWhenStatusError: requestOptions.receiveDataWhenStatusError,
          extra: {
            ...requestOptions.extra,
            'retried': true,
          },
        );

        final Response response = await _dio.request(
          requestOptions.path,
          data: requestOptions.data,
          queryParameters: requestOptions.queryParameters,
          options: newOptions,
          cancelToken: requestOptions.cancelToken,
          onReceiveProgress: requestOptions.onReceiveProgress,
          onSendProgress: requestOptions.onSendProgress,
        );
        return handler.resolve(response);
      } catch (_) {
        await _forceLogout();
      }
    }
    
    handler.next(err);
  }
  
  Future<void> _refreshAccessToken(String refreshToken) async {
    final authRepository = sl<AuthRepository>();
    await authRepository.refreshToken(refreshToken: refreshToken);
  }

  Future<void> _forceLogout() async {
    try {
      // Clear local storages first
    final localStorage = sl<LocalStorageService>();
    await localStorage.removeData(StorageConstants.accessToken);
    await localStorage.removeData(StorageConstants.refreshToken);
    } catch (_) {}
    // Dispatch logout to trigger router redirect
    try {
      AppBloc.authBloc.add(const LogoutEvent());
    } catch (_) {}
  }
}

String _extractErrorMessage(DioException err) {
  final data = err.response?.data;
  // Backend ResultDto pattern: { success, message, errors, errorCode, ... }
  if (data is Map<String, dynamic>) {
    // Prefer explicit message when present
    final explicitMessage = (data['message'] ?? data['error'] ?? '').toString();
    if (explicitMessage.trim().isNotEmpty) return explicitMessage;

    // errors may be List or Map of field=>[errors]
    final errors = data['errors'];
    if (errors is List) {
      final joined = errors.map((e) => e.toString()).join('\n');
      if (joined.trim().isNotEmpty) return joined;
    } else if (errors is Map) {
      final List<String> messages = [];
      errors.forEach((key, value) {
        if (value is List) {
          messages.addAll(value.map((e) => e.toString()));
        } else if (value != null) {
          messages.add(value.toString());
        }
      });
      final joined = messages.join('\n');
      if (joined.trim().isNotEmpty) return joined;
    }
  }

  // Fallback based on status code
  final statusCode = err.response?.statusCode;
  switch (statusCode) {
    case 400:
      return 'طلب غير صحيح';
    case 401:
      return 'غير مصرح بالوصول';
    case 403:
      return 'ليس لديك صلاحية';
    case 404:
      return 'لم يتم العثور على البيانات';
    case 422:
      return 'البيانات المدخلة غير صحيحة';
    case 500:
      return 'خطأ في الخادم';
    default:
      return err.message ?? 'حدث خطأ غير متوقع';
  }
}

class LoggingInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    debugPrint('REQUEST[${options.method}] => PATH: ${options.path}');
    handler.next(options);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    debugPrint('RESPONSE[${response.statusCode}] => PATH: ${response.requestOptions.path}');
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    debugPrint('ERROR[${err.response?.statusCode}] => PATH: ${err.requestOptions.path}');
    handler.next(err);
  }
}