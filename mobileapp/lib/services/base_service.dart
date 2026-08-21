import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../core/network/api_exception.dart';

/// Base for all API services.
///
/// Provides the shared [dio] instance and [send], which:
///  * converts transport/HTTP errors into [ApiException] (localized keys for
///    synthetic errors, raw backend message otherwise), and
///  * enforces the `ApiResponse.success` flag for endpoints that return HTTP
///    200 even on logical failure (login, user register, change-role,
///    change-password all do this on the backend).
abstract class BaseService {
  Dio get dio => ApiClient.instance.dio;

  Future<Response<T>> send<T>(Future<Response<T>> Function() request) async {
    try {
      final res = await request();
      final data = res.data;
      if (data is Map && data['success'] == false) {
        final message = (data['message'] as String?)?.trim();
        throw ApiException(
          message != null && message.isNotEmpty ? message : 'common.error',
          statusCode: res.statusCode,
        );
      }
      return res;
    } on DioException catch (e) {
      throw toApiException(e);
    }
  }
}
