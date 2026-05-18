import 'package:flutter/material.dart';
import 'package:flutter_blue_plus/flutter_blue_plus.dart';

class DeviceList extends StatelessWidget {
  final List<ScanResult> devices;
  final bool isScanning;
  final bool isSending;
  final Function(BluetoothDevice) onDeviceSelected;

  const DeviceList({
    super.key,
    required this.devices,
    required this.isScanning,
    required this.isSending,
    required this.onDeviceSelected,
  });

  @override
  Widget build(BuildContext context) {
    if (isScanning && devices.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            CircularProgressIndicator(color: Colors.indigo),
            SizedBox(height: 16),
            Text(
              'Căutare dispozitive...',
              style: TextStyle(color: Colors.white60),
            ),
          ],
        ),
      );
    }

    if (devices.isEmpty) {
      return const Center(
        child: Text(
          'Niciun dispozitiv găsit.\nAsigură-te că Bluetooth-ul este activ.',
          textAlign: TextAlign.center,
          style: TextStyle(color: Colors.white60),
        ),
      );
    }

    return ListView.builder(
      itemCount: devices.length,
      itemBuilder: (context, index) {
        final device = devices[index].device;
        final rssi = devices[index].rssi;

        return Container(
          margin: const EdgeInsets.only(bottom: 8),
          decoration: BoxDecoration(
            color: const Color(0xFF1E293B),
            borderRadius: BorderRadius.circular(12),
          ),
          child: ListTile(
            leading: Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.blue.withOpacity(0.2),
                borderRadius: BorderRadius.circular(8),
              ),
              child: const Icon(Icons.bluetooth, color: Colors.blue),
            ),
            title: Text(
              device.platformName,
              style: const TextStyle(color: Colors.white),
            ),
            subtitle: Text(
              'Semnal: $rssi dBm',
              style: const TextStyle(color: Colors.white54, fontSize: 12),
            ),
            trailing: isSending
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.indigo,
                      foregroundColor: Colors.white,
                    ),
                    onPressed: () => onDeviceSelected(device),
                    child: const Text('Trimite'),
                  ),
          ),
        );
      },
    );
  }
}