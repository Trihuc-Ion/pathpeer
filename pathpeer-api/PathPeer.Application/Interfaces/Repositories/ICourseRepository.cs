using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course);
    Task<List<Course>> GetCoursesAsync();
    Task<Course?> GetCourseByIdAsync(int id);
    Task<Course> UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Course course);   
}
