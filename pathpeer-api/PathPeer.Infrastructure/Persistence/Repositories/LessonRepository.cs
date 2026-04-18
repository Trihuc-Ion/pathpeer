using System;
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
}
