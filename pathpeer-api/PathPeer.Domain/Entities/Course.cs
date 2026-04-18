using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string Language { get; set; } = "en";
    public decimal Price { get; set; }
    public CourseLevel? Level { get; set; }

    // Status
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public int Version { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relație cu User
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<Section> Sections { get; set; } = new List<Section>();



    // public DateTime? ReviewStartedAt { get; set; }
    // public DateTime? ReviewEndsAt { get; set; }
    // public bool CanBeApproved() =>
    //     ReviewEndsAt.HasValue &&
    //     ReviewEndsAt.Value <= DateTime.UtcNow;
    // public string? Category { get; set; }
    // public int VotesUp { get; set; } = 0;
    // public int VotesDown { get; set; } = 0;
    // public bool CanBeApproved() =>
    //     VotesUp >= 10 && ReviewEndsAt <= DateTime.UtcNow;
}
