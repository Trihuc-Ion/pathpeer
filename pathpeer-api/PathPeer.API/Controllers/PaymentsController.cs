using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly string _frontendUrl;

    public PaymentsController(IPaymentService paymentService, IConfiguration configuration)
    {
        _paymentService = paymentService;
        _frontendUrl = configuration["FrontendUrl"]
            ?? throw new InvalidOperationException("FrontendUrl lipsește din appsettings.");
    }

    [HttpPost("checkout/{courseId}")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout(int courseId, [FromQuery] string provider = "stripe")
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var successUrl = $"{_frontendUrl}/payment/success?courseId={courseId}";
        var cancelUrl = $"{_frontendUrl}/payment/cancel?courseId={courseId}";

        try
        {
            var result = await _paymentService.CreateCheckoutAsync(userId, courseId, provider, successUrl, cancelUrl);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("webhook/{provider}")]
    public async Task<IActionResult> Webhook(string provider)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        await _paymentService.ProcessWebhookAsync(provider, payload, signature);

        return Ok();
    }
}
