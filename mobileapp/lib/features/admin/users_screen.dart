import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants.dart';
import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../models/app_user.dart';
import '../../providers/auth_provider.dart';
import '../../services/user_service.dart';
import '../../widgets/app_loader.dart';
import '../../widgets/empty_state.dart';
import '../../widgets/primary_button.dart';

class UsersScreen extends StatefulWidget {
  const UsersScreen({super.key});

  @override
  State<UsersScreen> createState() => _UsersScreenState();
}

class _UsersScreenState extends State<UsersScreen> {
  final _service = UserService();
  late Future<List<AppUser>> _future;

  @override
  void initState() {
    super.initState();
    _future = _service.getAll();
  }

  Future<void> _reload() async {
    final future = _service.getAll();
    setState(() => _future = future);
    await future.catchError((_) => <AppUser>[]);
  }

  Future<void> _add() async {
    final saved = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      useSafeArea: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (_) => const _AddUserSheet(),
    );
    if (saved == true) _reload();
  }

  Future<void> _changeRole(AppUser u, String newRole) async {
    if (newRole == u.primaryRole) return;
    try {
      await _service.changeRole(userId: u.id, newRole: newRole);
      if (mounted) showSnack(context, 'users.roleUpdated'.tr());
      _reload();
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    return Scaffold(
      backgroundColor: Colors.transparent,
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _add,
        backgroundColor: CbeColors.purple,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.person_add_alt),
        label: Text('users.addBtn'.tr()),
      ),
      body: FutureBuilder<List<AppUser>>(
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
          final users = snapshot.data ?? const [];
          if (users.isEmpty) {
            return EmptyState(
              icon: Icons.admin_panel_settings_outlined,
              message: 'users.subtitle'.tr(),
              onRetry: _reload,
              retryLabel: 'common.retry'.tr(),
            );
          }
          return RefreshIndicator(
            onRefresh: _reload,
            child: ListView.builder(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 96),
              itemCount: users.length,
              itemBuilder: (context, i) => _tile(users[i], auth),
            ),
          );
        },
      ),
    );
  }

  Widget _tile(AppUser u, AuthProvider auth) {
    final isSelf = auth.user?.email == u.email;
    final canChangeRole = auth.isSuperAdmin && !isSelf;
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    u.email,
                    style: const TextStyle(fontWeight: FontWeight.w600),
                  ),
                ),
                if (isSelf)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: Colors.blue.shade50,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text('users.you'.tr(),
                        style: TextStyle(fontSize: 12, color: Colors.blue.shade800)),
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 6,
              runSpacing: 4,
              children: u.roles
                  .map((r) => Chip(
                        label: Text(r, style: const TextStyle(fontSize: 12)),
                        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        visualDensity: VisualDensity.compact,
                        backgroundColor: Colors.grey.shade100,
                        side: BorderSide(color: Colors.grey.shade300),
                      ))
                  .toList(),
            ),
            const SizedBox(height: 6),
            Text(
              '${'users.tableLinkedId'.tr()}: ${u.employeeId ?? '-'}',
              style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
            ),
            if (canChangeRole) ...[
              const SizedBox(height: 8),
              Row(
                children: [
                  Text('${'users.tableRoles'.tr()}: ',
                      style: const TextStyle(fontSize: 13)),
                  const SizedBox(width: 8),
                  DropdownButton<String>(
                    value: Roles.assignable.contains(u.primaryRole)
                        ? u.primaryRole
                        : Roles.staff,
                    underline: const SizedBox.shrink(),
                    items: Roles.assignable
                        .map((r) => DropdownMenuItem(value: r, child: Text(r)))
                        .toList(),
                    onChanged: (r) {
                      if (r != null) _changeRole(u, r);
                    },
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}

/// Register-a-new-admin form (email/password/linked employee id).
class _AddUserSheet extends StatefulWidget {
  const _AddUserSheet();

  @override
  State<_AddUserSheet> createState() => _AddUserSheetState();
}

class _AddUserSheetState extends State<_AddUserSheet> {
  final _formKey = GlobalKey<FormState>();
  final _service = UserService();
  final _email = TextEditingController();
  final _password = TextEditingController();
  final _employeeId = TextEditingController();
  bool _saving = false;
  bool _obscure = true;

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    _employeeId.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_saving) return;
    if (!(_formKey.currentState?.validate() ?? false)) return;
    setState(() => _saving = true);
    try {
      await _service.register(
        email: _email.text.trim(),
        password: _password.text,
        employeeId: _employeeId.text.trim().isEmpty
            ? null
            : _employeeId.text.trim(),
      );
      if (!mounted) return;
      showSnack(context, 'users.registered'.tr());
      Navigator.of(context).pop(true);
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(20, 20, 20, 28),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('users.addTitle'.tr(),
                  style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
              const SizedBox(height: 16),
              TextFormField(
                controller: _email,
                keyboardType: TextInputType.emailAddress,
                decoration: InputDecoration(labelText: 'users.email'.tr()),
                validator: (v) =>
                    (v == null || v.trim().isEmpty) ? 'users.email'.tr() : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _password,
                obscureText: _obscure,
                decoration: InputDecoration(
                  labelText: 'users.password'.tr(),
                  helperText: 'users.passwordHint'.tr(),
                  helperMaxLines: 2,
                  suffixIcon: IconButton(
                    icon: Icon(_obscure
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined),
                    onPressed: () => setState(() => _obscure = !_obscure),
                  ),
                ),
                validator: (v) => (v == null || v.isEmpty)
                    ? 'users.password'.tr()
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _employeeId,
                textCapitalization: TextCapitalization.characters,
                decoration: InputDecoration(
                  labelText: 'users.linkedId'.tr(),
                  helperText: 'users.linkedIdHint'.tr(),
                  helperMaxLines: 2,
                ),
              ),
              const SizedBox(height: 20),
              PrimaryButton(
                label: 'users.register'.tr(),
                loading: _saving,
                onPressed: _submit,
              ),
              const SizedBox(height: 8),
              TextButton(
                onPressed: _saving ? null : () => Navigator.of(context).pop(false),
                child: Text('users.cancel'.tr()),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
