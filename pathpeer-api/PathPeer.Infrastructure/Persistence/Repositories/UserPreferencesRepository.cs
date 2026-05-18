using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly AppDbContext _db;

    public UserPreferencesRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserPreferences?> GetByUserIdAsync(int userId) =>
        await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<UserPreferences> CreateAsync(UserPreferences preferences)
    {
        await _db.UserPreferences.AddAsync(preferences);
        await _db.SaveChangesAsync();
        return preferences;
    }

    public async Task<UserPreferences> UpdateAsync(UserPreferences preferences)
    {
        preferences.UpdatedAt = DateTime.UtcNow;
        _db.UserPreferences.Update(preferences);
        await _db.SaveChangesAsync();
        return preferences;
    }
}