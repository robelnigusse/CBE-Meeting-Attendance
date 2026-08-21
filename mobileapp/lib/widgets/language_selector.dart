import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';

import '../core/theme.dart';

/// Native language names, matching the web LanguageSelector options.
const Map<String, String> _langNames = {
  'en': 'English',
  'am': 'አማርኛ',
  'or': 'Afaan Oromoo',
  'ti': 'ትግርኛ',
};

/// A globe button that opens a menu to switch the app locale via
/// easy_localization (which persists the choice).
class LanguageSelector extends StatelessWidget {
  const LanguageSelector({super.key, this.iconColor});

  final Color? iconColor;

  @override
  Widget build(BuildContext context) {
    final current = context.locale.languageCode;
    return PopupMenuButton<Locale>(
      tooltip: 'Select Language',
      icon: Icon(Icons.language, color: iconColor ?? Colors.white),
      onSelected: (locale) => context.setLocale(locale),
      itemBuilder: (context) => context.supportedLocales.map((locale) {
        final code = locale.languageCode;
        final selected = code == current;
        return PopupMenuItem<Locale>(
          value: locale,
          child: Row(
            children: [
              if (selected)
                const Icon(Icons.check, size: 18, color: CbeColors.purple)
              else
                const SizedBox(width: 18),
              const SizedBox(width: 10),
              Text(
                _langNames[code] ?? code,
                style: TextStyle(
                  fontWeight: selected ? FontWeight.w600 : FontWeight.normal,
                ),
              ),
            ],
          ),
        );
      }).toList(),
    );
  }
}
