namespace PathPeer.Application.Features.P2P.DTOs;

public class P2PTransferDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = null!;
    public string SenderUsername { get; set; } = null!;
    public string ReceiverUsername { get; set; } = null!;
    public DateTime TransferredAt { get; set; }
}