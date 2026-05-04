import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/core/network/dio_client.dart';
import 'package:dio/dio.dart';
import 'package:go_router/go_router.dart';

// Provider pentru detaliile unui curs
final courseDetailProvider = FutureProvider.family<Map<String, dynamic>, int>((ref, courseId) async {
  final response = await dio.get('/courses/$courseId');
  return Map<String, dynamic>.from(response.data);
});

class CourseDetailPage extends ConsumerWidget {
  final int courseId;

  const CourseDetailPage({super.key, required this.courseId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final courseAsync = ref.watch(courseDetailProvider(courseId));

    return Scaffold(
      appBar: AppBar(
        title: const Text('Detalii Curs'),
        leading: BackButton(onPressed: () => context.go('/courses')),
      ),
      body: courseAsync.when(
        data: (course) {
          return Padding(
            padding: const EdgeInsets.all(16),
            child: Card(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              elevation: 3,
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(course['title'], style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 12),
                    Text(course['description'] ?? 'Fără descriere'),
                    const SizedBox(height: 20),
                    Text('💰 Preț: ${course['price']} lei'),
                    Text('📌 Status: ${course['status']}'),
                    Text('👤 Creator: ${course['creatorUsername']}'),
                    Text('🌐 Limbă: ${course['language']}'),
                    Text('📊 Nivel: ${course['level']}'),
                    Text('👍 Voturi: ${course['votesUp']} UP / ${course['votesDown']} DOWN'),
                    Text('📅 Creat: ${DateTime.parse(course['createdAt']).toLocal().toString().split(' ')[0]}'),
                  ],
                ),
              ),
            ),
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(child: Text('Eroare: ${err.toString()}')),
      ),
    );
  }
}