using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class UserPreferences
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string PreferredLanguages { get; set; } = "[]";
    public CourseLevel? PreferredLevel { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}