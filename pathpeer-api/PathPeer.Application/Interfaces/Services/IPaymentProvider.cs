using PathPeer.Application.Features.Payments.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IPaymentProvider
{
    string ProviderName { get; }
    Task<CheckoutResultDto> CreateCheckoutSessionAsync(CreateCheckoutDto dto);
    Task<WebhookResultDto> HandleWebhookAsync(string payload, string signature);
}