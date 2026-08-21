import 'package:flutter/material.dart';

/// CBE brand palette (taken from the web frontend's index.css).
class CbeColors {
  CbeColors._();
  static const Color purple = Color(0xFF910096);
  static const Color gold = Color(0xFFE8A029);
  static const Color darkPurple = Color(0xFF6A006E);
  static const Color light = Color(0xFFF9F9F9);
}

/// Builds the Material 3 theme using the CBE palette.
ThemeData buildCbeTheme() {
  final scheme = ColorScheme.fromSeed(
    seedColor: CbeColors.purple,
    primary: CbeColors.purple,
    secondary: CbeColors.gold,
    brightness: Brightness.light,
  );

  return ThemeData(
    useMaterial3: true,
    colorScheme: scheme,
    scaffoldBackgroundColor: CbeColors.light,
    appBarTheme: const AppBarTheme(
      backgroundColor: CbeColors.purple,
      foregroundColor: Colors.white,
      elevation: 0,
      centerTitle: false,
    ),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: Colors.white,
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: Color(0xFFE2E2E2)),
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: Color(0xFFE2E2E2)),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: const BorderSide(color: CbeColors.purple, width: 2),
      ),
    ),
    filledButtonTheme: FilledButtonThemeData(
      style: FilledButton.styleFrom(
        backgroundColor: CbeColors.purple,
        foregroundColor: Colors.white,
        minimumSize: const Size.fromHeight(52),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        textStyle: const TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
      ),
    ),
    cardTheme: CardTheme(
      color: Colors.white,
      elevation: 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: const BorderSide(color: Color(0xFFEDEDED)),
      ),
    ),
    snackBarTheme: const SnackBarThemeData(
      behavior: SnackBarBehavior.floating,
    ),
  );
}
