using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Users.DTOs;

public class UpdatePreferencesDto
{
    public List<string>? PreferredLanguages { get; set; }
    public CourseLevel? PreferredLevel { get; set; }
}