using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
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
        public async Task<IActionResult> Register(Registration_model reg)
        {
            //check if user already exits

            bool exits = _reg_DL.checkUser(reg.email);
            if(exits)
            {
                return BadRequest(new { message = "User already Exists" });
            }


            var otp = new Random().Next(100000, 999999).ToString(); // generate 6-digit OTP
            bool sent = await _email.SendOtpEmail(reg.email, otp);
            
            
            if (!sent)
            {




                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Quizzy - Modern Quiz System", "mrayyan403@gmail.com"));
                message.To.Add(new MailboxAddress("rayan","mrayyan5296@gmail.com"));
                message.Subject = "Log In Alert - Quizzy";

                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $@"
                    
                            <html>
                            <body style='font-family: Arial, sans-serif; background-color: white; color: #2E2B41; padding: 30px;'>
                                <div style='background-color: #F3F0FF; border-radius: 10px; padding: 30px; max-width: 600px; margin: auto;'>
                                    <h2 style='color: #8672FF; text-align: center;'>Login Alert</h2>
                                    <p>Ytest emeil</b>.</p>
                                    <p>If this wasn't you, please log in and change your password immediately.</p>
                                    <div style='margin-top: 40px; font-size: 12px; color: #999; text-align: center;'>
                                        <footer style='font-size: 12px; color: #888;'>Stay Secure!</footer>
                                        &copy; 2025 Quizzy. All rights reserved.
                                    </div>
                                </div>
                            </body>
                            </html>"
                };
                try
                {
                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        client.Authenticate("mrayyan403@gmail.com", "yuax ekty ofav lkvj"); // your app password
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex });

                    Console.WriteLine("internet issue");

                }











                return StatusCode(500, new { message = "Failed to send OTP. Try again." });
            }
            _cache.Set($"otp_{reg.email}", otp, TimeSpan.FromMinutes(5));
            _cache.Set($"user_{reg.email}", reg, TimeSpan.FromMinutes(5));


            return Ok(new { message = "OTP sent to your email" });         



        }

        [HttpPost]
        [Route("enterotp")]
        public IActionResult checkOtp([FromForm] string email, [FromForm] string otp)
        {
            Console.WriteLine("otp recievd from user is "+otp);
            string? cachedOtp = _cache.Get<string>($"otp_{email}");

            if (cachedOtp == null || cachedOtp != otp)
                return BadRequest(new { message = "Invalid or expired OTP" });

            var cachedUser = _cache.Get<Registration_model>($"user_{email}");
            if (cachedUser == null)
                return BadRequest(new { message = "User data expired" });
            Console.WriteLine("otp in the system is " + cachedOtp);


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
                return Ok(new { message = "Your account has been created successfully" });
            }
            else
            {
                return BadRequest(new { message = "There is an error in registration. Try again later" });

            }
        }

        [HttpPost]
        [Route("setdp")]
        public async Task<IActionResult> uploadDP([FromForm]ProfilePic pic)
        {
            Console.WriteLine("function called");
            string url;
            if (pic.Pic == null || pic.Pic.Length == 0)
            {
                url = null;
            }
            else
            {
                url = await _azure.UploadProfilePic(pic.Pic);
            }

            if (url == null)
            {
                return Ok(new { message = "No Profile Photo Added" });
            }

            else
            {
                bool isDpUpload=_reg_DL.addProfilePhoto(url,pic.email);
                if (isDpUpload)
                {
                    return Ok(new { message = "Profile Photo Uploaded" });
                }
                else
                {
                    return BadRequest(new { message = "Error in uploading profile photo" });
                }

            }
        }
    
    }
}
