import 'dart:convert';

import 'package:flutter/foundation.dart';

import '../core/network/api_client.dart';
import '../core/storage/secure_store.dart';
import '../models/auth_user.dart';
import '../services/auth_service.dart';

/// Owns the authenticated session (token + user), mirrors the web AuthContext.
///
/// Also wires the [ApiClient] token/401 callbacks: every request gets the
/// bearer token, and any 401 clears the session (there is no refresh token).
/// Used as the [GoRouter] refreshListenable so route guards re-evaluate on
/// login/logout.
class AuthProvider extends ChangeNotifier {
  AuthProvider({AuthService? authService, SecureStore store = const SecureStore()})
      : _authService = authService ?? AuthService(),
        _store = store {
    ApiClient.instance.tokenProvider = () => _token;
    ApiClient.instance.onUnauthorized = _handleUnauthorized;
  }

  final AuthService _authService;
  final SecureStore _store;

  AuthUser? _user;
  String? _token;
  bool _initializing = true;

  AuthUser? get user => _user;
  String? get token => _token;
  bool get initializing => _initializing;
  bool get isAuthenticated => _token != null && _user != null;
  bool get isAdmin => _user?.isAdmin ?? false;
  bool get isSuperAdmin => _user?.isSuperAdmin ?? false;

  /// Restore a persisted session on startup (called before runApp finishes).
  Future<void> loadFromStorage() async {
    final token = await _store.readToken();
    final userJson = await _store.readUser();
    if (token != null && token.isNotEmpty && userJson != null) {
      try {
        _user = AuthUser.fromJson(jsonDecode(userJson) as Map<String, dynamic>);
        _token = token;
      } catch (_) {
        _user = null;
        _token = null;
        await _store.clear();
      }
    }
    _initializing = false;
    notifyListeners();
  }

  Future<void> login(String email, String password) async {
    final result = await _authService.login(email, password);
    _token = result.token;
    _user = result.user;
    await _store.saveSession(
      token: result.token,
      userJson: jsonEncode(result.user.toJson()),
    );
    notifyListeners();
  }

  Future<void> logout() async {
    if (_token == null && _user == null) return;
    _token = null;
    _user = null;
    await _store.clear();
    notifyListeners();
  }

  /// Fired from the Dio error interceptor on any 401. Fire-and-forget logout.
  void _handleUnauthorized() {
    if (_token == null) return;
    logout();
  }

  @override
  void dispose() {
    ApiClient.instance.tokenProvider = null;
    ApiClient.instance.onUnauthorized = null;
    super.dispose();
  }
}
