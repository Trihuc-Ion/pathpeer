using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _db;

    public EnrollmentRepository(AppDbContext context)
    {
        _db = context;
    }

    public async Task CreateEnrollmentAsync(Enrollment enrollment)
    {
        await _db.Enrollments.AddAsync(enrollment);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteEnrollmentAsync(Enrollment enrollment)
    {
        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsEnrollmentAsync(int userId, int courseId)
    {
        return await _db.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<Enrollment?> GetEnrollmentAsync(int userId, int courseId) =>
        await _db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(int userId)
    {
        return await _db.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }
}
