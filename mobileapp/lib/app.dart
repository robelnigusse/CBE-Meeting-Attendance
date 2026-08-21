import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'core/theme.dart';

/// Root widget: MaterialApp.router wired to the CBE theme and easy_localization.
class CbeApp extends StatelessWidget {
  const CbeApp({super.key, required this.router});

  final GoRouter router;

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'CBE Meeting Attendance',
      debugShowCheckedModeBanner: false,
      theme: buildCbeTheme(),
      routerConfig: router,
      localizationsDelegates: context.localizationDelegates,
      supportedLocales: context.supportedLocales,
      locale: context.locale,
    );
  }
}
