using PathPeer.Application.Features.Payments.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Payments;

public class PaymentService : IPaymentService
{
    private readonly IEnumerable<IPaymentProvider> _providers;
    private readonly ICourseRepository _courseRepository;
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public PaymentService(
        IEnumerable<IPaymentProvider> providers,
        ICourseRepository courseRepository,
        ILicenseRepository licenseRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _providers = providers;
        _courseRepository = courseRepository;
        _licenseRepository = licenseRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<CheckoutResultDto> CreateCheckoutAsync(int userId, int courseId, string provider, string successUrl, string cancelUrl)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId)
            ?? throw new KeyNotFoundException("Cursul nu a fost găsit.");

        if (await _licenseRepository.ExistsAsync(userId, courseId))
            throw new InvalidOperationException("Ai deja acces la acest curs.");

        if (course.Price == 0)
        {
            await _licenseRepository.CreateAsync(new UserLicense
            {
                UserId = userId,
                CourseId = courseId,
                AmountPaid = 0,
                Currency = "USD"
            });
            return new CheckoutResultDto { CheckoutUrl = string.Empty, SessionId = string.Empty };
        }

        var paymentProvider = GetProvider(provider);

        return await paymentProvider.CreateCheckoutSessionAsync(new CreateCheckoutDto
        {
            UserId = userId,
            CourseId = courseId,
            CourseTitle = course.Title,
            Price = course.Price,
            Currency = "USD",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        });
    }

    public async Task ProcessWebhookAsync(string provider, string payload, string signature)
    {
        var paymentProvider = GetProvider(provider);
        var result = await paymentProvider.HandleWebhookAsync(payload, signature);

        if (!result.IsSuccess)
            return;

        if (await _licenseRepository.ExistsAsync(result.UserId, result.CourseId))
            return;

        await _licenseRepository.CreateAsync(new UserLicense
        {
            UserId = result.UserId,
            CourseId = result.CourseId,
            AmountPaid = result.AmountPaid,
            Currency = result.Currency,
            StripeSessionId = result.SessionId,
            StripePaymentIntentId = result.PaymentIntentId
        });

        var course = await _courseRepository.GetCourseByIdAsync(result.CourseId);
        var user = await _userRepository.GetByIdAsync(result.UserId);
        if (course is not null && user is not null)
            await _emailService.SendPurchaseConfirmationAsync(user.Email, user.Username, course.Title, result.AmountPaid);
    }

    private IPaymentProvider GetProvider(string providerName)
    {
        return _providers.FirstOrDefault(p => p.ProviderName == providerName.ToLower())
            ?? throw new NotSupportedException($"Provider-ul '{providerName}' nu este suportat.");
    }
}
