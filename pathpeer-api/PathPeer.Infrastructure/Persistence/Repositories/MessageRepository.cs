using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;

    public MessageRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Message> CreateMessageAsync(Message message)
    {
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
        await _db.Entry(message).Reference(m => m.Sender).LoadAsync();
        return message;
    }

    public async Task<List<Message>> GetConversationAsync(int userId1, int userId2, int skip, int take) =>
        await _db.Messages
            .Include(m => m.Sender)
            .Where(m =>
                (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    public async Task MarkAsReadAsync(int senderId, int receiverId)
    {
        await _db.Messages
            .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));
    }
}
