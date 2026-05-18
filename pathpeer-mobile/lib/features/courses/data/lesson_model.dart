import 'package:hive_flutter/hive_flutter.dart';

part 'lesson_model.g.dart';

@HiveType(typeId: 2)
class LessonModel extends HiveObject {
  @HiveField(0)
  late int serverId;

  @HiveField(1)
  late int sectionServerId;

  @HiveField(2)
  late String title;

  @HiveField(3)
  late int order;

  @HiveField(4)
  String? blocksJson;
}