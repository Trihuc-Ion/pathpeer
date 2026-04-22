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
        _db.Sections.Add(section);
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
}
