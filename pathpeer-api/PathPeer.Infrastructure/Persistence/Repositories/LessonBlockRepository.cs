using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class LessonBlockRepository : ILessonBlockRepository
{
    private readonly AppDbContext _db;
    
    public LessonBlockRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddLessonBlockAsync(LessonBlock block)
    {
        _db.LessonBlocks.Add(block);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetLastLessonBlockOrderAsync(int lessonId)
    {
        return await _db.LessonBlocks
            .Where(x => x.LessonId == lessonId)
            .OrderByDescending(x => x.Order)
            .Select(x => x.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<List<LessonBlock>> GetLessonBlocksByLessonIdAsync(int lessonId)
    {
        return await _db.LessonBlocks
            .Where(x => x.LessonId == lessonId)
            .OrderBy(x => x.Order)
            .ToListAsync();
    }
}
