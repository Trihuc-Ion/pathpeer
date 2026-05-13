using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.API.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET api/enrollments/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _enrollmentService.GetUserEnrollmentsAsync(userId);
            return Ok(result);
        }

        // POST api/enrollments/{courseId}
        [HttpPost("{courseId}")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _enrollmentService.EnrollAsync(userId, courseId);
                return Ok(new { message = "Înscris cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/enrollments/{courseId}
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _enrollmentService.UnenrollAsync(userId, courseId);
                return Ok(new { message = "Dezinscris cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/enrollments/{courseId}/status
        [HttpGet("{courseId}/status")]
        public async Task<IActionResult> GetEnrollmentStatus(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isEnrolled = await _enrollmentService.IsEnrolledAsync(userId, courseId);
            return Ok(new { isEnrolled });
        }
    }
}
