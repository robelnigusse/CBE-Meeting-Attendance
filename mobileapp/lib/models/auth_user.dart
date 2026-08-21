import '../core/constants.dart';

/// The signed-in user (backend CurrentUserDto).
///
/// NOTE: [employeeId] here is the Employee table GUID (the FK on the user),
/// NOT the human "EMP001" string. Resolve the string via
/// GET /employees/id/{guid} when needed (e.g. for profile images).
class AuthUser {
  AuthUser({
    required this.id,
    required this.email,
    this.employeeId,
    this.employeeName,
    this.division,
    this.department,
    this.jobTitle,
    this.roles = const [],
  });

  final String id;
  final String email;
  final String? employeeId;
  final String? employeeName;
  final String? division;
  final String? department;
  final String? jobTitle;
  final List<String> roles;

  bool get isSuperAdmin => roles.contains(Roles.superAdmin);
  bool get isAdmin => roles.contains(Roles.admin) || isSuperAdmin;

  factory AuthUser.fromJson(Map<String, dynamic> json) {
    return AuthUser(
      id: json['id']?.toString() ?? '',
      email: json['email'] as String? ?? '',
      employeeId: json['employeeId']?.toString(),
      employeeName: json['employeeName'] as String?,
      division: json['division'] as String?,
      department: json['department'] as String?,
      jobTitle: json['jobTitle'] as String?,
      roles: (json['roles'] as List?)?.map((e) => e.toString()).toList() ?? const [],
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'email': email,
        'employeeId': employeeId,
        'employeeName': employeeName,
        'division': division,
        'department': department,
        'jobTitle': jobTitle,
        'roles': roles,
      };
}
