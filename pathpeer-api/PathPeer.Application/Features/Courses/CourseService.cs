using System;
using System.Text.Json;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Features.Courses.Helpers;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Courses;

public class CourseService : ICourseService
{

    private readonly ICourseRepository _courseRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonBlockRepository _lessonBlockRepository;

    public CourseService(
        ICourseRepository courseRepository,
        ISectionRepository sectionRepository,
        ILessonRepository lessonRepository,
        ILessonBlockRepository lessonBlockRepository)
    {
        _courseRepository = courseRepository;
        _sectionRepository = sectionRepository;
        _lessonRepository = lessonRepository;
        _lessonBlockRepository = lessonBlockRepository;
    }
    
    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int creatorId)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            // Category = dto.Category,
            Language = dto.Language,
            Price = dto.Price,
            Level = dto.Level,
            CreatorId = creatorId
        };

        var createdCourse = await _courseRepository.CreateCourseAsync(course);

        return new CourseDto
        {
            Id = createdCourse.Id,
            Title = dto.Title,
            Description = dto.Description,
            // Category = dto.Category,
            Language = dto.Language,
            Price = dto.Price,
            Level = dto.Level,
            Status = createdCourse.Status.ToString(),
            Version = createdCourse.Version,
            // VotesUp = createdCourse.VotesUp,
            // VotesDown = createdCourse.VotesDown,
            CreatedAt = createdCourse.CreatedAt,
            CreatorId = creatorId,
            CreatorUsername = createdCourse.Creator.Username
        };
    }

    

    public async Task AddSection(int courseId, CreateSectionDto dto)
    {
        var section = new Section
        {
            Title = dto.Title,
            CourseId = courseId
        };

        await _sectionRepository.AddSectionAsync(section);
    }

    public async Task AddLesson(int sectionId, CreateLessonDto dto)
    {
        var lesson = new Lesson
        {
            Title = dto.Title,
            SectionId = sectionId
        };

        await _lessonRepository.AddLessonAsync(lesson);
    }

    public async Task AddLessonBlock(int lessonId, CreateLessonBlockDto dto)
    {
        var lastOrder = await _lessonBlockRepository.GetLastLessonBlockOrderAsync(lessonId);
        var nextOrder = lastOrder + 1;

        var block = new LessonBlock
        {
            LessonId = lessonId,
            Type = dto.Type,
            Order = nextOrder,
            Data = JsonSerializer.Serialize(dto.Data)
        };

        await _lessonBlockRepository.AddLessonBlockAsync(block);
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
            // Category = course.Category,
            Price = course.Price,
            Language = course.Language,
            Level = course.Level,
            Status = course.Status.ToString(),
            Version = course.Version,
            // VotesUp = course.VotesUp,
            // VotesDown = course.VotesDown,
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
            // Category = c.Category,
            Price = c.Price,
            Language = c.Language,
            Level = c.Level,
            Status = c.Status.ToString(),
            Version = c.Version,
            // VotesUp = c.VotesUp,
            // VotesDown = c.VotesDown,
            CreatedAt = c.CreatedAt,
            CreatorId = c.CreatorId,
            CreatorUsername = c.Creator.Username
        }).ToList();
    }
    public async Task<object> GetLessonBlocks(int lessonId)
    {
        var blocks = await _lessonBlockRepository.GetLessonBlocksByLessonIdAsync(lessonId);

        return blocks.Select(b => new
        {
            b.Id,
            b.Type,
            Data = BlockDataHelper.Deserialize(b)
        });
    }
}
