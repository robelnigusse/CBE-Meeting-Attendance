import 'package:go_router/go_router.dart';

import 'features/admin/admin_shell.dart';
import 'features/auth/login_screen.dart';
import 'features/kiosk/kiosk_screen.dart';
import 'features/settings/settings_screen.dart';
import 'providers/auth_provider.dart';

/// Builds the app router. [auth] drives the redirect guard and is used as the
/// refreshListenable so routes re-evaluate on login/logout.
GoRouter buildRouter(AuthProvider auth) {
  return GoRouter(
    initialLocation: '/',
    refreshListenable: auth,
    routes: [
      GoRoute(path: '/', builder: (_, __) => const KioskScreen()),
      GoRoute(path: '/login', builder: (_, __) => const LoginScreen()),
      GoRoute(path: '/admin', builder: (_, __) => const AdminShell()),
      GoRoute(path: '/settings', builder: (_, __) => const SettingsScreen()),
    ],
    redirect: (context, state) {
      if (auth.initializing) return null;
      final location = state.matchedLocation;
      final authed = auth.isAuthenticated;

      // Protect the admin area.
      if (location == '/admin' && !authed) return '/login';
      // Don't show login to an already-authenticated user.
      if (location == '/login' && authed) return '/admin';
      return null;
    },
  );
}
