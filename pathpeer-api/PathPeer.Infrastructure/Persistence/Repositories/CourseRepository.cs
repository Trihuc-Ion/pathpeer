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
        await _db.Courses.AddAsync(course);
        await _db.SaveChangesAsync();

        await _db.Entry(course)
        .Reference(c => c.Creator)
        .LoadAsync();

        return course;
    }

    public async Task DeleteCourseAsync(Course course)
    {
        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id) =>
        await _db.Courses
        .Include(c => c.Creator)
        .Include(c => c.Sections.OrderBy(s => s.Order))
            .ThenInclude(s => s.Lessons.OrderBy(l => l.Order))
                .ThenInclude(l => l.Blocks.OrderBy(b => b.Order))
        .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Course>> GetCoursesAsync()
    {
        return await _db.Courses
            .Include(c => c.Creator)
            .ToListAsync();
    }

    public async Task<Course> UpdateCourseAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        _db.Courses.Update(course);
        await _db.SaveChangesAsync();
        return course;
    }
}
