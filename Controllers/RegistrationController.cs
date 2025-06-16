using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;

namespace whatsapp_clone_backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly Registration_DL _reg_DL;
        private  Azure_services _azure = new Azure_services();

        public RegistrationController(Registration_DL registration_DL)
        {
            _reg_DL = registration_DL;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] Registration_model reg)
        {
            //check if user already exits

            bool exits = _reg_DL.checkUser(reg.email);
            if(exits)
            {
                return BadRequest("User already Exists");
            }



            string url;
            if (reg.profile_pic == null || reg.profile_pic.Length == 0)
            {
                url = null;
            }
            else
            {
                url = await _azure.UploadProfilePic(reg.profile_pic);
            }

            var Reg_dto = new
            {
                first_name = reg.first_name,
                last_name = reg.last_name,
                password = reg.password,
                profile_pic_url = url,
                date_of_birth = reg.date_of_birth,
                email = reg.email,
            };


            bool isReg = _reg_DL.userReg(Reg_dto);

            if (isReg)
            {
                return Ok("Your account has been created successfully");
            }
            else
            {
                return BadRequest("There is an error in registration. Try again later");

            }

        }
    }
}
