using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new { message = "Public endpoint works" });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            var user = User.Identity?.Name;
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new { message = "Protected endpoint works", user, claims });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok(new { message = "Admin endpoint works" });
        }
    }
}