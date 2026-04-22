using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _db;

    public LessonRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetLastLessonOrderAsync(int sectionId)
    {
        return await _db.Lessons
            .Where(x => x.SectionId == sectionId)
            .OrderByDescending(x => x.Order)
            .Select(x => x.Order)
            .FirstOrDefaultAsync();
    }
}
