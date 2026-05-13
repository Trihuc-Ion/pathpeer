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

        // Add Section
        [HttpPost("{courseId}/sections")]
        [Authorize]
        public async Task<IActionResult> AddSection(int courseId, [FromBody] CreateSectionDto dto)
        {
            var section = await _courseService.AddSection(courseId, dto);
            return Ok(section);
        }

        // Update Section
        [HttpPut("/api/sections/{sectionId}")]
        [Authorize]
        public async Task<IActionResult> UpdateSection(int sectionId, [FromBody] UpdateSectionDto dto)
        {
            try
            {
                var result = await _courseService.UpdateSectionAsync(sectionId, dto);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Delete Section
        [HttpDelete("/api/sections/{sectionId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            try
            {
                await _courseService.DeleteSectionAsync(sectionId);
                return Ok(new { message = "Secțiunea a fost ștearsă!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Reorder Sections
        [HttpPut("{courseId}/sections/reorder")]
        [Authorize]
        public async Task<IActionResult> ReorderSections(int courseId, [FromBody] ReorderDto dto)
        {
            try
            {
                await _courseService.ReorderSectionsAsync(courseId, dto.OrderedIds);
                return Ok(new { message = "Ordine actualizată!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Add Lesson
        [HttpPost("/api/sections/{sectionId}/lessons")]
        [Authorize]
        public async Task<IActionResult> AddLesson(int sectionId, [FromBody] CreateLessonDto dto)
        {
            var lesson = await _courseService.AddLesson(sectionId, dto);
            return Ok(lesson);
        }

        // Update Lesson
        [HttpPut("/api/lessons/{lessonId}")]
        [Authorize]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDto dto)
        {
            try
            {
                var result = await _courseService.UpdateLessonAsync(lessonId, dto);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Delete Lesson
        [HttpDelete("/api/lessons/{lessonId}")]
        [Authorize]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            try
            {
                await _courseService.DeleteLessonAsync(lessonId);
                return Ok(new { message = "Lecția a fost ștearsă!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Reorder Lessons
        [HttpPut("/api/sections/{sectionId}/lessons/reorder")]
        [Authorize]
        public async Task<IActionResult> ReorderLessons(int sectionId, [FromBody] ReorderDto dto)
        {
            try
            {
                await _courseService.ReorderLessonsAsync(sectionId, dto.OrderedIds);
                return Ok(new { message = "Ordine actualizată!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Add Block
        [HttpPost("/api/lessons/{lessonId}/blocks")]
        [Authorize]
        public async Task<IActionResult> AddBlock(int lessonId, CreateLessonBlockDto dto)
        {
            await _courseService.AddLessonBlock(lessonId, dto);
            return Ok();
        }

        // Update Block
        [HttpPut("/api/blocks/{blockId}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlock(int blockId, [FromBody] UpdateLessonBlockDto dto)
        {
            try
            {
                await _courseService.UpdateLessonBlock(blockId, dto);
                return Ok(new { message = "Block actualizat!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Delete Block
        [HttpDelete("/api/blocks/{blockId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlock(int blockId)
        {
            try
            {
                await _courseService.DeleteLessonBlock(blockId);
                return Ok(new { message = "Block șters!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Reorder Blocks
        [HttpPut("/api/lessons/{lessonId}/blocks/reorder")]
        [Authorize]
        public async Task<IActionResult> ReorderBlocks(int lessonId, [FromBody] ReorderDto dto)
        {
            try
            {
                await _courseService.ReorderBlocksAsync(lessonId, dto.OrderedIds);
                return Ok(new { message = "Ordine actualizată!" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // Update Course
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _courseService.UpdateCourseAsync(id, dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get Blocks
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

        // Delete Course
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _courseService.DeleteCourseAsync(id, userId);
                return Ok(new { message = "Cursul a fost șters!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
