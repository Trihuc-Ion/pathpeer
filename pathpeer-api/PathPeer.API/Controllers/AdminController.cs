using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PathPeer.Application.Interfaces.Services;

namespace PathPeer.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("users/{id}/approve-teacher")]
    public async Task<IActionResult> ApproveTeacher(int id)
    {
        try
        {
            await _userService.ApproveTeacherAsync(id);
            return Ok(new { message = "Profesorul a fost aprobat." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("users/{id}/reject-teacher")]
    public async Task<IActionResult> RejectTeacher(int id)
    {
        try
        {
            await _userService.RejectTeacherAsync(id);
            return Ok(new { message = "Cererea a fost respinsă." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}