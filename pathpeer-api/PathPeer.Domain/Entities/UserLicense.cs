using PathPeer.Domain.Enums;

namespace PathPeer.Domain.Entities;

public class UserLicense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "USD";
    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public LicenseStatus Status { get; set; } = LicenseStatus.Active;
    public DateTime? RevokedAt { get; set; }
}