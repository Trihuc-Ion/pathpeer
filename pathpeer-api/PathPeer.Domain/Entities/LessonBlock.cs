using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class LessonBlock
{
    public int Id { get; set; }

    public BlockType Type { get; set; }
    public int Order { get; set; }

    public string Data { get; set; } = null!;

    // Relație cu Lesson
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}