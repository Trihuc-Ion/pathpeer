import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/features/auth/presentation/pages/login_page.dart';
import 'package:pathpeer_mobile/features/auth/presentation/pages/register_page.dart';
import 'package:pathpeer_mobile/features/auth/presentation/providers/auth_provider.dart';
import 'package:pathpeer_mobile/features/p2p/presentation/pages/p2p_home_page.dart';
import 'package:pathpeer_mobile/navigation/routes.dart';
import 'package:pathpeer_mobile/pages/home_page.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  final authState = ref.watch(authProvider);

  return GoRouter(
    initialLocation: Routes.home,
    redirect: (context, state) {
      final status = authState.status;
      final isLoginRoute = state.matchedLocation == Routes.login;
      final isRegisterRoute = state.matchedLocation == Routes.register;
      final isGuestRoute = isLoginRoute || isRegisterRoute;

      if (status == AuthStatus.initial) return Routes.splash;

      if (status == AuthStatus.unauthenticated && !isGuestRoute) {
        return Routes.login;
      }

      if (status == AuthStatus.authenticated && isGuestRoute) {
        return Routes.home;
      }

      return null; // niciun redirect
    },
    routes: [
      GoRoute(
        path: Routes.splash,
        builder: (context, state) => const SplashPage(),
      ),
      GoRoute(
        path: Routes.home,
        builder: (context, state) => const HomePage(),
      ),
      GoRoute(
        path: Routes.login,
        builder: (context, state) => const LoginPage(),
      ),
      GoRoute(
        path: Routes.register,
        builder: (context, state) => const RegisterPage(),
      ),
      GoRoute(
        path: Routes.p2p,
        builder: (context, state) => const P2pHomePage(),
      ),
    ],
  );
});

// Splash simplu — apare doar o fracțiune de secundă
class SplashPage extends StatelessWidget {
  const SplashPage({super.key});

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      backgroundColor: Color(0xFF0F172A),
      body: Center(
        child: CircularProgressIndicator(color: Colors.indigo),
      ),
    );
  }
}