using PathPeer.Application.Features.Payments.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<CheckoutResultDto> CreateCheckoutAsync(int userId, int courseId, string provider, string successUrl, string cancelUrl);
    Task ProcessWebhookAsync(string provider, string payload, string signature);
}
