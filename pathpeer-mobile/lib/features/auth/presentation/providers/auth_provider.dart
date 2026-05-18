import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/features/auth/domain/auth_repository.dart';

enum AuthStatus { initial, loading, authenticated, unauthenticated, error }

class AuthState {
  final AuthStatus status;
  final UserModel? user;
  final String? error;

  const AuthState({
    this.status = AuthStatus.initial,
    this.user,
    this.error,
  });

  AuthState copyWith({
    AuthStatus? status,
    UserModel? user,
    String? error,
  }) => AuthState(
    status: status ?? this.status,
    user: user ?? this.user,
    error: error,
  );
}

class AuthNotifier extends StateNotifier<AuthState> {
  final AuthRepository _repo = AuthRepository();

  AuthNotifier() : super(const AuthState()) {
    checkAuth();
  }

  // Verifică dacă e logat la startup
  Future<void> checkAuth() async {
    final loggedIn = await _repo.isLoggedIn();
    state = state.copyWith(
      status: loggedIn
          ? AuthStatus.authenticated
          : AuthStatus.unauthenticated,
    );
  }

  Future<void> login(String email, String password) async {
    try {
      state = state.copyWith(status: AuthStatus.loading, error: null);
      final user = await _repo.login(email, password);
      state = state.copyWith(status: AuthStatus.authenticated, user: user);
    } catch (e) {
      state = state.copyWith(
        status: AuthStatus.error,
        error: _parseError(e),
      );
    }
  }

  Future<void> register(String email, String password, String username) async {
    try {
      state = state.copyWith(status: AuthStatus.loading, error: null);
      final user = await _repo.register(email, password, username);
      state = state.copyWith(status: AuthStatus.authenticated, user: user);
    } catch (e) {
      state = state.copyWith(
        status: AuthStatus.error,
        error: _parseError(e),
      );
    }
  }

  Future<void> logout() async {
    await _repo.logout();
    state = state.copyWith(status: AuthStatus.unauthenticated, user: null);
  }

  String _parseError(dynamic e) {
    if (e.toString().contains('message')) {
      return e.response?.data['message'] ?? 'Eroare necunoscută';
    }
    return 'Eroare la conectare. Verifică internetul.';
  }
}

final authProvider = StateNotifierProvider<AuthNotifier, AuthState>(
  (ref) => AuthNotifier(),
);