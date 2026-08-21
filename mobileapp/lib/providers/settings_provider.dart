import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/constants.dart';
import '../core/network/api_client.dart';

/// Holds the backend base URL (persisted). Locale is handled by
/// easy_localization directly, so it isn't duplicated here.
class SettingsProvider extends ChangeNotifier {
  SettingsProvider(this._prefs)
      : _baseUrl = _prefs.getString(AppConstants.kBaseUrl) ??
            AppConstants.defaultBaseUrl;

  final SharedPreferences _prefs;
  String _baseUrl;

  String get baseUrl => _baseUrl;

  Future<void> setBaseUrl(String url) async {
    final normalized = normalize(url);
    _baseUrl = normalized;
    await _prefs.setString(AppConstants.kBaseUrl, normalized);
    ApiClient.instance.updateBaseUrl(normalized);
    notifyListeners();
  }

  /// Trim and drop a trailing slash so paths like `/api` + `/users/login` join
  /// cleanly.
  static String normalize(String raw) {
    var s = raw.trim();
    while (s.endsWith('/')) {
      s = s.substring(0, s.length - 1);
    }
    return s;
  }

  static bool isValid(String raw) {
    final uri = Uri.tryParse(raw.trim());
    return uri != null &&
        (uri.isScheme('http') || uri.isScheme('https')) &&
        uri.host.isNotEmpty;
  }
}
