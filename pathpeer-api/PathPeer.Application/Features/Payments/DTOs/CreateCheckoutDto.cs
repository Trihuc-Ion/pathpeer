namespace PathPeer.Application.Features.Payments.DTOs;

public class CreateCheckoutDto
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = null!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public string SuccessUrl { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
}