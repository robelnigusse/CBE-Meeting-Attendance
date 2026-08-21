import 'package:dio/dio.dart';

/// A normalized error carrying a user-facing [message] and optional [statusCode].
class ApiException implements Exception {
  ApiException(this.message, {this.statusCode});

  final String message;
  final int? statusCode;

  @override
  String toString() => message;
}

/// Converts any thrown error (usually a [DioException]) into an [ApiException]
/// with the friendliest message we can extract from the backend response.
ApiException toApiException(Object error) {
  if (error is ApiException) return error;

  if (error is DioException) {
    final status = error.response?.statusCode;
    final data = error.response?.data;

    // Backend wraps everything in ApiResponse { success, message, data }.
    if (data is Map && data['message'] is String &&
        (data['message'] as String).trim().isNotEmpty) {
      return ApiException(data['message'] as String, statusCode: status);
    }

    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.connectionError:
        return ApiException('common.networkError', statusCode: status);
      default:
        if (status == 401) {
          return ApiException('common.sessionExpired', statusCode: 401);
        }
        return ApiException('common.error', statusCode: status);
    }
  }

  return ApiException('common.error');
}
