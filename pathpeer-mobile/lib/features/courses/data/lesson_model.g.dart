// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'lesson_model.dart';

// **************************************************************************
// TypeAdapterGenerator
// **************************************************************************

class LessonModelAdapter extends TypeAdapter<LessonModel> {
  @override
  final int typeId = 2;

  @override
  LessonModel read(BinaryReader reader) {
    final numOfFields = reader.readByte();
    final fields = <int, dynamic>{
      for (int i = 0; i < numOfFields; i++) reader.readByte(): reader.read(),
    };
    return LessonModel()
      ..serverId = fields[0] as int
      ..sectionServerId = fields[1] as int
      ..title = fields[2] as String
      ..order = fields[3] as int
      ..blocksJson = fields[4] as String?;
  }

  @override
  void write(BinaryWriter writer, LessonModel obj) {
    writer
      ..writeByte(5)
      ..writeByte(0)
      ..write(obj.serverId)
      ..writeByte(1)
      ..write(obj.sectionServerId)
      ..writeByte(2)
      ..write(obj.title)
      ..writeByte(3)
      ..write(obj.order)
      ..writeByte(4)
      ..write(obj.blocksJson);
  }

  @override
  int get hashCode => typeId.hashCode;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is LessonModelAdapter &&
          runtimeType == other.runtimeType &&
          typeId == other.typeId;
}
