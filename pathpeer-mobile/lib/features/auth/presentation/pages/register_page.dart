import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/core/network/dio_client.dart';
import 'package:go_router/go_router.dart';
import 'package:dio/dio.dart';

import '../providers/auth_provider.dart';

class RegisterPage extends ConsumerStatefulWidget {
  const RegisterPage({super.key});

  @override
  ConsumerState<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends ConsumerState<RegisterPage> {
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  final usernameController = TextEditingController();

  bool loading = false;
  String? error;

  Future<void> handleRegister() async {
    setState(() {
      loading = true;
      error = null;
    });

    try {
      final response = await dio.post('/auth/register', data: {
        'email': emailController.text.trim(),
        'password': passwordController.text,
        'username': usernameController.text.trim(),
      });

      final token = response.data['token'] as String;
      final user = response.data['user'] as Map<String, dynamic>;

      // Salvează token și user în Riverpod
      await ref.read(authActionsProvider).saveToken(token);
      ref.read(userProvider.notifier).state = user;

      // Duce utilizatorul pe Home
      if (mounted) context.go('/');
    } on DioError catch (e) {
      setState(() {
        error = e.response?.data['message'] ?? 'Eroare la înregistrare';
      });
    } catch (e) {
      setState(() {
        error = 'Eroare necunoscută';
      });
    } finally {
      setState(() => loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Card(
          margin: const EdgeInsets.all(20),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Text("Înregistrează-te PathPeer", style: TextStyle(fontSize: 22)),
                const SizedBox(height: 20),

                if (error != null)
                  Text(error!, style: const TextStyle(color: Colors.red)),
                
                const SizedBox(height: 10),

                TextField(
                  controller: usernameController,
                  decoration: const InputDecoration(labelText: "Username"),
                ),
                const SizedBox(height: 10),

                TextField(
                  controller: emailController,
                  decoration: const InputDecoration(labelText: "Email"),
                ),
                const SizedBox(height: 10),

                TextField(
                  controller: passwordController,
                  obscureText: true,
                  decoration: const InputDecoration(labelText: "Parolă"),
                ),
                const SizedBox(height: 20),

                ElevatedButton(
                  onPressed: loading ? null : handleRegister,
                  child: loading
                      ? const CircularProgressIndicator(color: Colors.white)
                      : const Text("Înregistrează-te"),
                ),

                const SizedBox(height: 10),
                TextButton(
                  onPressed: () => context.go('/login'),
                  child: const Text("Ai deja cont? Login"),
                )
              ],
            ),
          ),
        ),
      ),
    );
  }
}