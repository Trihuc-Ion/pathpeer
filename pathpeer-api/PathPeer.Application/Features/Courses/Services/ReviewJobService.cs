using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Services;

public class ReviewJobService : IReviewJobService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEmailService _emailService;

    public ReviewJobService(ICourseRepository courseRepository, IEmailService emailService)
    {
        _courseRepository = courseRepository;
        _emailService = emailService;
    }

    public async Task ProcessExpiredReviewsAsync()
    {
        var expiredCourses = await _courseRepository.GetExpiredReviewCoursesAsync();

        foreach (var course in expiredCourses)
        {
            var approved = course.VotesUp >= 10;
            course.Status = approved ? CourseStatus.ApprovedInCloud : CourseStatus.Rejected;

            await _courseRepository.UpdateCourseAsync(course);

            if (approved)
                await _emailService.SendCourseApprovedAsync(course.Creator.Email, course.Creator.Username, course.Title);
            else
                await _emailService.SendCourseRejectedAsync(course.Creator.Email, course.Creator.Username, course.Title);
        }
    }
}