using System;

namespace PathPeer.Application.Features.Courses.DTOs;

public class ReorderDto
{
    public List<int> OrderedIds { get; set; } = new();
}
