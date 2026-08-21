/// Result of POST /attendance (take attendance) and the shape used by
/// GET /attendance/check.
class AttendanceResult {
  AttendanceResult({
    required this.employeeId,
    this.fullName = '',
    this.meetingDate = '',
    this.session = '',
    this.attendanceTime,
    this.attended,
  });

  final String employeeId;
  final String fullName;
  final String meetingDate;
  final String session;
  final DateTime? attendanceTime;

  /// Only present on the /attendance/check response.
  final bool? attended;

  factory AttendanceResult.fromJson(Map<String, dynamic> json) {
    return AttendanceResult(
      employeeId: json['employeeId'] as String? ?? '',
      fullName: json['fullName'] as String? ?? '',
      meetingDate: json['meetingDate']?.toString() ?? '',
      session: json['session'] as String? ?? '',
      attendanceTime: json['attendanceTime'] == null
          ? null
          : DateTime.tryParse(json['attendanceTime'].toString()),
      attended: json['attended'] as bool?,
    );
  }
}
