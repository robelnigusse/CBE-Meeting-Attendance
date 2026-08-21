import 'dart:io';

import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../../core/ui_utils.dart';
import '../../models/employee.dart';
import '../../services/employee_service.dart';
import '../../widgets/primary_button.dart';

/// Shows the add/edit employee form in a modal bottom sheet.
/// Returns true when a save succeeded.
Future<bool?> showEmployeeForm(BuildContext context, {Employee? employee}) {
  return showModalBottomSheet<bool>(
    context: context,
    isScrollControlled: true,
    useSafeArea: true,
    backgroundColor: Colors.white,
    shape: const RoundedRectangleBorder(
      borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
    ),
    builder: (_) => EmployeeFormSheet(employee: employee),
  );
}

class EmployeeFormSheet extends StatefulWidget {
  const EmployeeFormSheet({super.key, this.employee});

  final Employee? employee;

  @override
  State<EmployeeFormSheet> createState() => _EmployeeFormSheetState();
}

class _EmployeeFormSheetState extends State<EmployeeFormSheet> {
  final _formKey = GlobalKey<FormState>();
  final _service = EmployeeService();

  late final TextEditingController _employeeId;
  late final TextEditingController _fullName;
  late final TextEditingController _division;
  late final TextEditingController _jobTitle;
  late final TextEditingController _department;
  late final TextEditingController _phone;

  String? _imagePath;
  bool _saving = false;

  bool get _isEdit => widget.employee != null;

  @override
  void initState() {
    super.initState();
    final e = widget.employee;
    _employeeId = TextEditingController(text: e?.employeeId ?? '');
    _fullName = TextEditingController(text: e?.fullName ?? '');
    _division = TextEditingController(text: e?.division ?? '');
    _jobTitle = TextEditingController(text: e?.jobTitle ?? '');
    _department = TextEditingController(text: e?.department ?? '');
    _phone = TextEditingController(text: e?.phoneNumber ?? '');
  }

  @override
  void dispose() {
    _employeeId.dispose();
    _fullName.dispose();
    _division.dispose();
    _jobTitle.dispose();
    _department.dispose();
    _phone.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final file = await ImagePicker()
        .pickImage(source: ImageSource.gallery, imageQuality: 80);
    if (file != null) setState(() => _imagePath = file.path);
  }

  Future<void> _save() async {
    if (_saving) return;
    if (!(_formKey.currentState?.validate() ?? false)) return;
    setState(() => _saving = true);
    try {
      if (_isEdit) {
        await _service.update(
          widget.employee!.id,
          fullName: _fullName.text.trim(),
          division: _division.text.trim(),
          jobTitle: _jobTitle.text.trim(),
          department: _department.text.trim(),
          phoneNumber: _phone.text.trim(),
        );
      } else {
        await _service.register(
          employeeId: _employeeId.text.trim(),
          fullName: _fullName.text.trim(),
          division: _division.text.trim(),
          jobTitle: _jobTitle.text.trim(),
          department: _department.text.trim(),
          phoneNumber: _phone.text.trim(),
          imagePath: _imagePath,
        );
      }
      if (!mounted) return;
      showSnack(context, 'employees.saved'.tr());
      Navigator.of(context).pop(true);
    } catch (e) {
      if (mounted) showSnack(context, errorText(e), isError: true);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = MediaQuery.of(context).viewInsets.bottom;
    return Padding(
      padding: EdgeInsets.only(bottom: bottomInset),
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(20, 20, 20, 28),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                _isEdit ? 'employees.editTitle'.tr() : 'employees.addTitle'.tr(),
                style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _employeeId,
                readOnly: _isEdit,
                textCapitalization: TextCapitalization.characters,
                decoration: InputDecoration(
                  labelText: 'employees.empId'.tr(),
                  enabled: !_isEdit,
                ),
                validator: (v) => (v == null || v.trim().isEmpty)
                    ? 'employees.empId'.tr()
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _fullName,
                decoration: InputDecoration(labelText: 'employees.fullName'.tr()),
                validator: (v) => (v == null || v.trim().isEmpty)
                    ? 'employees.fullName'.tr()
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _division,
                decoration: InputDecoration(labelText: 'employees.division'.tr()),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _jobTitle,
                decoration: InputDecoration(labelText: 'employees.jobTitle'.tr()),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _department,
                decoration:
                    InputDecoration(labelText: 'employees.department'.tr()),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _phone,
                keyboardType: TextInputType.phone,
                decoration: InputDecoration(labelText: 'employees.phone'.tr()),
              ),
              if (!_isEdit) ...[
                const SizedBox(height: 16),
                Row(
                  children: [
                    if (_imagePath != null)
                      Padding(
                        padding: const EdgeInsets.only(right: 12),
                        child: ClipRRect(
                          borderRadius: BorderRadius.circular(8),
                          child: Image.file(
                            File(_imagePath!),
                            width: 56,
                            height: 56,
                            fit: BoxFit.cover,
                          ),
                        ),
                      ),
                    OutlinedButton.icon(
                      onPressed: _pickImage,
                      icon: const Icon(Icons.photo_camera_outlined, size: 18),
                      label: Text('employees.photo'.tr()),
                    ),
                  ],
                ),
              ],
              const SizedBox(height: 24),
              PrimaryButton(
                label: 'employees.save'.tr(),
                loading: _saving,
                onPressed: _save,
              ),
              const SizedBox(height: 8),
              TextButton(
                onPressed: _saving ? null : () => Navigator.of(context).pop(false),
                child: Text('employees.cancel'.tr()),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
