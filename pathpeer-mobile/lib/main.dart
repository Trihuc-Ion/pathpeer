import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';  // Riverpod
import 'package:pathpeer_mobile/core/network/dio_client.dart';
import 'package:pathpeer_mobile/core/storage/local_db.dart';
import 'package:pathpeer_mobile/navigation/app_router.dart';  // GoRouter

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await dotenv.load(fileName: ".env");
  await LocalDb.init();
  // setupDioInterceptors();
  runApp(
    const ProviderScope(  // Riverpod
      child: MyApp(),
    ),
  );
}

class MyApp extends ConsumerWidget  {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {  // Riverpod
    final router = ref.watch(appRouterProvider);
    return MaterialApp.router(
      title: 'PathPeer Mobile',
      routerConfig: router,
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue),
      ),
    );
  }
}