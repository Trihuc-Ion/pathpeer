import 'package:pathpeer_mobile/core/storage/secure_storage.dart';

class AuthLocal {
  static const String _tokenKey = 'token';
  static const String _userKey = 'user';

  Future<void> saveToken(String token) async {
    await secureStorage.write(key: _tokenKey, value: token);
  }

  Future<String?> getToken() async {
    return await secureStorage.read(key: _tokenKey);
  }

  Future<void> deleteToken() async {
    await secureStorage.delete(key: _tokenKey);
  }

  Future<bool> hasToken() async {
    final token = await getToken();
    return token != null;
  }
}