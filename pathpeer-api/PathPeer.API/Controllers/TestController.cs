using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PathPeer.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public() => Ok("Acest endpoint e public");

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected() => Ok("Acest endpoint necesită JWT");
    }
}
