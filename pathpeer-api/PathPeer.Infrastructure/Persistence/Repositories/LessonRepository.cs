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
        await _db.Lessons.AddAsync(lesson);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteLessonAsync(Lesson lesson)
    {
        _db.Lessons.Remove(lesson);
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

    public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
    {
        return await _db.Lessons.FindAsync(lessonId);
    }

    public async Task ReorderLessonsAsync(int sectionId, List<int> orderedLessonIds)
    {
        var lessons = await _db.Lessons
            .Where(l => l.SectionId == sectionId && orderedLessonIds.Contains(l.id))
            .ToListAsync();

        for (int i = 0; i < orderedLessonIds.Count; i++)
        {
            var lesson = lessons.FirstOrDefault(l => l.id == orderedLessonIds[i]);
            if (lesson != null) lesson.Order = i + 1;
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateLessonAsync(Lesson lesson)
    {
        _db.Lessons.Update(lesson);
        await _db.SaveChangesAsync();
    }
}
