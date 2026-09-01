import '../models/attendance.dart';
import 'base_service.dart';

class AttendanceService extends BaseService {
  /// POST /attendance — 200 on success; 400 with a message on
  /// "Employee not found." / "Attendance already taken." (surfaced as
  /// [ApiException] via [send]).
  Future<AttendanceResult> takeAttendance(String employeeId, double latitude, double longitude) async {
    final res = await send(() => dio.post(
          '/attendance',
          data: {
            'employeeId': employeeId,
            'latitude': latitude,
            'longitude': longitude,
          },
        ));
    return AttendanceResult.fromJson((res.data['data'] as Map).cast<String, dynamic>());
  }

  /// GET /attendance/check?employeeId= — 404 when the employee is unknown.
  Future<AttendanceResult> check(String employeeId) async {
    final res = await send(() => dio.get(
          '/attendance/check',
          queryParameters: {'employeeId': employeeId},
        ));
    return AttendanceResult.fromJson((res.data['data'] as Map).cast<String, dynamic>());
  }
}
