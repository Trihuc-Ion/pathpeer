import 'package:dio/dio.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:pathpeer_mobile/core/storage/secure_storage.dart';

final dio = Dio(BaseOptions(
  baseUrl: dotenv.env['API_URL'] ?? 'http://10.0.2.2:5164/api',
  connectTimeout: const Duration(seconds: 5),
  receiveTimeout: const Duration(seconds: 5),
));

void setupDioInterceptors() {
  dio.interceptors.add(InterceptorsWrapper(
    onRequest: (options, handler) async {
      final token = await secureStorage.read(key: 'token');
      
      if (token != null) {
        options.headers['Authorization'] = 'Bearer $token';
      }
      
      return handler.next(options);
    },
  ));
}