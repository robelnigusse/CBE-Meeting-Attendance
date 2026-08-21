import 'dart:io';

import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:open_filex/open_filex.dart';
import 'package:path_provider/path_provider.dart';
import 'package:share_plus/share_plus.dart';

import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../models/dashboard.dart';
import '../../services/admin_service.dart';
import '../../widgets/app_loader.dart';
import '../../widgets/empty_state.dart';
import '../../widgets/stat_card.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  final _service = AdminService();
  late Future<DashboardData> _future;
  String? _exportingType;

  @override
  void initState() {
    super.initState();
    _future = _service.getDashboard();
  }

  Future<void> _refresh() async {
    final future = _service.getDashboard();
    setState(() => _future = future);
    await future.catchError((_) => DashboardData());
  }

  Future<void> _export(String type) async {
    if (_exportingType != null) return;
    setState(() => _exportingType = type);
    try {
      final file = await _service.export(type);
      final dir = await getTemporaryDirectory();
      final path = '${dir.path}/${file.filename}';
      await File(path).writeAsBytes(file.bytes, flush: true);
      final result = await OpenFilex.open(path);
      if (result.type != ResultType.done && mounted) {
        await Share.shareXFiles([XFile(path)]);
      }
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    } finally {
      if (mounted) setState(() => _exportingType = null);
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<DashboardData>(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const AppLoader();
        }
        if (snapshot.hasError) {
          return EmptyState(
            icon: Icons.cloud_off,
            message: errorText(snapshot.error!),
            onRetry: _refresh,
            retryLabel: 'common.retry'.tr(),
          );
        }
        final data = snapshot.data ?? DashboardData();
        return RefreshIndicator(
          onRefresh: _refresh,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              _statsGrid(data),
              const SizedBox(height: 20),
              _exportSection(),
              const SizedBox(height: 20),
              Text(
                'dashboard.todayAttendees'.tr(),
                style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              if (data.todayAttendees.isEmpty)
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 24),
                  child: Text(
                    'dashboard.noAttendees'.tr(),
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.grey.shade600),
                  ),
                )
              else
                ...data.todayAttendees.map(_attendeeTile),
            ],
          ),
        );
      },
    );
  }

  Widget _statsGrid(DashboardData d) {
    final cards = [
      StatCard(
        label: 'dashboard.totalEmployees'.tr(),
        value: '${d.totalEmployees}',
        icon: Icons.groups,
        color: CbeColors.purple,
      ),
      StatCard(
        label: 'dashboard.todayAttendance'.tr(),
        value: '${d.todayAttendance}',
        icon: Icons.event_available,
        color: Colors.green,
      ),
      StatCard(
        label: 'dashboard.morning'.tr(),
        value: '${d.morningAttendance}',
        icon: Icons.wb_sunny_outlined,
        color: CbeColors.gold,
      ),
      StatCard(
        label: 'dashboard.afternoon'.tr(),
        value: '${d.afternoonAttendance}',
        icon: Icons.wb_twilight,
        color: Colors.blue,
      ),
    ];
    // Two content-sized rows instead of a fixed-aspect GridView: cards grow to
    // fit their content (icon + value + 1- or 2-line label across all locales),
    // so they never overflow. IntrinsicHeight keeps both cards in a row equal.
    Widget row(StatCard a, StatCard b) => IntrinsicHeight(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(child: a),
              const SizedBox(width: 12),
              Expanded(child: b),
            ],
          ),
        );
    return Column(
      children: [
        row(cards[0], cards[1]),
        const SizedBox(height: 12),
        row(cards[2], cards[3]),
      ],
    );
  }

  Widget _exportSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'dashboard.exportReports'.tr(),
              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 10,
              runSpacing: 10,
              children: [
                _exportButton('csv', 'dashboard.csv'.tr(), Icons.description),
                _exportButton('excel', 'dashboard.excel'.tr(), Icons.table_chart),
                _exportButton('pdf', 'dashboard.pdf'.tr(), Icons.picture_as_pdf),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _exportButton(String type, String label, IconData icon) {
    final busy = _exportingType == type;
    return OutlinedButton.icon(
      onPressed: _exportingType == null ? () => _export(type) : null,
      icon: busy
          ? const SizedBox(
              height: 16,
              width: 16,
              child: CircularProgressIndicator(strokeWidth: 2),
            )
          : Icon(icon, size: 18),
      label: Text(label),
    );
  }

  Widget _attendeeTile(AttendeeSummary a) {
    final subtitleParts = [a.department, a.division].where((s) => s.isNotEmpty);
    final time = a.attendanceTime != null
        ? DateFormat.jm().format(a.attendanceTime!.toLocal())
        : '';
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: CbeColors.purple.withValues(alpha: 0.1),
          child: Text(
            a.fullName.isNotEmpty ? a.fullName[0].toUpperCase() : '?',
            style: const TextStyle(color: CbeColors.purple),
          ),
        ),
        title: Text(a.fullName, style: const TextStyle(fontWeight: FontWeight.w600)),
        subtitle: Text(subtitleParts.join(' · ')),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            if (a.session.isNotEmpty)
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                decoration: BoxDecoration(
                  color: Colors.green.shade50,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  a.session,
                  style: TextStyle(fontSize: 12, color: Colors.green.shade800),
                ),
              ),
            if (time.isNotEmpty)
              Padding(
                padding: const EdgeInsets.only(top: 2),
                child: Text(time,
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade600)),
              ),
          ],
        ),
      ),
    );
  }
}
