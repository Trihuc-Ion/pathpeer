using System;
using PathPeer.Application.Features.Auth.DTOs;
using PathPeer.Application.Features.Users.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetProfileAsync(int userId);
}
