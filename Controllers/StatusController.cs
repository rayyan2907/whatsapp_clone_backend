using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Hubs;

namespace whatsapp_clone_backend.Controllers
{
   
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        [HttpGet("is-online/{userId}")]
        public IActionResult IsOnline(string userId)
        {
            var isOnline = StatusHub.IsUserOnline(userId);
            Console.WriteLine(isOnline);
            return Ok(new { userId, isOnline });
        }
    }
}
