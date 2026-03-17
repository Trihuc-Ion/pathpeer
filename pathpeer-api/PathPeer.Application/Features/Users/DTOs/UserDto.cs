using System;

namespace PathPeer.Application.Features.Users.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string TeacherStatus { get; set; } = null!;
}
