namespace PathPeer.Domain.Entities;

public class Lesson
{
    public int id { get; set; }
    public string Title { get; set; } = null!;
    public int Order { get; set; }

    // Relație cu Section
    public int SectionId { get; set; }
    public Section Section { get; set; } = null!;

    public ICollection<LessonBlock> Blocks { get; set; } = new List<LessonBlock>();
}