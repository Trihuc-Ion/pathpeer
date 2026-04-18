using System;
using PathPeer.Application.Features.Courses.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface ICourseService
{
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int creatorId);
    Task<List<CourseDto>> GetCoursesAsync();
    Task<CourseDto> GetCourseByIdAsync(int id);
    Task AddSection(int courseId, CreateSectionDto dto);
    Task AddLesson(int sectionId, CreateLessonDto dto);
    Task AddLessonBlock(int lessonId, CreateLessonBlockDto dto);
    Task<object> GetLessonBlocks(int lessonId);
}