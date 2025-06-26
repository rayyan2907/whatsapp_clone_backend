using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    public class SeachController : ControllerBase
    {
        private readonly SearchUser_DL _search;
        public SeachController (SearchUser_DL search)
        {
            _search=search;
        }

        [HttpGet("getUser")]
        [Authorize]
        public IActionResult searchByEmail([FromQuery] string prefix)
        {
            
            if (string.IsNullOrWhiteSpace(prefix))
                return BadRequest(new { message = "Email search query cannot be empty." });

            var users = _search.seacrchByEmail(prefix);
            return Ok(users);
        }
            


    }
}
