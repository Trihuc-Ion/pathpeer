import 'package:go_router/go_router.dart';
import 'package:pathpeer_mobile/features/auth/presentation/pages/login_page.dart';
import 'package:pathpeer_mobile/features/auth/presentation/pages/register_page.dart';
import 'package:pathpeer_mobile/features/courses/presentation/pages/course_detail_page.dart';
import 'package:pathpeer_mobile/features/courses/presentation/pages/courses_page.dart';
import 'package:pathpeer_mobile/pages/home_page.dart';

final router = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      builder: (context, state) => const HomePage(),
      ),
    GoRoute(
      path: '/login',
      builder: (context, state) => const LoginPage(),
    ),
    GoRoute(
      path: '/register',
      builder: (context, state) => const RegisterPage(),
    ),
    GoRoute(
      path: '/courses',
      builder: (context, state) => const CoursesPage(),
    ),
    GoRoute(
      path: '/courses/:id',
      builder: (context, state) => CourseDetailPage(
        courseId: int.parse(state.pathParameters['id']!),
      ),
    ),
  ],
);