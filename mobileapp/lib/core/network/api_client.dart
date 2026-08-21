import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

/// Owns the single [Dio] instance used by all services.
///
/// The base URL can change at runtime (Settings screen). The auth token and
/// 401-handling are wired by [AuthProvider] via [tokenProvider] and
/// [onUnauthorized] so this layer stays free of app-state dependencies.
class ApiClient {
  ApiClient._();
  static final ApiClient instance = ApiClient._();

  late final Dio dio;
  bool _initialized = false;

  /// Returns the current bearer token (or null). Set by AuthProvider.
  String? Function()? tokenProvider;

  /// Called when any request fails with 401. Set by AuthProvider.
  void Function()? onUnauthorized;

  void init(String baseUrl) {
    if (_initialized) {
      updateBaseUrl(baseUrl);
      return;
    }
    _initialized = true;

    dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        // Fail fast when the server is unreachable (wrong URL, backend down,
        // firewall) so the UI shows an actionable error instead of hanging.
        connectTimeout: const Duration(seconds: 10),
        sendTimeout: const Duration(seconds: 20),
        receiveTimeout: const Duration(seconds: 20),
      ),
      // Content-Type is left unset so Dio's ImplyContentTypeInterceptor picks
      // application/json for Map bodies and multipart/form-data (with the
      // correct boundary) for FormData uploads automatically.
    );

    // Lightweight request/response tracing (debug builds only) — makes login
    // and other calls visible in `flutter run` logs without leaking anything
    // in release builds.
    if (kDebugMode) {
      dio.interceptors.add(
        InterceptorsWrapper(
          onRequest: (options, handler) {
            debugPrint('→ ${options.method} ${options.uri}');
            handler.next(options);
          },
          onResponse: (response, handler) {
            debugPrint(
                '← ${response.statusCode} ${response.requestOptions.uri}');
            handler.next(response);
          },
          onError: (e, handler) {
            debugPrint('✗ ${e.type} ${e.requestOptions.uri} '
                '(status ${e.response?.statusCode}) ${e.message}');
            handler.next(e);
          },
        ),
      );
    }

    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          final token = tokenProvider?.call();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (e, handler) {
          if (e.response?.statusCode == 401) {
            onUnauthorized?.call();
          }
          handler.next(e);
        },
      ),
    );
  }

  void updateBaseUrl(String baseUrl) {
    dio.options.baseUrl = baseUrl;
  }
}
