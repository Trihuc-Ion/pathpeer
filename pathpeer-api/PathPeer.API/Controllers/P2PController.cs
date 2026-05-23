using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Features.P2P.DTOs;

using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.API.Controllers;

[ApiController]
[Route("api/p2p")]
[Authorize]
public class P2PController : ControllerBase
{
    private readonly IP2PTransferRepository _p2pRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILicenseRepository _licenseRepository;

    public P2PController(
        IP2PTransferRepository p2pRepository,
        ICourseRepository courseRepository,
        ILicenseRepository licenseRepository)
    {
        _p2pRepository = p2pRepository;
        _courseRepository = courseRepository;
        _licenseRepository = licenseRepository;
    }

    [HttpPost("transfers")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateP2PTransferDto dto)
    {
        var receiverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var course = await _courseRepository.GetCourseByIdAsync(dto.CourseId);
        if (course is null)
            return NotFound("Cursul nu a fost găsit.");

        if (course.CreatorId != dto.SenderId)
            return BadRequest("Sender-ul nu este creatorul acestui curs.");

        if (await _licenseRepository.ExistsAsync(receiverId, dto.CourseId))
            return BadRequest("Ai deja acces la acest curs.");

        await _licenseRepository.CreateAsync(new UserLicense
        {
            UserId = receiverId,
            CourseId = dto.CourseId,
            AmountPaid = 0,
            Currency = "USD"
        });

        var transfer = await _p2pRepository.CreateAsync(new P2PTransfer
        {
            SenderId = dto.SenderId,
            ReceiverId = receiverId,
            CourseId = dto.CourseId
        });

        return Ok(new P2PTransferDto
        {
            Id = transfer.Id,
            CourseId = course.Id,
            CourseTitle = course.Title,
            SenderUsername = course.Creator.Username,
            ReceiverUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            TransferredAt = transfer.TransferredAt
        });
    }

    [HttpGet("transfers")]
    public async Task<IActionResult> GetTransfers()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var transfers = await _p2pRepository.GetByUserAsync(userId);

        var result = transfers.Select(t => new P2PTransferDto
        {
            Id = t.Id,
            CourseId = t.CourseId,
            CourseTitle = t.Course.Title,
            SenderUsername = t.Sender.Username,
            ReceiverUsername = t.Receiver.Username,
            TransferredAt = t.TransferredAt
        });

        return Ok(result);
    }
}
