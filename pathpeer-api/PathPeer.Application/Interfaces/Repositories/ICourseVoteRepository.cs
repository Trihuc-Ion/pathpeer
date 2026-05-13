using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ICourseVoteRepository
{
    Task<CourseVote?> GetAsync(int userId, int courseId);
    Task CreateAsync(CourseVote vote);
    Task UpdateAsync(CourseVote vote);
    Task DeleteAsync(CourseVote vote);
}
