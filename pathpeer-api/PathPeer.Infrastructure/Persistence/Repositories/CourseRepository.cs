using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;
using PathPeer.Domain.Enums;

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

    public async Task<List<Course>> GetCoursesAsync() =>
        await _db.Courses
            .Include(c => c.Creator)
            .ToListAsync();

    public async Task<List<Course>> GetCoursesInReviewAsync() =>
        await _db.Courses
            .Include(c => c.Creator)
            .Where(c => c.Status == CourseStatus.InReviewCloud)
            .ToListAsync();

    public async Task<List<Course>> GetExpiredReviewCoursesAsync() =>
        await _db.Courses
            .Where(c => c.Status == CourseStatus.InReviewCloud && c.ReviewEndsAt <= DateTime.UtcNow)
            .ToListAsync();

    public async Task<List<Course>> GetRecommendedCoursesAsync(int userId, string? language, CourseLevel? level)
    {
        var enrolledCourseIds = await _db.Enrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.CourseId)
            .ToListAsync();

        var query = _db.Courses
            .Include(c => c.Creator)
            .Where(c => c.Status == CourseStatus.ApprovedInCloud && !enrolledCourseIds.Contains(c.Id));

        if (language != null)
            query = query.Where(c => c.Language == language);

        if (level != null)
            query = query.Where(c => c.Level == level);

        return await query
            .OrderByDescending(c => c.Score)
            .Take(10)
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
