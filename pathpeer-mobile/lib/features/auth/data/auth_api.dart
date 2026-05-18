import 'package:dio/dio.dart';
import 'package:pathpeer_mobile/core/network/dio_client.dart';

class AuthApi {
  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await dio.post(
      '/auth/login',
      data: {'email': email, 'password': password},
      options: Options(
        headers: {'X-Client-Type': 'mobile'},
      ),
    );
    return response.data;
  }

  Future<Map<String, dynamic>> register(
    String email,
    String password,
    String username,
  ) async {
    final response = await dio.post(
      '/auth/register',
      data: {
        'email': email,
        'password': password,
        'username': username,
      },
      options: Options(
        headers: {'X-Client-Type': 'mobile'},
      ),
    );
    return response.data;
  }

  Future<void> logout() async {
    await dio.post('/auth/logout');
  }

  Future<Map<String, dynamic>> getProfile() async {
    final response = await dio.get('/auth/profile');
    return response.data;
  }
}