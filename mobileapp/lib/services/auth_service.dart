import '../models/auth_user.dart';
import 'base_service.dart';

/// Parsed result of a successful login.
class LoginResult {
  LoginResult({required this.token, this.expiration, required this.user});

  final String token;
  final DateTime? expiration;
  final AuthUser user;
}

class AuthService extends BaseService {
  /// POST /users/login — returns 200 with { success:false } on bad credentials,
  /// so [send] turns that into an [ApiException] carrying the backend message.
  Future<LoginResult> login(String email, String password) async {
    final res = await send(() => dio.post(
          '/users/login',
          data: {'email': email, 'password': password},
        ));
    final data = (res.data['data'] as Map).cast<String, dynamic>();
    return LoginResult(
      token: data['token'] as String,
      expiration: DateTime.tryParse(data['expiration']?.toString() ?? ''),
      user: AuthUser.fromJson((data['user'] as Map).cast<String, dynamic>()),
    );
  }

  /// POST /users/change-password (auth). Returns the backend message.
  Future<String> changePassword({
    required String oldPassword,
    required String newPassword,
  }) async {
    final res = await send(() => dio.post(
          '/users/change-password',
          data: {'oldPassword': oldPassword, 'newPassword': newPassword},
        ));
    return (res.data['message'] as String?) ?? '';
  }
}
