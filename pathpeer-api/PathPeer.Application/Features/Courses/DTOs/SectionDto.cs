using System;

namespace PathPeer.Application.Features.Courses.DTOs;

public class SectionDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<LessonDto> Lessons { get; set; } = new();
}
