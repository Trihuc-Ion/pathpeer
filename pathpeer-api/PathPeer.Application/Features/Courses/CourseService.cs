using System;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Courses;

public class CourseService : ICourseService
{

    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    
    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int creatorId)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Language = dto.Language,
            Level = dto.Level,
            CreatorId = creatorId
        };

        var createdCourse = await _courseRepository.CreateCourseAsync(course);

        return new CourseDto
        {
            Id = createdCourse.Id,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Language = dto.Language,
            Level = dto.Level,
            Status = createdCourse.Status.ToString(),
            Version = createdCourse.Version,
            VotesUp = createdCourse.VotesUp,
            VotesDown = createdCourse.VotesDown,
            CreatedAt = createdCourse.CreatedAt,
            CreatorId = creatorId,
            CreatorUsername = createdCourse.Creator.Username
        };
    }

    public async Task<CourseDto> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
    
        if (course == null)
            throw new Exception("Cursul nu a fost găsit");
            
        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Category = course.Category,
            Price = course.Price,
            Language = course.Language,
            Level = course.Level,
            Status = course.Status.ToString(),
            Version = course.Version,
            VotesUp = course.VotesUp,
            VotesDown = course.VotesDown,
            CreatedAt = course.CreatedAt,
            CreatorId = course.CreatorId,
            CreatorUsername = course.Creator.Username
        };
    }

    public async Task<List<CourseDto>> GetCoursesAsync()
    {
        var courses = await _courseRepository.GetCoursesAsync();
        
        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Category = c.Category,
            Price = c.Price,
            Language = c.Language,
            Level = c.Level,
            Status = c.Status.ToString(),
            Version = c.Version,
            VotesUp = c.VotesUp,
            VotesDown = c.VotesDown,
            CreatedAt = c.CreatedAt,
            CreatorId = c.CreatorId,
            CreatorUsername = c.Creator.Username
        }).ToList();
    }
}
