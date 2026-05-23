using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface IP2PTransferRepository
{
    Task<P2PTransfer> CreateAsync(P2PTransfer transfer);
    Task<List<P2PTransfer>> GetByUserAsync(int userId);
}
