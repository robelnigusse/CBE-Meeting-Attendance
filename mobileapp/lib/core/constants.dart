import 'package:flutter/material.dart';

/// App-wide constants: default backend URL, storage keys, roles, locales.
class AppConstants {
  AppConstants._();

  /// Default backend base URL. Overridden at runtime via the Settings screen.
  /// Set to this PC's current Wi-Fi LAN IP; change it in-app (Settings) if the
  /// IP changes. Find it any time with `ipconfig` (IPv4 Address).
  static const String defaultBaseUrl = 'http://192.168.8.118:5157/api';

  // Secure storage keys
  static const String kToken = 'access_token';
  static const String kUser = 'auth_user';

  // Shared preferences keys
  static const String kBaseUrl = 'base_url';
  static const String kLocale = 'locale';
}

/// Role names as defined by the backend (RoleHierarchy: Staff < Admin < SuperAdmin).
class Roles {
  Roles._();
  static const String superAdmin = 'SuperAdmin';
  static const String admin = 'Admin';
  static const String staff = 'Staff';

  static const List<String> assignable = [staff, admin, superAdmin];
}

/// Locales supported by the app (mirrors the web frontend).
const List<Locale> kSupportedLocales = [
  Locale('en'),
  Locale('am'),
  Locale('or'),
  Locale('ti'),
];

const Locale kFallbackLocale = Locale('en');
const String kTranslationsPath = 'assets/translations';
