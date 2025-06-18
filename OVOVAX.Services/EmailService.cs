using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OVOVAX.Core.Services.Contract;

namespace OVOVAX.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                var subject = "Password Reset Request - OVOVAX";
                var htmlBody = CreatePasswordResetEmailTemplate(resetLink);
                
                return await SendEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                // Log the exception (you can add logging here)
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string firstName)
        {
            try
            {
                var subject = "Welcome to OVOVAX - Registration Successful!";
                var htmlBody = CreateWelcomeEmailTemplate(firstName);
                
                return await SendEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                // Log the exception (you can add logging here)
                Console.WriteLine($"Error sending welcome email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                // Get email configuration from appsettings.json
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var username = _configuration["Email:Username"];
                var password = _configuration["Email:Password"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];

                // Validate configuration
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username) || 
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail))
                {
                    throw new InvalidOperationException("Email configuration is incomplete");
                }

                // Create the email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                // Create HTML body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                // Send the email
                using var client = new SmtpClient();
                
                // Connect to SMTP server
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                
                // Authenticate
                await client.AuthenticateAsync(username, password);
                
                // Send the message
                await client.SendAsync(message);
                
                // Disconnect
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can add logging here)
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string CreatePasswordResetEmailTemplate(string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset - OVOVAX</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        
        <!-- Header -->
        <div style='text-align: center; padding: 20px 0; border-bottom: 2px solid #007bff;'>
            <h1 style='color: #007bff; margin: 0; font-size: 28px;'>🔒 OVOVAX</h1>
            <p style='color: #666; margin: 5px 0; font-size: 16px;'>Secure Authentication System</p>
        </div>

        <!-- Content -->
        <div style='padding: 30px 0;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Password Reset Request</h2>
            
            <p style='color: #555; line-height: 1.6; font-size: 16px;'>
                We received a request to reset your password for your OVOVAX account. 
                If you made this request, please click the button below to reset your password:
            </p>

            <!-- Reset Button -->
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' 
                   style='display: inline-block; background-color: #007bff; color: #ffffff; padding: 12px 30px; 
                          text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>
                    Reset My Password
                </a>
            </div>

            <p style='color: #555; line-height: 1.6; font-size: 14px;'>
                <strong>Important:</strong> This link will expire in 24 hours for security reasons.
            </p>

            <p style='color: #555; line-height: 1.6; font-size: 14px;'>
                If you didn't request this password reset, please ignore this email. 
                Your password will remain unchanged.
            </p>

            <!-- Manual Link -->
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                <p style='color: #666; font-size: 12px; margin: 0;'>
                    If the button doesn't work, copy and paste this link into your browser:
                </p>
                <p style='color: #007bff; font-size: 12px; word-break: break-all; margin: 5px 0 0 0;'>
                    {resetLink}
                </p>
            </div>
        </div>

        <!-- Footer -->
        <div style='border-top: 1px solid #ddd; padding: 20px 0; text-align: center;'>
            <p style='color: #999; font-size: 12px; margin: 0;'>
                This is an automated message from OVOVAX System. Please do not reply to this email.
            </p>
            <p style='color: #999; font-size: 12px; margin: 5px 0 0 0;'>
                © 2025 OVOVAX. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        private string CreateWelcomeEmailTemplate(string firstName)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to OVOVAX</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        
        <!-- Header -->
        <div style='text-align: center; padding: 20px 0; border-bottom: 2px solid #28a745;'>
            <h1 style='color: #28a745; margin: 0; font-size: 28px;'>🎉 Welcome to OVOVAX!</h1>
            <p style='color: #666; margin: 5px 0; font-size: 16px;'>Your Registration was Successful</p>
        </div>

        <!-- Content -->
        <div style='padding: 30px 0;'>
            <h2 style='color: #333; margin-bottom: 20px;'>Hello {firstName}!</h2>
            
            <p style='color: #555; line-height: 1.6; font-size: 16px;'>
                Thank you for joining OVOVAX! Your account has been successfully created and you're now part of our secure authentication system.
            </p>

            <div style='background-color: #e8f5e8; padding: 20px; border-radius: 8px; margin: 25px 0;'>
                <h3 style='color: #28a745; margin: 0 0 15px 0; font-size: 18px;'>✅ What's Next?</h3>
                <ul style='color: #555; margin: 0; padding-left: 20px;'>
                    <li style='margin-bottom: 8px;'>Your account is ready to use</li>
                    <li style='margin-bottom: 8px;'>You can now log in to access all features</li>
                    <li style='margin-bottom: 8px;'>Keep your credentials secure</li>
                    <li style='margin-bottom: 8px;'>Contact support if you need any help</li>
                </ul>
            </div>

            <p style='color: #555; line-height: 1.6; font-size: 16px;'>
                If you have any questions or need assistance, don't hesitate to contact our support team.
            </p>

            <!-- CTA Button -->
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://localhost:5173/login' 
                   style='display: inline-block; background-color: #28a745; color: #ffffff; padding: 12px 30px; 
                          text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>
                    Login to Your Account
                </a>
            </div>
        </div>

        <!-- Footer -->
        <div style='border-top: 1px solid #ddd; padding: 20px 0; text-align: center;'>
            <p style='color: #999; font-size: 12px; margin: 0;'>
                This is an automated message from OVOVAX System. Please do not reply to this email.
            </p>
            <p style='color: #999; font-size: 12px; margin: 5px 0 0 0;'>
                © 2025 OVOVAX. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
