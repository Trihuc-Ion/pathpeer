using System;
using PathPeer.Application.Features.Courses.DTOs;

namespace PathPeer.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task EnrollAsync(int userId, int courseId);
    Task UnenrollAsync(int userId, int courseId);
    Task<IEnumerable<EnrollmentDto>> GetUserEnrollmentsAsync(int userId);
    Task<bool> IsEnrolledAsync(int userId, int courseId);
}
