using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public async Task<User> UpdateUserAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllUserAsync() =>
        await _db.Users.ToListAsync();
}
