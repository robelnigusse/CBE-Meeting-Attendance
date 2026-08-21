import 'package:dio/dio.dart';

import 'base_service.dart';

class ProfileService extends BaseService {
  /// POST /profile/upload (multipart) — [employeeId] is the human string id
  /// (e.g. "EMP001"), NOT the GUID.
  Future<String> upload({
    required String employeeId,
    required String imagePath,
  }) async {
    final form = FormData.fromMap({
      'EmployeeId': employeeId,
      'Image': await MultipartFile.fromFile(imagePath),
    });
    final res = await send(() => dio.post('/profile/upload', data: form));
    return (res.data['message'] as String?) ?? '';
  }

  /// GET /profile/{employeeId} — JPEG bytes, or null when there's no image
  /// (backend returns 404). Public endpoint.
  Future<List<int>?> getImage(String employeeId) async {
    try {
      final res = await dio.get<List<int>>(
        '/profile/$employeeId',
        options: Options(responseType: ResponseType.bytes),
      );
      return res.data;
    } on DioException {
      return null;
    }
  }
}
