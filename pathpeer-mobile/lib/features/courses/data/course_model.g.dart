// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'course_model.dart';

// **************************************************************************
// TypeAdapterGenerator
// **************************************************************************

class CourseModelAdapter extends TypeAdapter<CourseModel> {
  @override
  final int typeId = 0;

  @override
  CourseModel read(BinaryReader reader) {
    final numOfFields = reader.readByte();
    final fields = <int, dynamic>{
      for (int i = 0; i < numOfFields; i++) reader.readByte(): reader.read(),
    };
    return CourseModel()
      ..serverId = fields[0] as int
      ..title = fields[1] as String
      ..description = fields[2] as String?
      ..language = fields[3] as String
      ..price = fields[4] as double
      ..status = fields[5] as String
      ..creatorUsername = fields[6] as String
      ..creatorId = fields[7] as int
      ..level = fields[8] as String?
      ..createdAt = fields[9] as DateTime
      ..updatedAt = fields[10] as DateTime?
      ..isDownloaded = fields[11] as bool
      ..downloadedAt = fields[12] as DateTime?;
  }

  @override
  void write(BinaryWriter writer, CourseModel obj) {
    writer
      ..writeByte(13)
      ..writeByte(0)
      ..write(obj.serverId)
      ..writeByte(1)
      ..write(obj.title)
      ..writeByte(2)
      ..write(obj.description)
      ..writeByte(3)
      ..write(obj.language)
      ..writeByte(4)
      ..write(obj.price)
      ..writeByte(5)
      ..write(obj.status)
      ..writeByte(6)
      ..write(obj.creatorUsername)
      ..writeByte(7)
      ..write(obj.creatorId)
      ..writeByte(8)
      ..write(obj.level)
      ..writeByte(9)
      ..write(obj.createdAt)
      ..writeByte(10)
      ..write(obj.updatedAt)
      ..writeByte(11)
      ..write(obj.isDownloaded)
      ..writeByte(12)
      ..write(obj.downloadedAt);
  }

  @override
  int get hashCode => typeId.hashCode;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CourseModelAdapter &&
          runtimeType == other.runtimeType &&
          typeId == other.typeId;
}
