import 'package:flutter_blue_plus/flutter_blue_plus.dart' hide BluetoothService;
import 'package:pathpeer_mobile/features/courses/data/course_model.dart';
import 'package:pathpeer_mobile/features/courses/data/courses_local.dart';
import 'package:pathpeer_mobile/features/p2p/data/bluetooth_service.dart';

class P2pTransfer {
  final BluetoothService _bluetooth;
  final CoursesLocal _local;

  P2pTransfer({
    BluetoothService? bluetooth,
    CoursesLocal? local,
  })  : _bluetooth = bluetooth ?? BluetoothService(),
        _local = local ?? CoursesLocal();

  Future<bool> requestPermissions() => _bluetooth.requestPermissions();

  Stream<List<ScanResult>> scanDevices() => _bluetooth.scanResults;
  Stream<bool> get isScanning => _bluetooth.isScanning;

  Future<void> startScan() => _bluetooth.startScan();
  Future<void> stopScan() => _bluetooth.stopScan();

  Future<void> shareCourse(BluetoothDevice device, CourseModel course) =>
      _bluetooth.sendCourse(device, course);

  Future<void> receiveAndSaveCourse(BluetoothDevice device) async {
    await for (final course in _bluetooth.receiveCourses(device)) {
      await _local.saveCourse(course);
    }
  }
}