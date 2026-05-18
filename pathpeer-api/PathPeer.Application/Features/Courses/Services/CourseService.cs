using System;
using System.Text.Json;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Features.Courses.Helpers;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Services;

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



    public async Task<SectionDto> AddSection(int courseId, CreateSectionDto dto)
    {
        var lastOrder = await _sectionRepository.GetLastSectionOrderAsync(courseId);

        var section = new Section
        {
            Title = dto.Title,
            CourseId = courseId,
            Order = lastOrder + 1
        };

        await _sectionRepository.AddSectionAsync(section);
        return new SectionDto
        {
            Id = section.Id,
            Title = section.Title,
            Order = section.Order
        };
    }

    public async Task<LessonDto> AddLesson(int sectionId, CreateLessonDto dto)
    {
        var lastOrder = await _lessonRepository.GetLastLessonOrderAsync(sectionId);

        var lesson = new Lesson
        {
            Title = dto.Title,
            SectionId = sectionId,
            Order = lastOrder + 1
        };

        await _lessonRepository.AddLessonAsync(lesson);
        return new LessonDto
        {
            Id = lesson.id,
            Title = lesson.Title,
            Order = lesson.Order
        };
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
            CreatorUsername = course.Creator.Username,

            Sections = course.Sections?.Select(s => new SectionDto
            {
                Id = s.Id,
                Title = s.Title,
                Order = s.Order,
                Lessons = s.Lessons?.Select(l => new LessonDto
                {
                    Id = l.id,
                    Title = l.Title,
                    Order = l.Order,
                    Blocks = l.Blocks?.Select(b => new BlockDto
                    {
                        Id = b.Id,
                        Type = b.Type.ToString(),
                        Order = b.Order,
                        Data = BlockDataHelper.Deserialize(b)
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList() ?? new()
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

    public async Task<CourseDto> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new Exception("Cursul nu există");

        if (course.CreatorId != userId)
            throw new Exception("Nu ai permisiunea să modifici acest curs");

        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.Language != null) course.Language = dto.Language;
        if (dto.Price != null) course.Price = dto.Price.Value;
        if (dto.Level != null) course.Level = dto.Level;

        var updated = await _courseRepository.UpdateCourseAsync(course);

        return new CourseDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Language = updated.Language,
            Price = updated.Price,
            Level = updated.Level,
            Status = updated.Status.ToString(),
            Version = updated.Version,
            CreatedAt = updated.CreatedAt,
            CreatorId = updated.CreatorId,
            CreatorUsername = updated.Creator.Username
        };
    }

    public async Task DeleteCourseAsync(int courseId, int userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new Exception("Cursul nu există");

        if (course.CreatorId != userId)
            throw new Exception("Nu ai permisiunea să ștergi acest curs");

        await _courseRepository.DeleteCourseAsync(course);
    }

    public async Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto)
    {
        var section = await _sectionRepository.GetSectionByIdAsync(sectionId)
        ?? throw new Exception("Secțiunea nu există");

        section.Title = dto.Title;
        await _sectionRepository.UpdateSectionAsync(section);

        return new SectionDto { Id = section.Id, Title = section.Title, Order = section.Order };
    }

    public async Task DeleteSectionAsync(int sectionId)
    {
        var section = await _sectionRepository.GetSectionByIdAsync(sectionId)
        ?? throw new Exception("Secțiunea nu există");

        await _sectionRepository.DeleteSectionAsync(section);
    }

    public async Task ReorderSectionsAsync(int courseId, List<int> orderedSectionIds)
    {
        await _sectionRepository.ReorderSectionsAsync(courseId, orderedSectionIds);
    }

    public async Task<LessonDto> UpdateLessonAsync(int lessonId, UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId)
        ?? throw new Exception("Lecția nu există");

        lesson.Title = dto.Title;
        await _lessonRepository.UpdateLessonAsync(lesson);

        return new LessonDto { Id = lesson.id, Title = lesson.Title, Order = lesson.Order };

    }

    public async Task DeleteLessonAsync(int lessonId)
    {
        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId)
        ?? throw new Exception("Lecția nu există");

        await _lessonRepository.DeleteLessonAsync(lesson);
    }

    public async Task ReorderLessonsAsync(int sectionId, List<int> orderedLessonIds)
    {
        await _lessonRepository.ReorderLessonsAsync(sectionId, orderedLessonIds);
    }

    public async Task UpdateLessonBlock(int blockId, UpdateLessonBlockDto dto)
    {
        var block = await _lessonBlockRepository.GetLessonBlockByIdAsync(blockId)
        ?? throw new Exception("Block-ul nu există");

        block.Data = JsonSerializer.Serialize(dto.Data);
        await _lessonBlockRepository.UpdateLessonBlockAsync(block);
    }

    public async Task DeleteLessonBlock(int blockId)
    {
        var block = await _lessonBlockRepository.GetLessonBlockByIdAsync(blockId)
        ?? throw new Exception("Block-ul nu există");

        await _lessonBlockRepository.DeleteLessonBlockAsync(block);
    }

    public async Task ReorderBlocksAsync(int lessonId, List<int> orderedBlockIds)
    {
        await _lessonBlockRepository.ReorderLessonBlocksAsync(lessonId, orderedBlockIds);
    }

    public async Task<List<CourseDto>> GetCoursesInReviewAsync()
    {
        var courses = await _courseRepository.GetCoursesInReviewAsync();

        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Price = c.Price,
            Language = c.Language,
            Level = c.Level,
            Status = c.Status.ToString(),
            Version = c.Version,
            VotesUp = c.VotesUp,
            VotesDown = c.VotesDown,
            Score = c.Score,
            TotalVotes = c.TotalVotes,
            CreatedAt = c.CreatedAt,
            CreatorId = c.CreatorId,
            CreatorUsername = c.Creator.Username
        }).ToList();
    }

    public async Task SubmitForReviewAsync(int courseId, int userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId)
            ?? throw new KeyNotFoundException("Cursul nu există.");

        if (course.CreatorId != userId)
            throw new UnauthorizedAccessException("Nu ești creatorul acestui curs.");

        if (course.Status != CourseStatus.Draft && course.Status != CourseStatus.Rejected)
            throw new InvalidOperationException("Numai cursurile Draft sau Rejected pot fi trimise la review.");

        course.Status = CourseStatus.InReviewCloud;
        course.ReviewStartedAt = DateTime.UtcNow;
        course.ReviewEndsAt = DateTime.UtcNow.AddDays(7);

        await _courseRepository.UpdateCourseAsync(course);
    }
}
