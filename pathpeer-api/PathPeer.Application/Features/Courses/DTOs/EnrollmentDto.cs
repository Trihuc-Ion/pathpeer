using System;

namespace PathPeer.Application.Features.Courses.DTOs;

public class EnrollmentDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}
