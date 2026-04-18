using System;
using System.ComponentModel.DataAnnotations;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.DTOs;

public class CreateCourseDto
{
    [Required]
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    [MaxLength(50)]
    public string? Language { get; set; }
    // [MaxLength(100)]
    // public string? Category { get; set; }
    [Range(0, 10000)]
    public decimal Price { get; set; }
    public CourseLevel? Level { get; set; }
}
