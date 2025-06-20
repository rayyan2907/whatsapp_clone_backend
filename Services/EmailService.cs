using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace whatsapp_clone_backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                // Load HTML template
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Email_templates", "otp_template.html");
                string htmlBody = File.ReadAllText(templatePath).Replace("{{OTP}}", otp);

                // Build email
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("WhatsApp", _config["EmailSettings:Username"]));

                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Your OTP Verification Code";

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                email.Body = bodyBuilder.ToMessageBody();

                // SMTP settings
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return "done";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");

                return ex.Message;
            }
        }
        
    }
}
