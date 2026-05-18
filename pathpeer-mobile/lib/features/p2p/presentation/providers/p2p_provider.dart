import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pathpeer_mobile/features/courses/data/course_model.dart';
import 'package:pathpeer_mobile/features/p2p/domain/p2p_transfer.dart';

enum P2pStatus { idle, scanning, sending, receiving, success, error }

class P2pState {
  final List<ScanResult> devices;
  final P2pStatus status;
  final String? message;

  const P2pState({
    this.devices = const [],
    this.status = P2pStatus.idle,
    this.message,
  });

  P2pState copyWith({
    List<ScanResult>? devices,
    P2pStatus? status,
    String? message,
  }) => P2pState(
    devices: devices ?? this.devices,
    status: status ?? this.status,
    message: message,
  );
}

class P2pNotifier extends StateNotifier<P2pState> {
  final P2pTransfer _transfer = P2pTransfer();

  P2pNotifier() : super(const P2pState());

  Future<void> startScan() async {
    try {
      final granted = await _transfer.requestPermissions();
      if (!granted) {
        state = state.copyWith(
          status: P2pStatus.error,
          message: 'Permisiunile Bluetooth sunt necesare pentru partajare',
        );
        return;
      }

      state = state.copyWith(
        status: P2pStatus.scanning,
        devices: [],
        message: null,
      );
      await _transfer.startScan();

      _transfer.scanDevices().listen((results) {
        // Filtrezi doar dispozitivele cu nume
        final named = results
            .where((r) => r.device.platformName.isNotEmpty)
            .toList();
        state = state.copyWith(devices: named);
      });
    } catch (e) {
      state = state.copyWith(status: P2pStatus.error, message: e.toString());
    }
  }

  Future<void> stopScan() async {
    await _transfer.stopScan();
    state = state.copyWith(status: P2pStatus.idle);
  }

  Future<void> sendCourse(BluetoothDevice device, CourseModel course) async {
    try {
      state = state.copyWith(status: P2pStatus.sending, message: null);
      await _transfer.shareCourse(device, course);
      state = state.copyWith(
        status: P2pStatus.success,
        message: 'Cursul "${course.title}" a fost trimis cu succes!',
      );
    } catch (e) {
      state = state.copyWith(
        status: P2pStatus.error,
        message: 'Eroare la trimitere: ${e.toString()}',
      );
    }
  }

  void reset() => state = const P2pState();
}

final p2pProvider = StateNotifierProvider<P2pNotifier, P2pState>(
  (ref) => P2pNotifier(),
);
