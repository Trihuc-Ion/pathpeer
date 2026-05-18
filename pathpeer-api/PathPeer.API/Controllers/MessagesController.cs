using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Features.Chat.DTOs;
using PathPeer.Application.Interfaces.Repositories;

namespace PathPeer.API.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;

    public MessagesController(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetConversation(int userId, [FromQuery] int page = 0)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var messages = await _messageRepository.GetConversationAsync(currentUserId, userId, page * 50, 50);

        var result = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderUsername = m.Sender.Username,
            ReceiverId = m.ReceiverId,
            Content = m.Content,
            SentAt = m.SentAt,
            IsRead = m.IsRead
        });

        await _messageRepository.MarkAsReadAsync(userId, currentUserId);

        return Ok(result);
    }
}
