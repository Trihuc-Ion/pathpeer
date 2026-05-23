using Microsoft.Extensions.Logging;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Services;

public class ReviewJobService : IReviewJobService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReviewJobService> _logger;

    public ReviewJobService(ICourseRepository courseRepository, IEmailService emailService, ILogger<ReviewJobService> logger)
    {
        _courseRepository = courseRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ProcessExpiredReviewsAsync()
    {
        var expiredCourses = await _courseRepository.GetExpiredReviewCoursesAsync();
        _logger.LogInformation("Review job started: {Count} cursuri expirate de procesat", expiredCourses.Count);

        var approved = 0;
        var rejected = 0;

        foreach (var course in expiredCourses)
        {
            var isApproved = course.VotesUp >= 10;
            course.Status = isApproved ? CourseStatus.ApprovedInCloud : CourseStatus.Rejected;

            await _courseRepository.UpdateCourseAsync(course);

            if (isApproved)
            {
                approved++;
                _logger.LogInformation("Curs aprobat: {Title} (courseId: {Id}, votes: {Votes})", course.Title, course.Id, course.VotesUp);
                await _emailService.SendCourseApprovedAsync(course.Creator.Email, course.Creator.Username, course.Title);
            }
            else
            {
                rejected++;
                _logger.LogInformation("Curs respins: {Title} (courseId: {Id}, votes: {Votes})", course.Title, course.Id, course.VotesUp);
                await _emailService.SendCourseRejectedAsync(course.Creator.Email, course.Creator.Username, course.Title);
            }
        }

        _logger.LogInformation("Review job finalizat: {Approved} aprobate, {Rejected} respinse", approved, rejected);
    }
}