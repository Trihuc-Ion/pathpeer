import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/core/storage/secure_storage.dart';

// Stochează tokenul
final tokenProvider = StateProvider<String?>((ref) => null);

// Stochează userul — deocamdată null, îl completăm când facem auth feature
final userProvider = StateProvider<Map<String, dynamic>?>((ref) => null);

// Funcții helper
final authActionsProvider = Provider((ref) => AuthActions(ref));

class AuthActions {
  final Ref _ref;
  AuthActions(this._ref);

  Future<void> saveToken(String token) async {
    await secureStorage.write(key: 'token', value: token);
    _ref.read(tokenProvider.notifier).state = token;
  }

  Future<void> logout() async {
    await secureStorage.delete(key: 'token');
    _ref.read(tokenProvider.notifier).state = null;
    _ref.read(userProvider.notifier).state = null;
  }

  Future<void> loadToken() async {
    final token = await secureStorage.read(key: 'token');
    if (token != null) {
      _ref.read(tokenProvider.notifier).state = token;
    }
  }
}