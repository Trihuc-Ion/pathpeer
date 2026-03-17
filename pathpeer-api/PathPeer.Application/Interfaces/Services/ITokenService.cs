using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
