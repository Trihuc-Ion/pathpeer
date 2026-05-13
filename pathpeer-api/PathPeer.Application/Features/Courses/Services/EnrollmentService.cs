using System;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Courses.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    
    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }
    public async Task EnrollAsync(int userId, int courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new Exception("Cursul nu există");

        var alreadyEnrolled = await _enrollmentRepository.ExistsEnrollmentAsync(userId, courseId);
        if (alreadyEnrolled)
            throw new Exception("Ești deja înscris la acest curs");

        await _enrollmentRepository.CreateEnrollmentAsync(new Enrollment
        {
            UserId = userId,
            CourseId = courseId
        });
    }

    public async Task<IEnumerable<EnrollmentDto>> GetUserEnrollmentsAsync(int userId)
    {
        var enrollments = await _enrollmentRepository.GetEnrollmentsByUserAsync(userId);
        return enrollments.Select(e => new EnrollmentDto
        {
            CourseId = e.CourseId,
            CourseTitle = e.Course.Title,
            EnrolledAt = e.EnrolledAt
        });
    }

    public async Task<bool> IsEnrolledAsync(int userId, int courseId)
    {
        return await _enrollmentRepository.ExistsEnrollmentAsync(userId, courseId);
    }

    public async Task UnenrollAsync(int userId, int courseId)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);
        if (enrollment == null)
            throw new Exception("Nu ești înscris la acest curs");

        await _enrollmentRepository.DeleteEnrollmentAsync(enrollment);
    }
}
