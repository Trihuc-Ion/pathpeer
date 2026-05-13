using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Features.Courses.DTOs;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.API.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/votes")]
    [Authorize]
    public class CourseVoteController : ControllerBase
    {
        private readonly ICourseVoteService _voteService;
        public CourseVoteController(ICourseVoteService voteService)
        {
            _voteService = voteService;
        }

        // GET api/courses/{courseId}/votes
        [HttpGet]
        public async Task<IActionResult> GetVotes(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _voteService.GetVotesAsync(userId, courseId);
            return Ok(result);
        }

        // POST api/courses/{courseId}/votes
        [HttpPost]
        public async Task<IActionResult> Vote(int courseId, [FromBody] VoteDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _voteService.VoteAsync(userId, courseId, dto.Type);
                return Ok(new { message = "Vot înregistrat!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/courses/{courseId}/votes
        [HttpDelete]
        public async Task<IActionResult> RemoveVote(int courseId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _voteService.RemoveVoteAsync(userId, courseId);
                return Ok(new { message = "Vot eliminat!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
