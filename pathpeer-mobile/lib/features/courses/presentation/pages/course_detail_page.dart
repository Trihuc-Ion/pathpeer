import 'package:flutter/material.dart';

class CourseDetailPage extends StatelessWidget {
  final int courseId;

  const CourseDetailPage({super.key, required this.courseId});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Course Detail - $courseId'),
      ),
      body: Center(
        child: Text('Details for course ID: $courseId'),
      ),
    );
  }
}