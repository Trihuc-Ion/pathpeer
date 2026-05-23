using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class P2PTransferRepository : IP2PTransferRepository
{
    private readonly AppDbContext _db;

    public P2PTransferRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<P2PTransfer> CreateAsync(P2PTransfer transfer)
    {
        await _db.P2PTransfers.AddAsync(transfer);
        await _db.SaveChangesAsync();
        return transfer;
    }

    public async Task<List<P2PTransfer>> GetByUserAsync(int userId) =>
        await _db.P2PTransfers
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Include(t => t.Course)
            .Where(t => t.SenderId == userId || t.ReceiverId == userId)
            .OrderByDescending(t => t.TransferredAt)
            .ToListAsync();
}
