using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ILicenseRepository
{
    Task<bool> ExistsAsync(int userId, int courseId);
    Task<UserLicense> CreateAsync(UserLicense license);
    Task<List<UserLicense>> GetByUserAsync(int userId);
}
