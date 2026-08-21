import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'app.dart';
import 'core/constants.dart';
import 'core/network/api_client.dart';
import 'providers/auth_provider.dart';
import 'providers/settings_provider.dart';
import 'router.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await EasyLocalization.ensureInitialized();

  final prefs = await SharedPreferences.getInstance();
  final settings = SettingsProvider(prefs);

  // Bring up the shared HTTP client at the saved base URL before anything uses it.
  ApiClient.instance.init(settings.baseUrl);

  final auth = AuthProvider();
  await auth.loadFromStorage();

  final router = buildRouter(auth);

  runApp(
    EasyLocalization(
      supportedLocales: kSupportedLocales,
      path: kTranslationsPath,
      fallbackLocale: kFallbackLocale,
      child: MultiProvider(
        providers: [
          ChangeNotifierProvider<SettingsProvider>.value(value: settings),
          ChangeNotifierProvider<AuthProvider>.value(value: auth),
        ],
        child: CbeApp(router: router),
      ),
    ),
  );
}
