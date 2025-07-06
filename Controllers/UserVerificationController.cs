using Microsoft.AspNetCore.Mvc;
using MailKit.Security;
using MimeKit;

namespace Arpapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendOtpController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<OtpResponse>> SendOtp([FromBody] OtpRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.EmailAddress))
                    return BadRequest("Email address is required.");

                var otp = new Random().Next(100000, 999999).ToString();
                var expiry = DateTime.UtcNow.AddMinutes(10);

                Console.WriteLine($"Generated OTP: {otp} for {request.EmailAddress}");


                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Verify your access", "arpitpoddar11@gmail.com"));
                email.To.Add(new MailboxAddress(request.EmailAddress, request.EmailAddress));
                email.Subject = "Your OTP Code for verification";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"<p>Your OTP is: <strong>{otp}</strong><br/>It is valid for 10 minutes.</p>"
                };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.CheckCertificateRevocation = false;

                await smtp.AuthenticateAsync("arpitpoddar11@gmail.com", "jwusvwroqjsddsvo");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return Ok(new OtpResponse
                {
                    Otp = otp,
                    Expiry = expiry
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return StatusCode(500, "Failed to send OTP.");
            }
        }

        public class OtpRequest
        {
            public string EmailAddress { get; set; }
        }

        public class OtpResponse
        {
            public string Otp { get; set; }
            public DateTime Expiry { get; set; }
        }
    }
}
