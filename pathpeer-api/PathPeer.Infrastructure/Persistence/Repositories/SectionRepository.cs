using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly AppDbContext _db;

    public SectionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddSectionAsync(Section section)
    {
        await _db.Sections.AddAsync(section);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSectionAsync(Section section)
    {
        _db.Sections.Remove(section);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetLastSectionOrderAsync(int courseId)
    {
        return await _db.Sections
            .Where(x => x.CourseId == courseId)
            .OrderByDescending(x => x.Order)
            .Select(x => x.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(int sectionId)
    {
        return await _db.Sections.FindAsync(sectionId);
    }

    public async Task ReorderSectionsAsync(int courseId, List<int> orderedSectionIds)
    {
        var sections = await _db.Sections
            .Where(s => s.CourseId == courseId && orderedSectionIds.Contains(s.Id))
            .ToListAsync();
        
        for (int i = 0; i < orderedSectionIds.Count; i++)
        {
            var section = sections.FirstOrDefault(s => s.Id == orderedSectionIds[i]);
            if (section != null) section.Order = i + 1;
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateSectionAsync(Section section)
    {
        _db.Sections.Update(section);
        await _db.SaveChangesAsync();
    }
}
