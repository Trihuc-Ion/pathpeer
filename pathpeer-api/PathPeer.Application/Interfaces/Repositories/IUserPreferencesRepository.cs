using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(int userId);
    Task<UserPreferences> CreateAsync(UserPreferences preferences);
    Task<UserPreferences> UpdateAsync(UserPreferences preferences);
}