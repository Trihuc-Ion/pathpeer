namespace PathPeer.Application.Features.Users.DTOs;

public class UpdateProfileDto
{
    public string? Username { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? TeacherBio { get; set; }
}