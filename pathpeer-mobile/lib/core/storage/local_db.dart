import 'package:hive_flutter/hive_flutter.dart';
import 'package:pathpeer_mobile/features/courses/data/course_model.dart';
import 'package:pathpeer_mobile/features/courses/data/section_model.dart';
import 'package:pathpeer_mobile/features/courses/data/lesson_model.dart';

class LocalDb {
  static Future<void> init() async {
    await Hive.initFlutter();

    // Înregistrezi adaptoarele generate
    Hive.registerAdapter(CourseModelAdapter());
    Hive.registerAdapter(SectionModelAdapter());
    Hive.registerAdapter(LessonModelAdapter());

    // Deschizi box-urile
    await Hive.openBox<CourseModel>('courses');
    await Hive.openBox<SectionModel>('sections');
    await Hive.openBox<LessonModel>('lessons');
  }

  static Box<CourseModel> get courses => Hive.box<CourseModel>('courses');
  static Box<SectionModel> get sections => Hive.box<SectionModel>('sections');
  static Box<LessonModel> get lessons => Hive.box<LessonModel>('lessons');
}