using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class LicenseRepository : ILicenseRepository
{
    private readonly AppDbContext _db;

    public LicenseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsAsync(int userId, int courseId) =>
        await _db.UserLicenses.AnyAsync(l => l.UserId == userId && l.CourseId == courseId);

    public async Task<UserLicense> CreateAsync(UserLicense license)
    {
        await _db.UserLicenses.AddAsync(license);
        await _db.SaveChangesAsync();
        return license;
    }

    public async Task<List<UserLicense>> GetByUserAsync(int userId) =>
        await _db.UserLicenses
            .Include(l => l.Course)
            .Where(l => l.UserId == userId && l.Status == Domain.Enums.LicenseStatus.Active)
            .OrderByDescending(l => l.PurchasedAt)
            .ToListAsync();
}