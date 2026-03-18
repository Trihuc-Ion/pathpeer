using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Services;

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
