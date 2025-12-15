using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Services;
using TelimAPI.Infrastructure.Options;

namespace TelimAPI.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(to);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Username,
                    _settings.Password
                ),
                EnableSsl = _settings.EnableSsl
            };

            await client.SendMailAsync(message);
        }
    }
}
