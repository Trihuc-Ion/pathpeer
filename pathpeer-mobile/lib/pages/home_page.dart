import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../features/auth/presentation/providers/auth_provider.dart';

class HomePage extends ConsumerWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final user = ref.watch(userProvider); // citește userul din Riverpod
    final authActions = ref.read(authActionsProvider);

    final isAuthenticated = user != null;
    final username = user?['username'] ?? '';
    final role = user?['role'] ?? '';

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
                  "PathPeer",
                  style: TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF38BDF8),
                  ),
                ),

                const SizedBox(height: 20),

                if (isAuthenticated) ...[
                  Text("Salut, $username 👋"),
                  const SizedBox(height: 6),
                  Text(
                    "Rol: $role",
                    style: const TextStyle(fontSize: 13, color: Colors.grey),
                  ),

                  const SizedBox(height: 20),

                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      ElevatedButton(
                        onPressed: () {
                          context.go('/courses'); // navigare către cursuri
                        },
                        child: const Text("Vezi Cursuri"),
                      ),
                      const SizedBox(width: 10),
                      ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.red,
                          foregroundColor: Colors.white,
                        ),
                        onPressed: () async {
                          await authActions.logout(); // logout
                          context.go('/login');
                        },
                        child: const Text("Logout"),
                      ),
                    ],
                  )
                ] else ...[
                  const Text("Bine ai venit pe PathPeer 🚀"),

                  const SizedBox(height: 20),

                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      ElevatedButton(
                        onPressed: () {
                          context.go('/login');
                        },
                        child: const Text("Login"),
                      ),
                      const SizedBox(width: 10),
                      ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.grey,
                        ),
                        onPressed: () {
                          context.go('/register');
                        },
                        child: const Text("Register"),
                      ),
                    ],
                  )
                ]
              ],
            ),
          ),
        ),
      ),
    );
  }
}