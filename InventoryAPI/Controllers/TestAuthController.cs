using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/testauth")]
    public class TestAuthController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
        }

        [HttpGet("secure")]
        [Authorize]
        public IActionResult Secure()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                message = "Secure endpoint accessed",
                userId,
                userEmail,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            // This shows all headers received
            var headers = Request.Headers.Select(h => $"{h.Key}: {h.Value}");
            return Ok(new
            {
                message = "Debug info",
                headers,
                authHeader = Request.Headers["Authorization"].ToString()
            });
        }
    }
}