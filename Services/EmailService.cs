using System.Net;
using System.Net.Mail;

namespace whatsapp_clone_backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        public bool SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Email_templates", "otp_template.html");
                string htmlBody = File.ReadAllText(templatePath);
                htmlBody = htmlBody.Replace("{{OTP}}", otp);

                var smtpSettings = _config.GetSection("EmailSettings");
                var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
                {
                    Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
                };

                var message = new MailMessage
                {
                    From = new MailAddress(smtpSettings["Username"], "WhatsApp - OTP Verification"),
                    Subject = "Your OTP Verification Code",
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email: " + ex.Message);
                    return false;
                }
            }
            catch
            {
                return false;
            }

          
        }
    }
}
