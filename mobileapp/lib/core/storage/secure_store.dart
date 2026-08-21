import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../constants.dart';

/// Wraps [FlutterSecureStorage] for the JWT and the serialized current user.
class SecureStore {
  const SecureStore();

  static const _storage = FlutterSecureStorage(
    aOptions: AndroidOptions(encryptedSharedPreferences: true),
  );

  Future<String?> readToken() => _storage.read(key: AppConstants.kToken);
  Future<String?> readUser() => _storage.read(key: AppConstants.kUser);

  Future<void> saveSession({required String token, required String userJson}) async {
    await _storage.write(key: AppConstants.kToken, value: token);
    await _storage.write(key: AppConstants.kUser, value: userJson);
  }

  Future<void> clear() async {
    await _storage.delete(key: AppConstants.kToken);
    await _storage.delete(key: AppConstants.kUser);
  }
}
