import 'package:flutter/material.dart';
import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/features/courses/data/course_model.dart';
import 'package:pathpeer_mobile/features/p2p/presentation/providers/p2p_provider.dart';
import 'package:pathpeer_mobile/features/p2p/presentation/widgets/device_list.dart';
import 'package:pathpeer_mobile/features/p2p/presentation/widgets/transfer_progress.dart';

class P2pSharePage extends ConsumerStatefulWidget {
  final CourseModel course;

  const P2pSharePage({super.key, required this.course});

  @override
  ConsumerState<P2pSharePage> createState() => _P2pSharePageState();
}

class _P2pSharePageState extends ConsumerState<P2pSharePage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(p2pProvider.notifier).startScan();
    });
  }

  @override
  void dispose() {
    ref.read(p2pProvider.notifier).stopScan();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(p2pProvider);

    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: const Text(
          'Partajează curs',
          style: TextStyle(color: Colors.white),
        ),
        iconTheme: const IconThemeData(color: Colors.white),
        actions: [
          if (state.status == P2pStatus.scanning)
            IconButton(
              icon: const Icon(Icons.refresh),
              onPressed: () => ref.read(p2pProvider.notifier).startScan(),
            ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [

            // Cursul de trimis
            _CourseCard(course: widget.course),
            const SizedBox(height: 24),

            // Status trimitere
            TransferProgress(status: state.status, message: state.message),
            const SizedBox(height : 16),

            // Lista dispozitive
            const Text(
              'Dispozitive disponibile',
              style: TextStyle(
                color: Colors.white,
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),

            Expanded(
              child: DeviceList(
                devices: state.devices,
                isScanning: state.status == P2pStatus.scanning,
                isSending: state.status == P2pStatus.sending,
                onDeviceSelected: (device) => ref
                    .read(p2pProvider.notifier)
                    .sendCourse(device, widget.course),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _CourseCard extends StatelessWidget {
  final CourseModel course;
  const _CourseCard({required this.course});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFF1E293B),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.indigo.withOpacity(0.4)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.indigo.withOpacity(0.2),
              borderRadius: BorderRadius.circular(8),
            ),
            child: const Icon(Icons.book, color: Colors.indigo),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  course.title,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  course.creatorUsername,
                  style: TextStyle(color: Colors.white.withOpacity(0.5), fontSize: 12),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}