using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Features.Users.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Courses.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IDistributedCache _cache;

    public RecommendationService(
        ICourseRepository courseRepository,
        IUserPreferencesRepository preferencesRepository,
        IDistributedCache cache)
    {
        _courseRepository = courseRepository;
        _preferencesRepository = preferencesRepository;
        _cache = cache;
    }

    public async Task<List<CourseDto>> GetRecommendationsAsync(int userId)
    {
        var cacheKey = $"recommendations:{userId}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<CourseDto>>(cached)!;

        var preferences = await _preferencesRepository.GetByUserIdAsync(userId);

        string? language = null;
        if (preferences != null)
        {
            var languages = JsonSerializer.Deserialize<List<string>>(preferences.PreferredLanguages);
            language = languages?.FirstOrDefault();
        }

        var courses = await _courseRepository.GetRecommendedCoursesAsync(
            userId,
            language,
            preferences?.PreferredLevel
        );

        var result = courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Language = c.Language,
            Price = c.Price,
            Level = c.Level,
            Status = c.Status.ToString(),
            Version = c.Version,
            VotesUp = c.VotesUp,
            VotesDown = c.VotesDown,
            Score = c.Score,
            TotalVotes = c.TotalVotes,
            CreatedAt = c.CreatedAt,
            CreatorId = c.CreatorId,
            CreatorUsername = c.Creator.Username
        }).ToList();

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });

        return result;
    }

    public async Task<UserPreferencesDto> UpdatePreferencesAsync(int userId, UpdatePreferencesDto dto)
    {
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId);

        if (preferences == null)
        {
            preferences = new UserPreferences { UserId = userId };
            if (dto.PreferredLanguages != null)
                preferences.PreferredLanguages = JsonSerializer.Serialize(dto.PreferredLanguages);
            if (dto.PreferredLevel != null)
                preferences.PreferredLevel = dto.PreferredLevel;

            await _preferencesRepository.CreateAsync(preferences);
        }
        else
        {
            if (dto.PreferredLanguages != null)
                preferences.PreferredLanguages = JsonSerializer.Serialize(dto.PreferredLanguages);
            if (dto.PreferredLevel != null)
                preferences.PreferredLevel = dto.PreferredLevel;

            await _preferencesRepository.UpdateAsync(preferences);
        }

        await _cache.RemoveAsync($"recommendations:{userId}");

        return new UserPreferencesDto
        {
            PreferredLanguages = JsonSerializer.Deserialize<List<string>>(preferences.PreferredLanguages) ?? new(),
            PreferredLevel = preferences.PreferredLevel,
            UpdatedAt = preferences.UpdatedAt
        };
    }
}