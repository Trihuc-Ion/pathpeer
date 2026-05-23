namespace PathPeer.Application.Features.Payments.DTOs;

public class CheckoutResultDto
{
    public string CheckoutUrl { get; set; } = null!;
    public string SessionId { get; set; } = null!;
}