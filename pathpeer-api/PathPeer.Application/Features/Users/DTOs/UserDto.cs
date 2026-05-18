using System;

namespace PathPeer.Application.Features.Users.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string TeacherStatus { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? TeacherBio { get; set; }
    public DateTime? TeacherApprovedAt { get; set; }
}
