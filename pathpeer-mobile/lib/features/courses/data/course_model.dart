import 'package:hive_flutter/hive_flutter.dart';

part 'course_model.g.dart';

@HiveType(typeId: 0)
class CourseModel extends HiveObject {
  @HiveField(0)
  late int serverId;

  @HiveField(1)
  late String title;

  @HiveField(2)
  String? description;

  @HiveField(3)
  late String language;

  @HiveField(4)
  late double price;

  @HiveField(5)
  late String status;

  @HiveField(6)
  late String creatorUsername;

  @HiveField(7)
  late int creatorId;

  @HiveField(8)
  String? level;

  @HiveField(9)
  late DateTime createdAt;

  @HiveField(10)
  DateTime? updatedAt;

  @HiveField(11)
  bool isDownloaded = false;

  @HiveField(12)
  DateTime? downloadedAt;
}