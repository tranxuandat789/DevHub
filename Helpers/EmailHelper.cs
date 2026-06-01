using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DevHub.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var portStr = _config["Email:SmtpPort"] ?? "587";
            int.TryParse(portStr, out var port);
            if (port == 0) port = 587;

            var username = _config["Email:Username"] ?? "";
            var password = _config["Email:Password"] ?? "";
            var senderName = _config["Email:SenderName"] ?? "DevHub";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                System.Diagnostics.Debug.WriteLine("SMTP credentials are empty. Email was not sent.");
                return;
            }

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(username, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
