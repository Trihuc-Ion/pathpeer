import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:pathpeer_mobile/core/network/dio_client.dart';
import '../providers/auth_provider.dart'; // client HTTP (Dio)

class LoginPage extends ConsumerStatefulWidget {
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  final emailController = TextEditingController();
  final passwordController = TextEditingController();

  bool loading = false;
  String? error;

  Future<void> handleLogin() async {
    setState(() {
      loading = true;
      error = null;
    });

    try {
      final response = await dio.post(
        '/auth/login',
        data: {
          'email': emailController.text,
          'password': passwordController.text,
        },
      );

      final user = response.data['user'];
      final token = response.data['token'];

      // Salvează token și user în Riverpod
      await ref.read(authActionsProvider).saveToken(token);
      ref.read(userProvider.notifier).state = user;

      // Navighează către home
      context.go('/');
    } catch (e) {
      setState(() {
        error = "Email sau parolă incorectă";
      });
    } finally {
      setState(() {
        loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Card(
          margin: const EdgeInsets.all(20),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          child: Padding(
            padding: const EdgeInsets.all(30),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Text(
                  "Login PathPeer",
                  style: TextStyle(
                    fontSize: 26,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF38BDF8),
                  ),
                ),

                const SizedBox(height: 20),

                if (error != null)
                  Text(error!, style: const TextStyle(color: Colors.red)),

                TextField(
                  controller: emailController,
                  decoration: const InputDecoration(
                    labelText: "Email",
                    border: OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 12),

                TextField(
                  controller: passwordController,
                  obscureText: true,
                  decoration: const InputDecoration(
                    labelText: "Parolă",
                    border: OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 20),

                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: loading ? null : handleLogin,
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                    child: loading
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              color: Colors.white,
                              strokeWidth: 2,
                            ),
                          )
                        : const Text("Login"),
                  ),
                ),

                const SizedBox(height: 12),
                TextButton(
                  onPressed: () => context.go('/register'),
                  child: const Text("Nu ai cont? Înregistrează-te"),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
