using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        ILogger<EmailSender> logger;

        public EmailSender(
            IOptions<AuthMessageSenderOptions> optionsAccessor,
            ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            this.logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string emailAddress, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, emailAddress);
        }

        public Task SendEmailAsync(EmailRecipient recipient, string subject, string message)
        {
            string emailAddress = "";
            switch (recipient)
            {
                case EmailRecipient.UserAuthorizer:
                    emailAddress = Options.AuthEmailTo;
                    break;
                default:
                    emailAddress = Options.AuthEmailTo;
                    break;
            }

            return Execute(Options.SendGridKey, subject, message, emailAddress);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            if (email.Contains(',') || email.Contains(';'))
            {
                string[] split = email.Split(
                    new char[] { ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries);

                return Task.Run(() =>
                {
                    foreach (string address in split)
                        ExecuteSingleAddress(apiKey, subject, message, address);

                });
            }
            else
            {
                return ExecuteSingleAddress(apiKey, subject, message, email);
            }
        }

        Task ExecuteSingleAddress(string apiKey, string subject, string message, string email)
        {
            logger.LogInformation($"Preparing to send email to {email}");

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.AuthEmailFromEmailAddress, Options.AuthEmailFromName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));

            logger.LogInformation($"Preparation complete for: {email}\nMessage:\n{message}");

            return client.SendEmailAsync(msg);
        }
    }
}
