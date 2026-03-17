using System;
using PathPeer.Application.Features.Users.DTOs;

namespace PathPeer.Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public UserDto User { get; set; } = null!;
}
