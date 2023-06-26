using Microsoft.Extensions.Options;
using Nodus.NotificaitonService.Options;
using System.Net;
using System.Net.Mail;

namespace Nodus.NotificaitonService.Services
{
    public class EmailService
    {
        private readonly SmtpOptions _smtpOptions;
        public EmailService(IOptions<SmtpOptions> options)
        {
            _smtpOptions = options.Value;
        }
        public async Task<SendEmailResponse> SendEmail(EmailRequest request)
        {
            try
            {
                var client = new SmtpClient(_smtpOptions.Address)
                {
                    Port = _smtpOptions.Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = _smtpOptions.EnableSSL,
                    Credentials = new NetworkCredential(_smtpOptions.Email, _smtpOptions.AppPassword),

                };
                var mailMessage = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress(_smtpOptions.Email),
                    Subject = request.Subject,
                    Body = request.Message
                };
                mailMessage.To.Add(request.Email);
                await client.SendMailAsync(mailMessage);
                return new SendEmailResponse
                {
                    Success = true,
                    Message = "Email was sent successfully"
                };
            }
            catch (Exception e)
            {
                return new SendEmailResponse
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }
    }


}
