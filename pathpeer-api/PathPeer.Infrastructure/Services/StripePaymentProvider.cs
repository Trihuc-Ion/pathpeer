using Microsoft.Extensions.Configuration;
using PathPeer.Application.Features.Payments.DTOs;
using PathPeer.Application.Interfaces.Services;
using Stripe;
using Stripe.Checkout;

namespace PathPeer.Infrastructure.Services;

public class StripePaymentProvider : IPaymentProvider
{
    private readonly string _webhookSecret;

    public string ProviderName => "stripe";

    public StripePaymentProvider(IConfiguration configuration)
    {
        var secretKey = configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe:SecretKey lipsește din appsettings.");

        _webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new InvalidOperationException("Stripe:WebhookSecret lipsește din appsettings.");

        StripeConfiguration.ApiKey = secretKey;
    }

    public async Task<CheckoutResultDto> CreateCheckoutSessionAsync(CreateCheckoutDto dto)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = dto.Currency.ToLower(),
                        UnitAmountDecimal = dto.Price * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = dto.CourseTitle
                        }
                    },
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = dto.SuccessUrl,
            CancelUrl = dto.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "userId", dto.UserId.ToString() },
                { "courseId", dto.CourseId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new CheckoutResultDto
        {
            CheckoutUrl = session.Url,
            SessionId = session.Id
        };
    }

    public Task<WebhookResultDto> HandleWebhookAsync(string payload, string signature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, _webhookSecret);
        }
        catch (StripeException)
        {
            return Task.FromResult(new WebhookResultDto { IsSuccess = false });
        }

        if (stripeEvent.Type != EventTypes.CheckoutSessionCompleted)
            return Task.FromResult(new WebhookResultDto { IsSuccess = false });

        var session = stripeEvent.Data.Object as Session;
        if (session is null)
            return Task.FromResult(new WebhookResultDto { IsSuccess = false });

        var userId = int.Parse(session.Metadata["userId"]);
        var courseId = int.Parse(session.Metadata["courseId"]);
        var amountPaid = (session.AmountTotal ?? 0) / 100m;

        return Task.FromResult(new WebhookResultDto
        {
            IsSuccess = true,
            UserId = userId,
            CourseId = courseId,
            AmountPaid = amountPaid,
            Currency = session.Currency.ToUpper(),
            SessionId = session.Id,
            PaymentIntentId = session.PaymentIntentId
        });
    }
}
