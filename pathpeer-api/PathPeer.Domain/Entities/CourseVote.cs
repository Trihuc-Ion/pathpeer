using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class CourseVote
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public VoteType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
