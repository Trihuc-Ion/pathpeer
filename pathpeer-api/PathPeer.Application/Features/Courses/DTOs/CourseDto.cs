using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.DTOs;

public class CourseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    // public string? Category { get; set; }

    public string Language { get; set; } = null!;
    public decimal Price { get; set; }
    public CourseLevel? Level { get; set; }

    public string Status { get; set; } = null!;
    public int Version { get; set; }

    // public int VotesUp { get; set; }
    // public int VotesDown { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; } = null!;
}