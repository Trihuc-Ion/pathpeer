using PathPeer.Application.Features.Payments.DTOs;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.Infrastructure.Services;

public class PaynetPaymentProvider : IPaymentProvider
{
    public string ProviderName => "paynet";

    public Task<CheckoutResultDto> CreateCheckoutSessionAsync(CreateCheckoutDto dto)
    {
        throw new NotImplementedException("Paynet provider not implemented yet.");
    }

    public Task<WebhookResultDto> HandleWebhookAsync(string payload, string signature)
    {
        throw new NotImplementedException("Paynet provider not implemented yet.");
    }
}
