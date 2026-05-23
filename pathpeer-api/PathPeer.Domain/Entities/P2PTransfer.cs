namespace PathPeer.Domain.Entities;

public class P2PTransfer
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;
    public int ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime TransferredAt { get; set; } = DateTime.UtcNow;
}