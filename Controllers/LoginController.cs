using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx;
using System.Security.Claims;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace whatsapp_clone_backend.Controllers
{


    [ApiController]
    [Route("whatsapp")]
    public class LoginController : ControllerBase
    {
        private readonly Login_DL _login;
        private readonly IConfiguration _config;

        public LoginController (Login_DL login, IConfiguration config)
        {
            _login= login;
            _config = config;

        }

        
        [HttpPost]
        [Route("login")]
        public IActionResult Login(Login_model model)
        {
            Console.WriteLine($"request from user {model.email} recieved");
            bool isValiduser = _login.checkUser(model.email);
            if (isValiduser)
            {
                bool IsPwdValid = _login.checkPassword(model);

                if (IsPwdValid)
                {
                    User_Model user = _login.GetUser(model);
                    var token = Jwt_service.GenerateJWTToken(user, _config);
                    return Ok(new
                    {
                        token = token,
                        user = new
                        {
                            user.user_id,
                            user.first_name,
                            user.last_name,
                            user.email,
                            user.profile_pic_url,
                            user.date_of_birth
                        }
                    });
                }
                else
                {
                    return BadRequest("Password is incorrect");
                }
            }
            else
            {
                return BadRequest("No user found");
            }
        }


        [Authorize]
        [HttpPost]
        [Route("test")]
        public IActionResult GetData([FromBody] string request)
        {
            return Ok($"{request} is sent");
        }

    }
}
