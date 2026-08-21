import 'package:dio/dio.dart';

import '../models/employee.dart';
import 'base_service.dart';

class EmployeeService extends BaseService {
  /// GET /employees
  Future<List<Employee>> getAll() async {
    final res = await send(() => dio.get('/employees'));
    final list = (res.data['data'] as List?) ?? const [];
    return list
        .map((e) => Employee.fromJson((e as Map).cast<String, dynamic>()))
        .toList();
  }

  /// GET /employees/id/{guid} — resolves the login GUID to the full employee
  /// (and its human "EMP001" string id). Returns null if not found.
  Future<Employee?> getByGuid(String guid) async {
    try {
      final res = await dio.get('/employees/id/$guid');
      final data = res.data is Map ? res.data['data'] : null;
      if (data == null) return null;
      return Employee.fromJson((data as Map).cast<String, dynamic>());
    } on DioException {
      return null;
    }
  }

  /// POST /employees/register (multipart). [imagePath] optional.
  Future<String> register({
    required String employeeId,
    required String fullName,
    String division = '',
    String jobTitle = '',
    String department = '',
    String phoneNumber = '',
    String? imagePath,
  }) async {
    final form = FormData.fromMap({
      'EmployeeId': employeeId,
      'FullName': fullName,
      'Division': division,
      'JobTitle': jobTitle,
      'Department': department,
      'PhoneNumber': phoneNumber,
      if (imagePath != null && imagePath.isNotEmpty)
        'Image': await MultipartFile.fromFile(imagePath),
    });
    final res = await send(() => dio.post('/employees/register', data: form));
    return (res.data['message'] as String?) ?? '';
  }

  /// PUT /employees/{id} (JSON; id and image are not editable here).
  Future<String> update(
    String id, {
    required String fullName,
    String division = '',
    String jobTitle = '',
    String department = '',
    String phoneNumber = '',
  }) async {
    final res = await send(() => dio.put('/employees/$id', data: {
          'fullName': fullName,
          'division': division,
          'jobTitle': jobTitle,
          'department': department,
          'phoneNumber': phoneNumber,
        }));
    return (res.data['message'] as String?) ?? '';
  }

  /// DELETE /employees/{id}
  Future<String> delete(String id) async {
    final res = await send(() => dio.delete('/employees/$id'));
    return (res.data['message'] as String?) ?? '';
  }
}
