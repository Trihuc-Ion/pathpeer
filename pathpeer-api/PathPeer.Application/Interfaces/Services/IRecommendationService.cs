using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Features.Users.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IRecommendationService
{
    Task<List<CourseDto>> GetRecommendationsAsync(int userId);
    Task<UserPreferencesDto> UpdatePreferencesAsync(int userId, UpdatePreferencesDto dto);
}