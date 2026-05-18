using System;
using PathPeer.Domain.Entities;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course);
    Task<List<Course>> GetCoursesAsync();
    Task<List<Course>> GetCoursesInReviewAsync();
    Task<List<Course>> GetExpiredReviewCoursesAsync();
    Task<List<Course>> GetRecommendedCoursesAsync(int userId, string? language, CourseLevel? level);
    Task<Course?> GetCourseByIdAsync(int id);
    Task<Course> UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Course course);   
}
