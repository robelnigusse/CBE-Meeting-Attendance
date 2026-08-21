/// Backend EmployeeResponseDto.
class Employee {
  Employee({
    required this.id,
    required this.employeeId,
    required this.fullName,
    this.division = '',
    this.jobTitle = '',
    this.department = '',
    this.phoneNumber = '',
  });

  final String id; // GUID
  final String employeeId; // e.g. EMP001
  final String fullName;
  final String division;
  final String jobTitle;
  final String department;
  final String phoneNumber;

  factory Employee.fromJson(Map<String, dynamic> json) {
    return Employee(
      id: json['id']?.toString() ?? '',
      employeeId: json['employeeId'] as String? ?? '',
      fullName: json['fullName'] as String? ?? '',
      division: json['division'] as String? ?? '',
      jobTitle: json['jobTitle'] as String? ?? '',
      department: json['department'] as String? ?? '',
      phoneNumber: json['phoneNumber'] as String? ?? '',
    );
  }
}
