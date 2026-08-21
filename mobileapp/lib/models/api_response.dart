/// Mirrors the backend `ApiResponse<T>` envelope: { success, message, data, total }.
class ApiResponse<T> {
  ApiResponse({
    required this.success,
    this.message,
    this.data,
    this.total,
  });

  final bool success;
  final String? message;
  final T? data;
  final int? total;

  factory ApiResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Object? data)? parse,
  ) {
    final raw = json['data'];
    return ApiResponse<T>(
      success: json['success'] == true,
      message: json['message'] as String?,
      data: raw == null ? null : (parse != null ? parse(raw) : raw as T?),
      total: json['total'] as int?,
    );
  }
}
