/// A system user row from GET /users (CurrentUserDto: id, email, employeeId, roles).
class AppUser {
  AppUser({
    required this.id,
    required this.email,
    this.employeeId,
    this.roles = const [],
  });

  final String id;
  final String email;
  final String? employeeId; // GUID or null
  final List<String> roles;

  String get primaryRole => roles.isNotEmpty ? roles.first : 'Staff';

  factory AppUser.fromJson(Map<String, dynamic> json) {
    return AppUser(
      id: json['id']?.toString() ?? '',
      email: json['email'] as String? ?? '',
      employeeId: json['employeeId']?.toString(),
      roles: (json['roles'] as List?)?.map((e) => e.toString()).toList() ?? const [],
    );
  }
}
