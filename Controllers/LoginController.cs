using Microsoft.AspNetCore.Mvc;
using Mysqlx;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly Login_DL _login;

        public LoginController (Login_DL login)
        {

            _login= login;
        }

        [HttpPost]
        public IActionResult Login(string email)
        {
            bool isValiduser = _login.checkUser(email);
            if (isValiduser)
            {
                return Ok("user exists");

            }
            else
            {
                return BadRequest("No user found");
            }
        }

    }
}
