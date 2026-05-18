using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Services;

public class ReviewJobService : IReviewJobService
{
    private readonly ICourseRepository _courseRepository;

    public ReviewJobService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task ProcessExpiredReviewsAsync()
    {
        var expiredCourses = await _courseRepository.GetExpiredReviewCoursesAsync();

        foreach (var course in expiredCourses)
        {
            course.Status = course.VotesUp >= 10
                ? CourseStatus.ApprovedInCloud
                : CourseStatus.Rejected;

            await _courseRepository.UpdateCourseAsync(course);
        }
    }
}