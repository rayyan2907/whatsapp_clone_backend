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

        public async Task<bool> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                // Load HTML template
                string body = $@"

                    <!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Verify Your Account</title>
                        <style>
                            @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

                            * {{
                                margin: 0;
                                padding: 0;
                                box-sizing: border-box;
                            }}

                            body {{
                                font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                background: #0a0a0a;
                                min-height: 100vh;
                                padding: 20px;
                                color: #333;
                            }}

                            .email-container {{
                                max-width: 600px;
                                margin: 0 auto;
                                background: #1a1a1a;
                                border-radius: 24px;
                                box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
                                overflow: hidden;
                                animation: slideIn 0.8s ease-out;
                                border: 1px solid #2a2a2a;
                            }}

                            @keyframes slideIn {{
                                from {{
                                    opacity: 0;
                                    transform: translateY(30px);
                                }}

                                to {{
                                    opacity: 1;
                                    transform: translateY(0);
                                }}
                            }}

                            .header {{
                                background: #128C7E;
                                padding: 40px 30px;
                                text-align: center;
                                position: relative;
                                overflow: hidden;
                            }}

                                .header::before {{
                                    content: '';
                                    position: absolute;
                                    top: -50%;
                                    left: -50%;
                                    width: 200%;
                                    height: 200%;
                                    background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><defs><pattern id=""grain"" width=""100"" height=""100"" patternUnits=""userSpaceOnUse""><circle cx=""25"" cy=""25"" r=""1"" fill=""white"" opacity=""0.1""/><circle cx=""75"" cy=""25"" r=""1"" fill=""white"" opacity=""0.1""/><circle cx=""50"" cy=""50"" r=""1"" fill=""white"" opacity=""0.1""/><circle cx=""25"" cy=""75"" r=""1"" fill=""white"" opacity=""0.1""/><circle cx=""75"" cy=""75"" r=""1"" fill=""white"" opacity=""0.1""/></pattern></defs><rect width=""100"" height=""100"" fill=""url(%23grain)""/></svg>');
                                    animation: float 20s infinite linear;
                                }}

                            @keyframes float {{
                                0% {{
                                    transform: translateX(-50px) translateY(-50px);
                                }}

                                100% {{
                                    transform: translateX(50px) translateY(50px);
                                }}
                            }}

                            .logo {{
                                width: 80px;
                                height: 80px;
                                background: rgba(255, 255, 255, 0.15);
                                border-radius: 20px;
                                margin: 0 auto 20px;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                backdrop-filter: blur(10px);
                                border: 1px solid rgba(255, 255, 255, 0.2);
                                position: relative;
                                z-index: 1;
                            }}

                                .logo svg {{
                                    width: 40px;
                                    height: 40px;
                                    fill: white;
                                }}

                            .header h1 {{
                                color: white;
                                font-size: 28px;
                                font-weight: 600;
                                margin-bottom: 8px;
                                position: relative;
                                z-index: 1;
                            }}

                            .header p {{
                                color: rgba(255, 255, 255, 0.9);
                                font-size: 16px;
                                font-weight: 300;
                                position: relative;
                                z-index: 1;
                            }}

                            .content {{
                                padding: 50px 40px;
                                text-align: center;
                            }}

                            .greeting {{
                                font-size: 20px;
                                font-weight: 500;
                                color: #ffffff;
                                margin-bottom: 16px;
                            }}

                            .message {{
                                font-size: 16px;
                                color: #a0a0a0;
                                line-height: 1.6;
                                margin-bottom: 40px;
                            }}

                            .otp-container {{
                                background: #2a2a2a;
                                border-radius: 16px;
                                padding: 30px;
                                margin: 30px 0;
                                border: 1px solid #3a3a3a;
                                position: relative;
                                overflow: hidden;
                            }}

                                .otp-container::before {{
                                    content: '';
                                    position: absolute;
                                    top: 0;
                                    left: 0;
                                    right: 0;
                                    height: 2px;
                                    background: #25D366;
                                }}

                            .otp-label {{
                                font-size: 14px;
                                color: #888888;
                                text-transform: uppercase;
                                letter-spacing: 1px;
                                font-weight: 500;
                                margin-bottom: 12px;
                            }}

                            .otp-code {{
                                font-size: 36px;
                                font-weight: 700;
                                color: #25D366;
                                letter-spacing: 8px;
                                margin-bottom: 12px;
                                text-shadow: 0 2px 4px rgba(37, 211, 102, 0.3);
                            }}

                            .otp-expires {{
                                font-size: 13px;
                                color: #666666;
                                font-style: italic;
                            }}

                            .cta-button {{
                                display: inline-block;
                                background: #25D366;
                                color: white;
                                padding: 16px 32px;
                                border-radius: 12px;
                                text-decoration: none;
                                font-weight: 600;
                                font-size: 16px;
                                transition: all 0.3s ease;
                                box-shadow: 0 4px 15px rgba(37, 211, 102, 0.3);
                                border: none;
                                cursor: pointer;
                            }}

                                .cta-button:hover {{
                                    transform: translateY(-2px);
                                    box-shadow: 0 8px 25px rgba(37, 211, 102, 0.4);
                                    background: #20BA5A;
                                }}

                            .security-note {{
                                background: rgba(37, 211, 102, 0.1);
                                border-left: 4px solid #25D366;
                                padding: 20px;
                                margin: 30px 0;
                                border-radius: 0 12px 12px 0;
                            }}

                                .security-note p {{
                                    color: #cccccc;
                                    font-size: 14px;
                                    margin: 0;
                                    line-height: 1.5;
                                }}

                            .footer {{
                                background: #0f0f0f;
                                padding: 30px 40px;
                                text-align: center;
                                border-top: 1px solid #2a2a2a;
                            }}

                                .footer p {{
                                    color: #888888;
                                    font-size: 14px;
                                    margin-bottom: 16px;
                                    line-height: 1.5;
                                }}

                            .social-links {{
                                display: flex;
                                justify-content: center;
                                gap: 16px;
                                margin-bottom: 20px;
                            }}

                            .social-link {{
                                width: 40px;
                                height: 40px;
                                background: #2a2a2a;
                                border-radius: 10px;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                text-decoration: none;
                                transition: all 0.3s ease;
                            }}

                                .social-link:hover {{
                                    background: #25D366;
                                    transform: translateY(-2px);
                                }}

                                    .social-link:hover svg {{
                                        fill: white;
                                    }}

                                .social-link svg {{
                                    width: 20px;
                                    height: 20px;
                                    fill: #888888;
                                    transition: fill 0.3s ease;
                                }}

                            .copyright {{
                                color: #666666;
                                font-size: 12px;
                            }}

                            @media (max-width: 600px) {{
                                body {{
                                    padding: 10px;
                                }}

                                .content {{
                                    padding: 30px 20px;
                                }}

                                .header {{
                                    padding: 30px 20px;
                                }}

                                .otp-code {{
                                    font-size: 28px;
                                    letter-spacing: 4px;
                                }}

                                .footer {{
                                    padding: 20px;
                                }}
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""email-container"">
                            <div class=""header"">
            
                                <h1>Account Verification</h1>
                                <p>Complete your sign-up process</p>
                            </div>

                            <div class=""content"">
                                <div class=""greeting"">Hello there! 👋</div>
                                <div class=""message"">
                                    We're excited to have you on board. To complete your account setup and ensure your security, please verify your email address using the code below.
                                </div>

                                <div class=""otp-container"">
                                    <div class=""otp-label"">Your Verification Code</div>
                                    <div class=""otp-code"">{otp}</div>
                                    <div class=""otp-expires"">This code expires in 5 minutes</div>
                                </div>


                                <div class=""security-note"">
                                    <p>
                                        <strong>Security Notice:</strong> Never share this code with anyone. Our team will never ask for your verification code. If you didn't request this code, please ignore this email or contact our support team.
                                    </p>
                                </div>
                            </div>

                            <div class=""footer"">
                                <p>Need help? Contact our support team at <strong>mrayyan403@gmail.com</strong></p>

            

                                <div class=""copyright"">
                                    © 2025 Whatsapp. All rights reserved.
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";

                // Build email
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("WhatsApp", _config["EmailSettings:Username"]));

                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Your OTP Verification Code";

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                // SMTP settings
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");

                return false;
            }
        }
        
    }
}
