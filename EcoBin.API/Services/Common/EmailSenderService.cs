using EcoBin.API.Interfaces;
using System.Net;
using System.Net.Mail;

namespace EcoBin.API.Services.Common
{
    public class EmailSenderService(IConfiguration configuration, IWebHostEnvironment env) : IServiceInjector
    {

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpUser = configuration["SMTP:SMTPUser"];
            var smtpPassword = configuration["SMTP:SMTPPassword"];
            var smtpServer = configuration["SMTP:SMTPServer"];
            var smtpPort = configuration["SMTP:SMTPPort"];

            if (string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPassword) ||
                string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(smtpPort))
            {
                throw new InvalidOperationException("SMTP configuration is missing or incomplete in appsettings.json.");
            }

            if (!int.TryParse(smtpPort, out var port))
            {
                throw new InvalidOperationException("Invalid SMTP port specified in appsettings.json.");
            }

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(smtpUser, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };


            return smtpClient.SendMailAsync(mailMessage);
        }

        public Task SendPasswordResetAsync(string email, string token)
        {
            var templatePath = Path.Combine(env.ContentRootPath, "Templates", "password_reset.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Password reset template not found.", templatePath);

            var emailBody = File.ReadAllText(templatePath);
            emailBody = emailBody.Replace("{{reset_token}}", token);

            return SendEmailAsync(email, "Password Reset", emailBody);
        }

    }


}
