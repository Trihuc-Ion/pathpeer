import 'package:hive_flutter/hive_flutter.dart';

part 'section_model.g.dart';

@HiveType(typeId: 1)
class SectionModel extends HiveObject {
  @HiveField(0)
  late int serverId;

  @HiveField(1)
  late int courseServerId;

  @HiveField(2)
  late String title;

  @HiveField(3)
  late int order;
}