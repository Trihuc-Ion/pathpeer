using System;

namespace PathPeer.Application.Features.Courses.DTOs;

public class BlockDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Type { get; set; } = string.Empty;
    public object? Data { get; set; } 
}
