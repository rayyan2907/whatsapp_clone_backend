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
        public IActionResult getContacts([FromQuery] int id)
        {
            var contacts = _contactdl.getContacts(id);
            return Ok(contacts);
        }
    }
}
