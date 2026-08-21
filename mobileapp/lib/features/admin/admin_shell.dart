import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../widgets/language_selector.dart';
import 'dashboard_screen.dart';
import 'employees_screen.dart';
import 'users_screen.dart';
import '../profile/profile_screen.dart';

class _AdminTab {
  const _AdminTab(this.labelKey, this.icon, this.screen);
  final String labelKey;
  final IconData icon;
  final Widget screen;
}

/// Authenticated area with a role-filtered bottom navigation bar.
class AdminShell extends StatefulWidget {
  const AdminShell({super.key});

  @override
  State<AdminShell> createState() => _AdminShellState();
}

class _AdminShellState extends State<AdminShell> {
  int _index = 0;

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();

    // Mirrors AdminLayout.jsx filteredNav: Employees is SuperAdmin-only,
    // Users is Admin+SuperAdmin. Dashboard and Profile are always available.
    final tabs = <_AdminTab>[
      const _AdminTab('layout.dashboard', Icons.space_dashboard_outlined,
          DashboardScreen()),
      if (auth.isSuperAdmin)
        const _AdminTab('layout.employees', Icons.people_outline,
            EmployeesScreen()),
      if (auth.isAdmin)
        const _AdminTab('layout.users', Icons.admin_panel_settings_outlined,
            UsersScreen()),
      const _AdminTab('nav.profile', Icons.person_outline, ProfileScreen()),
    ];

    final index = _index.clamp(0, tabs.length - 1);

    return Scaffold(
      appBar: AppBar(
        title: Text(tabs[index].labelKey.tr()),
        actions: [
          const LanguageSelector(),
          IconButton(
            tooltip: 'settings.title'.tr(),
            icon: const Icon(Icons.settings),
            onPressed: () => context.push('/settings'),
          ),
          IconButton(
            tooltip: 'layout.logout'.tr(),
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await context.read<AuthProvider>().logout();
              if (context.mounted) context.go('/login');
            },
          ),
          const SizedBox(width: 4),
        ],
      ),
      body: IndexedStack(
        index: index,
        children: tabs.map((t) => t.screen).toList(),
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: index,
        onDestinationSelected: (i) => setState(() => _index = i),
        destinations: tabs
            .map((t) => NavigationDestination(
                  icon: Icon(t.icon),
                  label: t.labelKey.tr(),
                ))
            .toList(),
      ),
    );
  }
}
