using System;
using Microsoft.EntityFrameworkCore;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence.Repositories;

public class CourseVoteRepository : ICourseVoteRepository
{
    private readonly AppDbContext _db;
    public CourseVoteRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task CreateAsync(CourseVote vote)
    {
        await _db.CourseVotes.AddAsync(vote);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(CourseVote vote)
    {
        _db.CourseVotes.Remove(vote);
        await _db.SaveChangesAsync();
    }

    public async Task<CourseVote?> GetAsync(int userId, int courseId)
    {
        return await _db.CourseVotes
            .FirstOrDefaultAsync(v => v.UserId == userId && v.CourseId == courseId);
    }

    public async Task UpdateAsync(CourseVote vote)
    {
        _db.CourseVotes.Update(vote);
        await _db.SaveChangesAsync();
    }
}
