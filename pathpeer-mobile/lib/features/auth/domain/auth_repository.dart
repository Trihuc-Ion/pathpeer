import 'package:pathpeer_mobile/features/auth/data/auth_api.dart';
import 'package:pathpeer_mobile/features/auth/data/auth_local.dart';

class UserModel {
  final int id;
  final String email;
  final String username;
  final String role;
  final String teacherStatus;

  const UserModel({
    required this.id,
    required this.email,
    required this.username,
    required this.role,
    required this.teacherStatus,
  });

  factory UserModel.fromMap(Map<String, dynamic> map) => UserModel(
    id: map['id'],
    email: map['email'],
    username: map['username'],
    role: map['role'],
    teacherStatus: map['teacherStatus'],
  );
}

class AuthRepository {
  final AuthApi _api = AuthApi();
  final AuthLocal _local = AuthLocal();

  Future<UserModel> login(String email, String password) async {
    final data = await _api.login(email, password);
    // Mobile → token vine în body
    if (data['token'] != null) {
      await _local.saveToken(data['token']);
    }
    return UserModel.fromMap(data['user'] ?? data);
  }

  Future<UserModel> register(
    String email,
    String password,
    String username,
  ) async {
    final data = await _api.register(email, password, username);
    if (data['token'] != null) {
      await _local.saveToken(data['token']);
    }
    return UserModel.fromMap(data['user'] ?? data);
  }

  Future<void> logout() async {
    await _api.logout();
    await _local.deleteToken();
  }

  Future<bool> isLoggedIn() => _local.hasToken();
}