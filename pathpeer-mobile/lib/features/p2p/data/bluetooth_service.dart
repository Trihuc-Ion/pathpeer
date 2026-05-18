import 'dart:convert';
import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'package:pathpeer_mobile/features/courses/data/course_model.dart';
import 'package:permission_handler/permission_handler.dart';

class BluetoothService {
  static const String _serviceUuid = '12345678-1234-1234-1234-123456789012';
  static const String _characteristicUuid =
      '87654321-4321-4321-4321-210987654321';

  // ── SCANARE ──────────────────────────────

  Future<bool> isAvailable() async {
    return await FlutterBluePlus.isSupported;
  }

  Future<void> startScan() async {
    if (await FlutterBluePlus.isSupported == false) {
      throw Exception('Bluetooth nu este suportat pe acest dispozitiv');
    }
    await FlutterBluePlus.startScan(timeout: const Duration(seconds: 15));
  }

  Future<void> stopScan() async {
    await FlutterBluePlus.stopScan();
  }

  Stream<List<ScanResult>> get scanResults => FlutterBluePlus.scanResults;
  Stream<bool> get isScanning => FlutterBluePlus.isScanning;

  // ── TRIMITERE ──────────────────────────────

  Future<void> sendCourse(BluetoothDevice device, CourseModel course) async {
    await device.connect(timeout: const Duration(seconds: 10));

    try {
      final services = await device.discoverServices();

      final service = services.firstWhere(
        (s) => s.serviceUuid.str128 == _serviceUuid,
        orElse: () => throw Exception(
          'Aplicația PathPeer nu rulează pe celălalt dispozitiv',
        ),
      );

      final characteristic = service.characteristics.firstWhere(
        (c) => c.characteristicUuid.str128 == _characteristicUuid,
        orElse: () =>
            throw Exception('Caracteristica de transfer nu a fost găsită'),
      );

      // Serializezi cursul
      final courseJson = jsonEncode(_courseToMap(course));
      final bytes = utf8.encode(courseJson);

      // Trimiți în chunks — limita BLE e 512 bytes
      const chunkSize = 512;
      for (int i = 0; i < bytes.length; i += chunkSize) {
        final end = (i + chunkSize < bytes.length)
            ? i + chunkSize
            : bytes.length;
        await characteristic.write(
          bytes.sublist(i, end),
          withoutResponse: false,
        );
        // Pauză mică între chunks
        await Future.delayed(const Duration(milliseconds: 50));
      }

      // Semnal de sfârșit
      await characteristic.write(utf8.encode('__END__'));
    } finally {
      await device.disconnect();
    }
  }

  // ── PRIMIRE ──────────────────────────────

  Stream<CourseModel> receiveCourses(BluetoothDevice device) async* {
    await device.connect();
    final services = await device.discoverServices();

    final service = services.firstWhere(
      (s) => s.serviceUuid.str128 == _serviceUuid,
    );

    final characteristic = service.characteristics.firstWhere(
      (c) => c.characteristicUuid.str128 == _characteristicUuid,
    );

    await characteristic.setNotifyValue(true);

    final buffer = <int>[];

    await for (final value in characteristic.onValueReceived) {
      final chunk = utf8.decode(value);

      if (chunk == '__END__') {
        // Am primit tot cursul
        try {
          final jsonStr = utf8.decode(buffer);
          final data = jsonDecode(jsonStr);
          yield _courseFromMap(data);
          buffer.clear();
        } catch (e) {
          buffer.clear();
          throw Exception('Eroare la decodarea cursului: $e');
        }
      } else {
        buffer.addAll(value);
      }
    }
  }

  // ── HELPERS ──────────────────────────────

  Map<String, dynamic> _courseToMap(CourseModel course) => {
    'serverId': course.serverId,
    'title': course.title,
    'description': course.description,
    'language': course.language,
    'price': course.price,
    'status': course.status,
    'creatorUsername': course.creatorUsername,
    'creatorId': course.creatorId,
    'level': course.level,
    'createdAt': course.createdAt.toIso8601String(),
  };

  CourseModel _courseFromMap(Map<String, dynamic> data) => CourseModel()
    ..serverId = data['serverId']
    ..title = data['title']
    ..description = data['description']
    ..language = data['language']
    ..price = (data['price'] as num).toDouble()
    ..status = data['status']
    ..creatorUsername = data['creatorUsername']
    ..creatorId = data['creatorId']
    ..level = data['level']
    ..createdAt = DateTime.parse(data['createdAt'])
    ..isDownloaded = true
    ..downloadedAt = DateTime.now();

  Future<bool> requestPermissions() async {
    final statuses = await [
      Permission.bluetoothScan,
      Permission.bluetoothConnect,
      Permission.bluetoothAdvertise,
      Permission.locationWhenInUse,
    ].request();

    return statuses.values.every((s) => s.isGranted);
  }
}
