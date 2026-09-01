import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:geolocator/geolocator.dart';

import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../services/attendance_service.dart';
import '../../widgets/language_selector.dart';
import '../../widgets/primary_button.dart';

/// Public kiosk: enter an Employee ID to register attendance.
class KioskScreen extends StatefulWidget {
  const KioskScreen({super.key});

  @override
  State<KioskScreen> createState() => _KioskScreenState();
}

class _KioskScreenState extends State<KioskScreen> {
  final _controller = TextEditingController();
  final _service = AttendanceService();
  bool _loading = false;
  String? _message;
  bool _isError = false;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final id = _controller.text.trim();
    if (id.isEmpty || _loading) return;
    FocusScope.of(context).unfocus();
    setState(() {
      _loading = true;
      _message = null;
    });
    try {
      bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        throw Exception('Location services are disabled.');
      }

      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
        if (permission == LocationPermission.denied) {
          throw Exception('Location permissions are denied');
        }
      }
      
      if (permission == LocationPermission.deniedForever) {
        throw Exception('Location permissions are permanently denied.');
      }

      Position position = await Geolocator.getCurrentPosition();

      final result = await _service.takeAttendance(id, position.latitude, position.longitude);
      if (!mounted) return;
      setState(() {
        _isError = false;
        _message = 'kiosk.success'.tr();
      });
      _controller.clear();
      final detail = [result.fullName, result.session]
          .where((s) => s.isNotEmpty)
          .join(' · ');
      if (detail.isNotEmpty) showSnack(context, detail);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isError = true;
        _message = errorText(e);
      });
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('kiosk.title'.tr()),
        actions: [
          const LanguageSelector(),
          IconButton(
            tooltip: 'settings.title'.tr(),
            icon: const Icon(Icons.settings),
            onPressed: () => context.push('/settings'),
          ),
          TextButton(
            onPressed: () => context.push('/login'),
            style: TextButton.styleFrom(foregroundColor: Colors.white),
            child: Text('kiosk.adminPortal'.tr()),
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: Card(
              child: Padding(
                padding: const EdgeInsets.all(28),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Icon(Icons.groups, size: 56, color: CbeColors.purple),
                    const SizedBox(height: 12),
                    Text(
                      'kiosk.title'.tr(),
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                          fontSize: 24, fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'kiosk.subtitle'.tr(),
                      textAlign: TextAlign.center,
                      style: TextStyle(color: Colors.grey.shade600),
                    ),
                    const SizedBox(height: 28),
                    Text(
                      'kiosk.employeeIdLabel'.tr(),
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                    const SizedBox(height: 8),
                    TextField(
                      controller: _controller,
                      textInputAction: TextInputAction.done,
                      textCapitalization: TextCapitalization.characters,
                      decoration: InputDecoration(
                        hintText: 'kiosk.placeholder'.tr(),
                        prefixIcon: const Icon(Icons.badge_outlined),
                      ),
                      onSubmitted: (_) => _submit(),
                    ),
                    const SizedBox(height: 20),
                    PrimaryButton(
                      label: _loading
                          ? 'kiosk.buttonProcessing'.tr()
                          : 'kiosk.button'.tr(),
                      loading: _loading,
                      onPressed: _submit,
                    ),
                    if (_message != null) ...[
                      const SizedBox(height: 20),
                      Container(
                        padding: const EdgeInsets.all(14),
                        decoration: BoxDecoration(
                          color: _isError
                              ? Colors.red.shade50
                              : Colors.green.shade50,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: _isError
                                ? Colors.red.shade200
                                : Colors.green.shade200,
                          ),
                        ),
                        child: Row(
                          children: [
                            Icon(
                              _isError
                                  ? Icons.error_outline
                                  : Icons.check_circle_outline,
                              color: _isError
                                  ? Colors.red.shade700
                                  : Colors.green.shade700,
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              child: Text(
                                _message!,
                                style: TextStyle(
                                  color: _isError
                                      ? Colors.red.shade900
                                      : Colors.green.shade900,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
