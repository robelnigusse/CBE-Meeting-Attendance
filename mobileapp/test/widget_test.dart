import 'package:flutter_test/flutter_test.dart';
import 'package:mobileapp/core/network/api_exception.dart';
import 'package:mobileapp/providers/settings_provider.dart';

void main() {
  group('SettingsProvider.normalize', () {
    test('trims and strips trailing slashes', () {
      expect(SettingsProvider.normalize('  http://x:5157/api/  '),
          'http://x:5157/api');
      expect(SettingsProvider.normalize('http://x:5157/api'),
          'http://x:5157/api');
    });
  });

  group('SettingsProvider.isValid', () {
    test('accepts http/https URLs with a host', () {
      expect(SettingsProvider.isValid('http://192.168.1.100:5157/api'), isTrue);
      expect(SettingsProvider.isValid('https://api.example.com/api'), isTrue);
    });

    test('rejects malformed or non-http URLs', () {
      expect(SettingsProvider.isValid('not a url'), isFalse);
      expect(SettingsProvider.isValid('ftp://x/api'), isFalse);
      expect(SettingsProvider.isValid('http://'), isFalse);
    });
  });

  group('toApiException', () {
    test('passes through an existing ApiException', () {
      final e = ApiException('boom', statusCode: 400);
      expect(identical(toApiException(e), e), isTrue);
    });

    test('maps unknown errors to a generic key', () {
      expect(toApiException(Exception('x')).message, 'common.error');
    });
  });
}
