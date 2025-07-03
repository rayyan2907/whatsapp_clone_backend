using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;
using static System.Net.Mime.MediaTypeNames;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    [Route("message")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly Message_DL _msg_dl;

        public MessageController (Message_DL msg_dl)
        {
            _msg_dl = msg_dl;
        }


        [HttpGet]
        [Route("getMessage")]
        public IActionResult getMessage([FromQuery] int id, [FromQuery] int ofsset)
        {
            Console.WriteLine("messages called");
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            int user_id = int.Parse(userIdClaim.Value);
            if (user_id == null)
                return Unauthorized("You have been Logged Out.");


            var messages = _msg_dl.getMessages(user_id, id, ofsset);
            if (messages == null)
            {
                Console.WriteLine("got no messages (null)");
                return Ok(new List<object>()); // Return empty list to prevent Flutter errors
            }
            Console.WriteLine("got messages "+messages.Count);
            return Ok(messages);
        }


    }
}
