using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;

namespace whatsapp_clone_backend.Controllers
{
   
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly Contact_DL _contactdl;

        public ContactsController(Contact_DL contactdl)
        {
            _contactdl = contactdl;
        }

        [HttpGet]
        [Route("getContacts")]
        public IActionResult getContacts()
        {
            Console.WriteLine("fumction called");
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            int user_id = int.Parse(userIdClaim.Value);
            if (user_id == null)
                return Unauthorized("You have been Logged Out.");


            var contacts = _contactdl.getContacts(user_id);
            return Ok(contacts);
        }
    }
}
