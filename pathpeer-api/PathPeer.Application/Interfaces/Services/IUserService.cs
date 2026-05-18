using PathPeer.Application.Features.Users.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task RequestTeacherAsync(int userId, RequestTeacherDto dto);
    Task ApproveTeacherAsync(int targetUserId);
    Task RejectTeacherAsync(int targetUserId);
}