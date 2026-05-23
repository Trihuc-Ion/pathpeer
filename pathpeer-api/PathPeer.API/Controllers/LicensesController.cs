using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Interfaces.Repositories;

namespace PathPeer.API.Controllers;

[ApiController]
[Route("api/licenses")]
[Authorize]
public class LicensesController : ControllerBase
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly string _encryptionSecret;

    public LicensesController(ILicenseRepository licenseRepository, IConfiguration configuration)
    {
        _licenseRepository = licenseRepository;
        _encryptionSecret = configuration["P2P:EncryptionSecret"]
            ?? throw new InvalidOperationException("P2P:EncryptionSecret lipsește din appsettings.");
    }

    [HttpGet]
    public async Task<IActionResult> GetMyLicenses()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var licenses = await _licenseRepository.GetByUserAsync(userId);

        var result = licenses.Select(l => new
        {
            l.Id,
            l.CourseId,
            CourseTitle = l.Course.Title,
            l.AmountPaid,
            l.Currency,
            l.PurchasedAt,
            l.Status
        });

        return Ok(result);
    }

    [HttpGet("{courseId}/key")]
    public async Task<IActionResult> GetEncryptionKey(int courseId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (!await _licenseRepository.ExistsAsync(userId, courseId))
            return Forbid();

        var keyMaterial = Encoding.UTF8.GetBytes($"{userId}:{courseId}");
        var secret = Encoding.UTF8.GetBytes(_encryptionSecret);

        var derivedKey = HMACSHA256.HashData(secret, keyMaterial);
        var keyBase64 = Convert.ToBase64String(derivedKey);

        return Ok(new { key = keyBase64 });
    }
}
