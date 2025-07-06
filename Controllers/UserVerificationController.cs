using Microsoft.AspNetCore.Mvc;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
namespace Arpapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendOtpController : ControllerBase
    {
        [HttpPost]
        public async Task<bool> SendOtp([FromBody] OtpRequest request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Verify your access", "arpitpoddar11@gmail.com"));
                email.To.Add(new MailboxAddress(request.EmailAddress, request.EmailAddress));
                email.Subject = "Test";

                var bodyBuilder = new BodyBuilder { HtmlBody = "your otp :- " + request.Otp };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // **Use STARTTLS for Port 587**
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // **Fix: Disable Certificate Revocation Check**
                smtp.CheckCertificateRevocation = false;

                await smtp.AuthenticateAsync("arpitpoddar11@gmail.com", "jwusvwroqjsddsvo");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        public class OtpRequest
        {
            public string EmailAddress { get; set; }
            // Add these for OTP
            public string Otp { get; set; }
            public DateTime? OtpExpiry { get; set; }
        }
    }

}