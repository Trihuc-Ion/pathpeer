namespace PathPeer.Application.Features.Payments.DTOs;

public class WebhookResultDto
{
    public bool IsSuccess { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = null!;
    public string SessionId { get; set; } = null!;
    public string? PaymentIntentId { get; set; }
}