using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.DTOs;

public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public decimal? Price { get; set; }
    public CourseLevel? Level { get; set; }
}
