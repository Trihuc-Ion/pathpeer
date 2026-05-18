import 'package:pathpeer_mobile/core/storage/local_db.dart';
import 'course_model.dart';
import 'section_model.dart';
import 'lesson_model.dart';

class CoursesLocal {

  // ── COURSES ──────────────────────────

  List<CourseModel> getDownloadedCourses() {
    return LocalDb.courses.values
        .where((c) => c.isDownloaded)
        .toList();
  }

  CourseModel? getCourseByServerId(int serverId) {
    try {
      return LocalDb.courses.values
          .firstWhere((c) => c.serverId == serverId);
    } catch (_) {
      return null;
    }
  }

  Future<void> saveCourse(CourseModel course) async {
    await LocalDb.courses.put(course.serverId, course);
  }

  Future<void> deleteCourse(int serverId) async {
    await LocalDb.courses.delete(serverId);
    // Ștergi și secțiunile + lecțiile
    final sectionKeys = LocalDb.sections.keys
        .where((k) => LocalDb.sections.get(k)?.courseServerId == serverId)
        .toList();
    await LocalDb.sections.deleteAll(sectionKeys);
  }

  // ── SECTIONS ──────────────────────────

  List<SectionModel> getSectionsByCourse(int courseServerId) {
    return LocalDb.sections.values
        .where((s) => s.courseServerId == courseServerId)
        .toList()
      ..sort((a, b) => a.order.compareTo(b.order));
  }

  Future<void> saveSections(List<SectionModel> sections) async {
    for (final section in sections) {
      await LocalDb.sections.put(section.serverId, section);
    }
  }

  // ── LESSONS ──────────────────────────

  List<LessonModel> getLessonsBySection(int sectionServerId) {
    return LocalDb.lessons.values
        .where((l) => l.sectionServerId == sectionServerId)
        .toList()
      ..sort((a, b) => a.order.compareTo(b.order));
  }

  Future<void> saveLessons(List<LessonModel> lessons) async {
    for (final lesson in lessons) {
      await LocalDb.lessons.put(lesson.serverId, lesson);
    }
  }
}