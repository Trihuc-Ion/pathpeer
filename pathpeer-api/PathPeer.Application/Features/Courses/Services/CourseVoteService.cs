using System;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Services;

public class CourseVoteService : ICourseVoteService
{
    private readonly ICourseVoteRepository _voteRepository;
    private readonly ICourseRepository _courseRepository;

    public CourseVoteService(
        ICourseVoteRepository voteRepository,
        ICourseRepository courseRepository)
    {
        _voteRepository = voteRepository;
        _courseRepository = courseRepository;
    }

    public async Task VoteAsync(int userId, int courseId, VoteType type)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new Exception("Cursul nu există");

        var existingVote = await _voteRepository.GetAsync(userId, courseId);

        if (existingVote == null)
        {
            // Vot nou
            await _voteRepository.CreateAsync(new CourseVote
            {
                UserId = userId,
                CourseId = courseId,
                Type = type
            });

            // Actualizezi contorul
            if (type == VoteType.Up) course.VotesUp++;
            else course.VotesDown++;
        }
        else if (existingVote.Type == type)
        {
            // Anulezi votul (Up → None sau Down → None)
            if (type == VoteType.Up) course.VotesUp--;
            else course.VotesDown--;

            await _voteRepository.DeleteAsync(existingVote);
        }
        else
        {
            // Schimbă votul (Up → Down sau invers)
            if (type == VoteType.Up) { course.VotesUp++; course.VotesDown--; }
            else { course.VotesDown++; course.VotesUp--; }

            existingVote.Type = type;
            await _voteRepository.UpdateAsync(existingVote);
        }
        // Dacă votul e același — nu faci nimic
        await _courseRepository.UpdateCourseAsync(course);
    }

    public async Task RemoveVoteAsync(int userId, int courseId)
    {
        var vote = await _voteRepository.GetAsync(userId, courseId);
        if (vote == null)
            throw new Exception("Nu ai votat acest curs");

        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course != null)
        {
            if (vote.Type == VoteType.Up) course.VotesUp--;
            else course.VotesDown--;
            await _courseRepository.UpdateCourseAsync(course);
        }

        await _voteRepository.DeleteAsync(vote);
    }

    public async Task<VoteResultDto> GetVotesAsync(int userId, int courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new Exception("Cursul nu există");

        var userVote = await _voteRepository.GetAsync(userId, courseId);

        return new VoteResultDto
        {
            VotesUp = course.VotesUp,
            VotesDown = course.VotesDown,
            UserVote = userVote?.Type
        };
    }
}
