using System;
using PathPeer.Application.Features.Courses.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface ICourseService
{
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int creatorId);
    Task<List<CourseDto>> GetCoursesAsync();
    Task<List<CourseDto>> GetCoursesInReviewAsync();
    Task SubmitForReviewAsync(int courseId, int userId);
    Task<CourseDto> GetCourseByIdAsync(int id);
    Task<CourseDto> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int userId);
    Task DeleteCourseAsync(int courseId, int userId);

    //Sections
    Task<SectionDto> AddSection(int courseId, CreateSectionDto dto);
    Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto);
    Task DeleteSectionAsync(int sectionId);
    Task ReorderSectionsAsync(int courseId, List<int> orderedSectionIds);

    //Lessons
    Task<LessonDto> AddLesson(int sectionId, CreateLessonDto dto);
    Task<LessonDto> UpdateLessonAsync(int lessonId, UpdateLessonDto dto);
    Task DeleteLessonAsync(int lessonId);
    Task ReorderLessonsAsync(int sectionId, List<int> orderedLessonIds);

    //Blocks
    Task AddLessonBlock(int lessonId, CreateLessonBlockDto dto);
    Task<object> GetLessonBlocks(int lessonId);
    Task UpdateLessonBlock(int blockId, UpdateLessonBlockDto dto);
    Task DeleteLessonBlock(int blockId);
    Task ReorderBlocksAsync(int lessonId, List<int> orderedBlockIds);
}