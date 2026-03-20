using Application.Interfaces.Email;
using Domain.Entities.EmailSetting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class EmailService :IEmailService
    {
        private readonly EmailSetting _setting;

        public EmailService(IOptions<EmailSetting> setting)
        {
            _setting = setting.Value;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            using var client = new SmtpClient(_setting.SmtpHost, _setting.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _setting.SenderEmail,
                    _setting.SenderPassword
                ),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_setting.SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail); // ✅ FIXED
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_setting.SmtpHost, _setting.SmtpPort)
            {
                Credentials = new NetworkCredential(_setting.SenderEmail, _setting.SenderPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_setting.SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            await client.SendMailAsync(mail);
        }
    }
}
