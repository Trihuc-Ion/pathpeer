import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/core/network/dio_client.dart';
import 'package:go_router/go_router.dart';
import 'package:dio/dio.dart';

// Provider pentru lista de cursuri
final coursesProvider = FutureProvider<List<Map<String, dynamic>>>((ref) async {
  final response = await dio.get('/courses');
  return List<Map<String, dynamic>>.from(response.data);
});

class CoursesPage extends ConsumerWidget {
  const CoursesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final coursesAsync = ref.watch(coursesProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Cursuri'),
        leading: BackButton(onPressed: () => context.go('/')),
      ),
      body: coursesAsync.when(
        data: (courses) {
          if (courses.isEmpty) {
            return const Center(child: Text('Nu există cursuri încă.'));
          }

          return ListView.separated(
            padding: const EdgeInsets.all(16),
            itemCount: courses.length,
            separatorBuilder: (_, __) => const SizedBox(height: 12),
            itemBuilder: (context, index) {
              final course = courses[index];
              return ListTile(
                tileColor: Colors.blue[50],
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                title: Text(course['title'], style: const TextStyle(fontWeight: FontWeight.bold)),
                subtitle: Text('Creator: ${course['creatorUsername']} • Status: ${course['status']}'),
                trailing: Text('${course['price']} lei'),
                onTap: () => context.go('/courses/${course['id']}'),
              );
            },
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(child: Text('Eroare: ${err.toString()}')),
      ),
    );
  }
}