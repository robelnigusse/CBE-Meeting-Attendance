import 'dart:typed_data';

import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';

import '../../core/theme.dart';
import '../../core/ui_utils.dart';
import '../../providers/auth_provider.dart';
import '../../services/auth_service.dart';
import '../../services/employee_service.dart';
import '../../services/profile_service.dart';
import '../../widgets/primary_button.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  final _employeeService = EmployeeService();
  final _profileService = ProfileService();
  final _authService = AuthService();

  final _oldPassword = TextEditingController();
  final _newPassword = TextEditingController();
  final _confirmPassword = TextEditingController();
  final _passwordFormKey = GlobalKey<FormState>();

  String? _stringEmployeeId;
  Uint8List? _imageBytes;
  bool _loadingProfile = true;
  bool _uploading = false;
  bool _changingPassword = false;
  bool _obscureOld = true;
  bool _obscureNew = true;

  @override
  void initState() {
    super.initState();
    _init();
  }

  @override
  void dispose() {
    _oldPassword.dispose();
    _newPassword.dispose();
    _confirmPassword.dispose();
    super.dispose();
  }

  Future<void> _init() async {
    final guid = context.read<AuthProvider>().user?.employeeId;
    if (guid != null && guid.isNotEmpty) {
      final employee = await _employeeService.getByGuid(guid);
      _stringEmployeeId = employee?.employeeId;
      if (_stringEmployeeId != null) {
        final bytes = await _profileService.getImage(_stringEmployeeId!);
        if (bytes != null) _imageBytes = Uint8List.fromList(bytes);
      }
    }
    if (mounted) setState(() => _loadingProfile = false);
  }

  Future<void> _uploadPhoto() async {
    if (_stringEmployeeId == null || _uploading) return;
    final file = await ImagePicker()
        .pickImage(source: ImageSource.gallery, imageQuality: 80);
    if (file == null) return;
    setState(() => _uploading = true);
    try {
      await _profileService.upload(
        employeeId: _stringEmployeeId!,
        imagePath: file.path,
      );
      final bytes = await _profileService.getImage(_stringEmployeeId!);
      if (mounted) {
        setState(() {
          if (bytes != null) _imageBytes = Uint8List.fromList(bytes);
        });
        showSnack(context, 'profile.photoUpdated'.tr());
      }
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    } finally {
      if (mounted) setState(() => _uploading = false);
    }
  }

  Future<void> _changePassword() async {
    if (_changingPassword) return;
    if (!(_passwordFormKey.currentState?.validate() ?? false)) return;
    if (_newPassword.text != _confirmPassword.text) {
      showSnack(context, 'profile.passwordMismatch'.tr(), isError: true);
      return;
    }
    setState(() => _changingPassword = true);
    try {
      await _authService.changePassword(
        oldPassword: _oldPassword.text,
        newPassword: _newPassword.text,
      );
      if (!mounted) return;
      _oldPassword.clear();
      _newPassword.clear();
      _confirmPassword.clear();
      showSnack(context, 'profile.passwordChanged'.tr());
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    } finally {
      if (mounted) setState(() => _changingPassword = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = context.watch<AuthProvider>().user;
    if (user == null) return const SizedBox.shrink();

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _header(user.employeeName ?? user.email),
        const SizedBox(height: 20),
        _accountCard(user.email, user.roles),
        const SizedBox(height: 20),
        _passwordCard(),
      ],
    );
  }

  Widget _header(String name) {
    return Center(
      child: Column(
        children: [
          Stack(
            children: [
              CircleAvatar(
                radius: 48,
                backgroundColor: CbeColors.purple.withValues(alpha: 0.1),
                backgroundImage:
                    _imageBytes != null ? MemoryImage(_imageBytes!) : null,
                child: _imageBytes == null
                    ? (_loadingProfile
                        ? const CircularProgressIndicator()
                        : Text(
                            name.isNotEmpty ? name[0].toUpperCase() : '?',
                            style: const TextStyle(
                                fontSize: 32, color: CbeColors.purple),
                          ))
                    : null,
              ),
              if (_stringEmployeeId != null)
                Positioned(
                  bottom: 0,
                  right: 0,
                  child: Material(
                    color: CbeColors.gold,
                    shape: const CircleBorder(),
                    child: InkWell(
                      customBorder: const CircleBorder(),
                      onTap: _uploading ? null : _uploadPhoto,
                      child: Padding(
                        padding: const EdgeInsets.all(8),
                        child: _uploading
                            ? const SizedBox(
                                height: 16,
                                width: 16,
                                child: CircularProgressIndicator(
                                    strokeWidth: 2, color: Colors.white),
                              )
                            : const Icon(Icons.camera_alt,
                                size: 16, color: Colors.white),
                      ),
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 12),
          Text(name,
              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
          if (_stringEmployeeId == null && !_loadingProfile)
            Padding(
              padding: const EdgeInsets.only(top: 4),
              child: Text('profile.noEmployeeLinked'.tr(),
                  style: TextStyle(color: Colors.grey.shade600, fontSize: 13)),
            ),
        ],
      ),
    );
  }

  Widget _accountCard(String email, List<String> roles) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('profile.account'.tr(),
                style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            _infoRow(Icons.mail_outline, email),
            const SizedBox(height: 8),
            _infoRow(
              Icons.verified_user_outlined,
              roles.isEmpty ? '-' : roles.join(', '),
            ),
            const SizedBox(height: 8),
            _infoRow(Icons.badge_outlined, _stringEmployeeId ?? '-'),
          ],
        ),
      ),
    );
  }

  Widget _infoRow(IconData icon, String value) {
    return Row(
      children: [
        Icon(icon, size: 18, color: Colors.grey.shade600),
        const SizedBox(width: 10),
        Expanded(child: Text(value)),
      ],
    );
  }

  Widget _passwordCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _passwordFormKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('profile.changePassword'.tr(),
                  style:
                      const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
              const SizedBox(height: 12),
              TextFormField(
                controller: _oldPassword,
                obscureText: _obscureOld,
                decoration: InputDecoration(
                  labelText: 'profile.oldPassword'.tr(),
                  suffixIcon: IconButton(
                    icon: Icon(_obscureOld
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined),
                    onPressed: () => setState(() => _obscureOld = !_obscureOld),
                  ),
                ),
                validator: (v) => (v == null || v.isEmpty)
                    ? 'profile.oldPassword'.tr()
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _newPassword,
                obscureText: _obscureNew,
                decoration: InputDecoration(
                  labelText: 'profile.newPassword'.tr(),
                  suffixIcon: IconButton(
                    icon: Icon(_obscureNew
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined),
                    onPressed: () => setState(() => _obscureNew = !_obscureNew),
                  ),
                ),
                validator: (v) => (v == null || v.isEmpty)
                    ? 'profile.newPassword'.tr()
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _confirmPassword,
                obscureText: _obscureNew,
                decoration:
                    InputDecoration(labelText: 'profile.confirmPassword'.tr()),
                validator: (v) => (v == null || v.isEmpty)
                    ? 'profile.confirmPassword'.tr()
                    : null,
              ),
              const SizedBox(height: 16),
              PrimaryButton(
                label: 'profile.changePassword'.tr(),
                loading: _changingPassword,
                onPressed: _changePassword,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
