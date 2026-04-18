using System;
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
}
