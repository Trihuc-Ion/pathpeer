using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message> CreateMessageAsync(Message message);
    Task<List<Message>> GetConversationAsync(int userId1, int userId2, int skip, int take);
    Task MarkAsReadAsync(int senderId, int receiverId);
}
