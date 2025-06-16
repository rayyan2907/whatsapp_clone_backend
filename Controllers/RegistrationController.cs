using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mysqlx;
using Org.BouncyCastle.Ocsp;
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
        private readonly EmailService _email;
        private readonly IMemoryCache _cache;

        public RegistrationController(Registration_DL registration_DL,EmailService email,IMemoryCache cache)
        {
            _reg_DL = registration_DL;
            _email = email;
            _cache = cache;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(Registration_model reg)
        {
            //check if user already exits

            bool exits = _reg_DL.checkUser(reg.email);
            if(exits)
            {
                return BadRequest("User already Exists");
            }


            var otp = new Random().Next(100000, 999999).ToString(); // generate 6-digit OTP
            bool sent = _email.SendOtpEmail(reg.email, otp);
            

            if (!sent)
            {
                return StatusCode(500, "Failed to send OTP. Try again.");
            }
            _cache.Set($"otp_{reg.email}", otp, TimeSpan.FromMinutes(5));
            _cache.Set($"user_{reg.email}", reg, TimeSpan.FromMinutes(5));


            return Ok("OTP sent to your email");         



        }

        [HttpPost]
        [Route("enterotp")]
        public IActionResult checkOtp([FromForm] string email, [FromForm] string otp)
        {
            string? cachedOtp = _cache.Get<string>($"otp_{email}");

            if (cachedOtp == null || cachedOtp != otp)
                return BadRequest("Invalid or expired OTP");

            var cachedUser = _cache.Get<Registration_model>($"user_{email}");
            if (cachedUser == null)
                return BadRequest("User data expired");          


            _cache.Remove($"otp_{email}");
            _cache.Remove($"user_{email}");
            
            var Reg_dto = new
            {
                first_name = cachedUser.first_name,
                last_name = cachedUser.last_name,
                password = cachedUser.password,
                date_of_birth = cachedUser.date_of_birth,
                email = cachedUser.email,
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

        [HttpPost]
        [Route("setdp")]
        public async Task<IActionResult> uploadDP(IFormFile file,string email)
        {
            string url;
            if (file == null || file.Length == 0)
            {
                url = null;
            }
            else
            {
                url = await _azure.UploadProfilePic(file);
            }

            if (url == null)
            {
                return Ok("No Profile Photo Added");
            }

            else
            {
                bool isDpUpload=_reg_DL.addProfilePhoto(url,email);
                if (isDpUpload)
                {
                    return Ok("Profile Photo Uploaded");
                }
                else
                {
                    return BadRequest("Error in uploading profile photo");
                }

            }
        }
    
    }
}
