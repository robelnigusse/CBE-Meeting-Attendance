import 'package:dio/dio.dart';

import '../core/network/api_exception.dart';
import '../models/dashboard.dart';
import 'base_service.dart';

/// A downloaded report file (bytes + a suggested filename).
class ExportFile {
  ExportFile({required this.bytes, required this.filename});
  final List<int> bytes;
  final String filename;
}

class AdminService extends BaseService {
  /// GET /admin/dashboard
  Future<DashboardData> getDashboard() async {
    final res = await send(() => dio.get('/admin/dashboard'));
    return DashboardData.fromJson((res.data['data'] as Map).cast<String, dynamic>());
  }

  /// GET /admin/export/{csv|excel|pdf} — returns raw file bytes.
  Future<ExportFile> export(String type) async {
    try {
      final res = await dio.get<List<int>>(
        '/admin/export/$type',
        options: Options(responseType: ResponseType.bytes),
      );
      return ExportFile(
        bytes: res.data ?? const [],
        filename: _filenameFor(type),
      );
    } on DioException catch (e) {
      throw toApiException(e);
    }
  }

  String _filenameFor(String type) {
    switch (type) {
      case 'excel':
        return 'CBE_Meeting_Attendance_Report.xlsx';
      case 'pdf':
        return 'CBE_Meeting_Attendance_Report.pdf';
      case 'csv':
      default:
        return 'CBE_Meeting_Attendance_Report.csv';
    }
  }
}
