using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;

    public CourseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Course> CreateCourseAsync(Course course)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        await _db.Entry(course)
        .Reference(c => c.Creator)
        .LoadAsync();
        
        return course;
    }

    public async Task<Course?> GetCourseByIdAsync(int id) =>
        await _db.Courses
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Course>> GetCoursesAsync()
    {
        return await _db.Courses
            .Include(c => c.Creator)
            .ToListAsync();
    }
}
