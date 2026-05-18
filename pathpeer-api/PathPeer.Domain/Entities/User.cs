using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Username { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }

    //User roles
    public UserRole Role { get; set; } = UserRole.User;

    // Status profesor
    public TeacherStatus TeacherStatus { get; set; } = TeacherStatus.NotRequested;
    public string? TeacherBio { get; set; }
    public DateTime? TeacherApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<UserLicense> Licenses { get; set; } = new List<UserLicense>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public UserPreferences? Preferences { get; set; }
}
