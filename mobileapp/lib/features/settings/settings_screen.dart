import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../providers/settings_provider.dart';

const Map<String, String> _langNames = {
  'en': 'English',
  'am': 'አማርኛ',
  'or': 'Afaan Oromoo',
  'ti': 'ትግርኛ',
};

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  late final TextEditingController _url;

  @override
  void initState() {
    super.initState();
    _url = TextEditingController(
      text: context.read<SettingsProvider>().baseUrl,
    );
  }

  @override
  void dispose() {
    _url.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    final value = _url.text.trim();
    if (!SettingsProvider.isValid(value)) {
      showSnack(context, 'settings.invalidUrl'.tr(), isError: true);
      return;
    }
    await context.read<SettingsProvider>().setBaseUrl(value);
    if (mounted) showSnack(context, 'settings.saved'.tr());
  }

  @override
  Widget build(BuildContext context) {
    final current = context.locale.languageCode;
    return Scaffold(
      appBar: AppBar(title: Text('settings.title'.tr())),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text('settings.server'.tr(),
                      style: const TextStyle(
                          fontSize: 16, fontWeight: FontWeight.bold)),
                  const SizedBox(height: 12),
                  TextField(
                    controller: _url,
                    keyboardType: TextInputType.url,
                    autocorrect: false,
                    decoration: InputDecoration(
                      labelText: 'settings.serverUrl'.tr(),
                      helperText: 'settings.serverUrlHint'.tr(),
                      helperMaxLines: 2,
                      prefixIcon: const Icon(Icons.dns_outlined),
                    ),
                  ),
                  const SizedBox(height: 12),
                  Align(
                    alignment: Alignment.centerRight,
                    child: FilledButton.icon(
                      onPressed: _save,
                      icon: const Icon(Icons.save_outlined, size: 18),
                      label: Text('settings.save'.tr()),
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          Card(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(16, 8, 16, 8),
                    child: Text('settings.language'.tr(),
                        style: const TextStyle(
                            fontSize: 16, fontWeight: FontWeight.bold)),
                  ),
                  ...context.supportedLocales.map((locale) {
                    final code = locale.languageCode;
                    return RadioListTile<String>(
                      value: code,
                      groupValue: current,
                      activeColor: CbeColors.purple,
                      title: Text(_langNames[code] ?? code),
                      onChanged: (_) => context.setLocale(locale),
                    );
                  }),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
