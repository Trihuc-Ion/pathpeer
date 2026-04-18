using PathPeer.Domain.Entities;

namespace PathPeer.Domain.Entities;

public class Section
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;

    // Relație cu Course
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}