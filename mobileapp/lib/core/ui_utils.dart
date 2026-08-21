import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';

import 'network/api_exception.dart';

/// Resolves an error to a user-facing string.
///
/// [ApiException] messages are either localization keys for synthetic errors
/// (e.g. `common.networkError`) or raw backend messages (e.g.
/// "Attendance already taken."). Calling `.tr()` on a raw message returns it
/// unchanged when no matching key exists, so this handles both.
String errorText(Object error) {
  if (error is ApiException) return error.message.tr();
  return 'common.error'.tr();
}

/// Shows a floating SnackBar, replacing any current one.
void showSnack(BuildContext context, String message, {bool isError = false}) {
  final messenger = ScaffoldMessenger.of(context);
  messenger
    ..hideCurrentSnackBar()
    ..showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: isError ? Colors.red.shade700 : null,
      ),
    );
}
