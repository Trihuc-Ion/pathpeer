using System;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Interfaces.Services;

public interface ICourseVoteService
{
    Task VoteAsync(int userId, int courseId, VoteType type);
    Task RemoveVoteAsync(int userId, int courseId);
    Task<VoteResultDto> GetVotesAsync(int userId, int courseId);
}
