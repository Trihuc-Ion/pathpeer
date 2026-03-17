using System;

namespace PathPeer.Application.Common;

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public int ExpiryHours { get; set; } = 24;
}
