namespace PathPeer.Application.Features.Chat.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } = null!;
    public int ReceiverId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}
