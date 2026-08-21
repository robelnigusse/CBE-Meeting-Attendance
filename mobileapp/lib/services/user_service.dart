import '../models/app_user.dart';
import 'base_service.dart';

class UserService extends BaseService {
  /// GET /users (auth) — list of system users.
  Future<List<AppUser>> getAll() async {
    final res = await send(() => dio.get('/users'));
    final list = (res.data['data'] as List?) ?? const [];
    return list
        .map((e) => AppUser.fromJson((e as Map).cast<String, dynamic>()))
        .toList();
  }

  /// POST /users/register (auth) — create an admin user. Returns 200 with
  /// { success:false } on failure (e.g. duplicate email), surfaced by [send].
  Future<String> register({
    required String email,
    required String password,
    String? employeeId,
  }) async {
    final res = await send(() => dio.post('/users/register', data: {
          'email': email,
          'password': password,
          'employeeId': employeeId,
        }));
    return (res.data['message'] as String?) ?? '';
  }

  /// PUT /users/change-role (SuperAdmin). [userId] is the user GUID.
  Future<String> changeRole({
    required String userId,
    required String newRole,
  }) async {
    final res = await send(() => dio.put('/users/change-role', data: {
          'userId': userId,
          'newRole': newRole,
        }));
    return (res.data['message'] as String?) ?? '';
  }
}
