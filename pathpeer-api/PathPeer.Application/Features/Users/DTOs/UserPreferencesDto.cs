using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Users.DTOs;

public class UserPreferencesDto
{
    public List<string> PreferredLanguages { get; set; } = new();
    public CourseLevel? PreferredLevel { get; set; }
    public DateTime UpdatedAt { get; set; }
}