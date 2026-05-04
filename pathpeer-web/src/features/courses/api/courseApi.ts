import client from '../../../shared/api/axiosClient'; // your existing axios instance with JWT interceptor
import type { Course, Section, Lesson, Block, BlockType, CourseLevel } from '../../../shared/types/course';

/* ─────────────────────────────────────────────
   DTOs  — adjust field names to match your .NET
   controllers / DTOs exactly
   ───────────────────────────────────────────── */

export interface CreateCourseDto {
  title: string;
  description: string;
  language: string;
  price: number;
  level: CourseLevel;
}

export interface CreateSectionDto {
  title: string;
  order: number;
}

export interface CreateLessonDto {
  title: string;
  order: number;
}

export interface CreateBlockDto {
  type: BlockType;
  order: number;
  data: Record<string, unknown>;
}

/* ─────────────────────────────────────────────
   Response shapes from your .NET API
   (adjust if your API returns different keys)
   ───────────────────────────────────────────── */

export interface CourseResponse {
  id: string;
  title: string;
  description: string;
  language: string;
  price: number;
  level: CourseLevel;
  status: string;
  version: number;
  sections: SectionResponse[];
}

export interface SectionResponse {
  id: string;
  title: string;
  order: number;
  lessons: LessonResponse[];
}

export interface LessonResponse {
  id: string;
  title: string;
  order: number;
  blocks: BlockResponse[];
}

export interface BlockResponse {
  id: string;
  type: BlockType;
  order: number;
  data: Record<string, unknown>;
}

/* ─────────────────────────────────────────────
   Map API response → internal Course type
   ───────────────────────────────────────────── */

export function mapResponseToCourse(r: CourseResponse): Course {
  return {
    id: String(r.id),
    title: r.title ?? '',
    description: r.description ?? '',
    language: r.language ?? 'English',
    price: r.price ?? 0,
    level: r.level ?? 'Beginner',
    status: (r.status as Course['status']) ?? 'Draft',
    version: r.version ?? 1,
    creatorId: 0, // TODO: include creatorId in API response for proper ownership checks
    sections: (r.sections ?? [])
      .sort((a, b) => a.order - b.order)
      .map(
        (s): Section => ({
          id: String(s.id),
          title: s.title ?? '',
          order: s.order,
          isExpanded: false,
          lessons: (s.lessons ?? [])
            .sort((a, b) => a.order - b.order)
            .map(
              (l): Lesson => ({
                id: String(l.id),
                title: l.title ?? '',
                order: l.order,
                isExpanded: false,
                blocks: (l.blocks ?? [])
                  .sort((a, b) => a.order - b.order)
                  .map(
                    (b): Block => ({
                      id: String(b.id),
                      type: b.type,
                      order: b.order,
                      data: b.data as Block['data'],
                    })
                  ),
              })
            ),
        })
      ),
  };
}

export const courseApi = {
  
  getCourses: async (): Promise<Course[]> => {
    const { data } = await client.get<CourseResponse[]>('/courses');
    return data.map(mapResponseToCourse);
  },

  /** GET /api/courses/:id — load a course for editing */
  getCourse: async (id: string): Promise<Course> => {
    const { data } = await client.get<CourseResponse>(`/courses/${id}`);
    return mapResponseToCourse(data);
  },

  /** POST /api/courses — create the course shell */
  createCourse: async (dto: CreateCourseDto): Promise<string> => {
    const { data } = await client.post<CourseResponse>('/courses', dto);
    return String(data.id);
  },

  /** POST /api/courses/:courseId/sections */
  createSection: async (courseId: string, dto: CreateSectionDto): Promise<string> => {
    const { data } = await client.post<SectionResponse>(
      `/courses/${courseId}/sections`,
      dto
    );
    return String(data.id);
  },

  /** POST /api/sections/:sectionId/lessons */
  createLesson: async (sectionId: string, dto: CreateLessonDto): Promise<string> => {
    const { data } = await client.post<LessonResponse>(
      `/sections/${sectionId}/lessons`,
      dto
    );
    return String(data.id);
  },

  /** POST /api/lessons/:lessonId/blocks */
  createBlock: async (lessonId: string, dto: CreateBlockDto): Promise<string> => {
    const { data } = await client.post<BlockResponse>(
      `/lessons/${lessonId}/blocks`,
      dto
    );
    return String(data.id);
  },
};