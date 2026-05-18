import 'package:flutter/material.dart';
import 'package:pathpeer_mobile/features/p2p/presentation/providers/p2p_provider.dart';

class TransferProgress extends StatelessWidget {
  final P2pStatus status;
  final String? message;

  const TransferProgress({
    super.key,
    required this.status,
    this.message,
  });

  @override
  Widget build(BuildContext context) {
    if (status == P2pStatus.idle || status == P2pStatus.scanning) {
      return const SizedBox.shrink();
    }

    Color color;
    IconData icon;

    switch (status) {
      case P2pStatus.sending:
        color = Colors.blue;
        icon = Icons.upload;
      case P2pStatus.success:
        color = Colors.green;
        icon = Icons.check_circle;
      case P2pStatus.error:
        color = Colors.red;
        icon = Icons.error;
      default:
        return const SizedBox.shrink();
    }

    return Container(
      margin: const EdgeInsets.only(bottom: 16),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Row(
        children: [
          status == P2pStatus.sending
              ? SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                    color: color,
                  ),
                )
              : Icon(icon, color: color, size: 20),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              message ?? 'Se trimite...',
              style: TextStyle(color: color),
            ),
          ),
        ],
      ),
    );
  }
}