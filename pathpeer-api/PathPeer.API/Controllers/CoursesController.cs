using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.API.Controllers
{
    [ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _courseService.CreateCourseAsync(dto, userId);
        return Ok(result);
    }

    [HttpPost("{courseId}/sections")]
    public async Task<IActionResult> AddSection(int courseId, CreateSectionDto dto)
    {
        var section = await _courseService.AddSection(courseId, dto);
        return Ok(section);
    }

    // 🔥 Add Lesson
    [HttpPost("/api/sections/{sectionId}/lessons")]
    public async Task<IActionResult> AddLesson(int sectionId, CreateLessonDto dto)
    {
        var lesson = await _courseService.AddLesson(sectionId, dto);
        return Ok(lesson);
    }

    // 🔥 Add Block
    [HttpPost("/api/lessons/{lessonId}/blocks")]
    public async Task<IActionResult> AddBlock(int lessonId, CreateLessonBlockDto dto)
    {
        await _courseService.AddLessonBlock(lessonId, dto);
        return Ok();
    }

    // 🔥 Get Blocks
    [HttpGet("/api/lessons/{lessonId}/blocks")]
    public async Task<IActionResult> GetBlocks(int lessonId)
    {
        var result = await _courseService.GetLessonBlocks(lessonId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var result = await _courseService.GetCoursesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(int id)
    {
        var result = await _courseService.GetCourseByIdAsync(id);
        return Ok(result);
    }
}
}
