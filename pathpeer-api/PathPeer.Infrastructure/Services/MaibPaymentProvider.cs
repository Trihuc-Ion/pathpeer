using PathPeer.Application.Features.Payments.DTOs;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.Infrastructure.Services;

public class MaibPaymentProvider : IPaymentProvider
{
    public string ProviderName => "maib";

    public Task<CheckoutResultDto> CreateCheckoutSessionAsync(CreateCheckoutDto dto)
    {
        throw new NotImplementedException("MAIB ePay provider not implemented yet.");
    }

    public Task<WebhookResultDto> HandleWebhookAsync(string payload, string signature)
    {
        throw new NotImplementedException("MAIB ePay provider not implemented yet.");
    }
}
