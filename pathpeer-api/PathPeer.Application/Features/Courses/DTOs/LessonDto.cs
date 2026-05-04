using System;

namespace PathPeer.Application.Features.Courses.DTOs;

public class LessonDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<BlockDto> Blocks { get; set; } = new();
}
