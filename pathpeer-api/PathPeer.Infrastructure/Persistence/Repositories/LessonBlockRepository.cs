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
        await _db.LessonBlocks.AddAsync(block);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteLessonBlockAsync(LessonBlock block)
    {
        _db.LessonBlocks.Remove(block);
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

    public async Task<LessonBlock?> GetLessonBlockByIdAsync(int blockId)
    {
        return await _db.LessonBlocks.FindAsync(blockId);
    }

    public async Task<List<LessonBlock>> GetLessonBlocksByLessonIdAsync(int lessonId)
    {
        return await _db.LessonBlocks
            .Where(x => x.LessonId == lessonId)
            .OrderBy(x => x.Order)
            .ToListAsync();
    }

    public async Task ReorderLessonBlocksAsync(int lessonId, List<int> orderedLessonBlockIds)
    {
        var blocks = await _db.LessonBlocks
            .Where(b => b.LessonId == lessonId && orderedLessonBlockIds.Contains(b.Id))
            .ToListAsync();

        for (int i = 0; i < orderedLessonBlockIds.Count; i++)
        {
            var block = blocks.FirstOrDefault(b => b.Id == orderedLessonBlockIds[i]);
            if (block != null) block.Order = i + 1;
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateLessonBlockAsync(LessonBlock block)
    {
        _db.LessonBlocks.Update(block);
        await _db.SaveChangesAsync();
    }
}
