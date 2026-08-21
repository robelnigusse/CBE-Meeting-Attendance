import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';

import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../models/employee.dart';
import '../../services/employee_service.dart';
import '../../widgets/app_loader.dart';
import '../../widgets/empty_state.dart';
import 'employee_form_sheet.dart';

class EmployeesScreen extends StatefulWidget {
  const EmployeesScreen({super.key});

  @override
  State<EmployeesScreen> createState() => _EmployeesScreenState();
}

class _EmployeesScreenState extends State<EmployeesScreen> {
  final _service = EmployeeService();
  late Future<List<Employee>> _future;

  @override
  void initState() {
    super.initState();
    _future = _service.getAll();
  }

  Future<void> _reload() async {
    final future = _service.getAll();
    setState(() => _future = future);
    await future.catchError((_) => <Employee>[]);
  }

  Future<void> _add() async {
    final saved = await showEmployeeForm(context);
    if (saved == true) _reload();
  }

  Future<void> _edit(Employee e) async {
    final saved = await showEmployeeForm(context, employee: e);
    if (saved == true) _reload();
  }

  Future<void> _delete(Employee e) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('employees.title'.tr()),
        content: Text('employees.deleteConfirm'.tr()),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: Text('common.cancel'.tr()),
          ),
          FilledButton(
            style: FilledButton.styleFrom(backgroundColor: Colors.red.shade700),
            onPressed: () => Navigator.of(ctx).pop(true),
            child: Text('common.delete'.tr()),
          ),
        ],
      ),
    );
    if (confirmed != true) return;
    try {
      await _service.delete(e.id);
      if (mounted) showSnack(context, 'employees.deleted'.tr());
      _reload();
    } catch (err) {
      if (mounted) showSnack(context, errorText(err), isError: true);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _add,
        backgroundColor: CbeColors.purple,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.add),
        label: Text('employees.addBtn'.tr()),
      ),
      body: FutureBuilder<List<Employee>>(
        future: _future,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const AppLoader();
          }
          if (snapshot.hasError) {
            return EmptyState(
              icon: Icons.cloud_off,
              message: errorText(snapshot.error!),
              onRetry: _reload,
              retryLabel: 'common.retry'.tr(),
            );
          }
          final employees = snapshot.data ?? const [];
          if (employees.isEmpty) {
            return EmptyState(
              icon: Icons.people_outline,
              message: 'employees.noEmp'.tr(),
              onRetry: _reload,
              retryLabel: 'common.retry'.tr(),
            );
          }
          return RefreshIndicator(
            onRefresh: _reload,
            child: ListView.builder(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 96),
              itemCount: employees.length,
              itemBuilder: (context, i) => _tile(employees[i]),
            ),
          );
        },
      ),
    );
  }

  Widget _tile(Employee e) {
    final divDept = [e.division, e.department].where((s) => s.isNotEmpty).join(' / ');
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        onTap: () => _edit(e),
        leading: CircleAvatar(
          backgroundColor: CbeColors.purple.withValues(alpha: 0.1),
          child: Text(
            e.fullName.isNotEmpty ? e.fullName[0].toUpperCase() : '?',
            style: const TextStyle(color: CbeColors.purple),
          ),
        ),
        title: Text(e.fullName, style: const TextStyle(fontWeight: FontWeight.w600)),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(e.employeeId),
            if (divDept.isNotEmpty)
              Text(divDept, style: TextStyle(color: Colors.grey.shade600)),
          ],
        ),
        isThreeLine: divDept.isNotEmpty,
        trailing: IconButton(
          icon: Icon(Icons.delete_outline, color: Colors.red.shade400),
          onPressed: () => _delete(e),
        ),
      ),
    );
  }
}
