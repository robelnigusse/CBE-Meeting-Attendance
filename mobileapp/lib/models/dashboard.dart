/// A row in the dashboard's "Today's Attendees" table.
class AttendeeSummary {
  AttendeeSummary({
    required this.employeeId,
    required this.fullName,
    this.department = '',
    this.division = '',
    this.session = '',
    this.meetingDate = '',
    this.attendanceTime,
  });

  final String employeeId;
  final String fullName;
  final String department;
  final String division;
  final String session;
  final String meetingDate;
  final DateTime? attendanceTime;

  factory AttendeeSummary.fromJson(Map<String, dynamic> json) {
    return AttendeeSummary(
      employeeId: json['employeeId'] as String? ?? '',
      fullName: json['fullName'] as String? ?? '',
      department: json['department'] as String? ?? '',
      division: json['division'] as String? ?? '',
      session: json['session'] as String? ?? '',
      meetingDate: json['meetingDate']?.toString() ?? '',
      attendanceTime: _parseDate(json['attendanceTime']),
    );
  }
}

/// Backend DashboardDto.
class DashboardData {
  DashboardData({
    this.totalEmployees = 0,
    this.todayAttendance = 0,
    this.morningAttendance = 0,
    this.afternoonAttendance = 0,
    this.todayAttendees = const [],
  });

  final int totalEmployees;
  final int todayAttendance;
  final int morningAttendance;
  final int afternoonAttendance;
  final List<AttendeeSummary> todayAttendees;

  factory DashboardData.fromJson(Map<String, dynamic> json) {
    return DashboardData(
      totalEmployees: (json['totalEmployees'] as num?)?.toInt() ?? 0,
      todayAttendance: (json['todayAttendance'] as num?)?.toInt() ?? 0,
      morningAttendance: (json['morningAttendance'] as num?)?.toInt() ?? 0,
      afternoonAttendance: (json['afternoonAttendance'] as num?)?.toInt() ?? 0,
      todayAttendees: (json['todayAttendees'] as List?)
              ?.map((e) => AttendeeSummary.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
    );
  }
}

DateTime? _parseDate(Object? value) {
  if (value == null) return null;
  return DateTime.tryParse(value.toString());
}
