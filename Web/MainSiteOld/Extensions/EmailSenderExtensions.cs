using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(
            this IEmailSender emailSender,
            string userEmail,
            string link)
        {
            return emailSender.SendEmailAsync(
                emailAddress: userEmail,
                subject: $"Confirm your email",
                message:
                    $"<p>Please confirm your email address by clicking this link: " +
                    $"<a href='{HtmlEncoder.Default.Encode(link)}'>link</a></p>");
        }

        public static Task SendUserVerificationAsync(
            this IEmailSender emailSender,
            string userEmail,
            string link)
        {
            return emailSender.SendEmailAsync(
                recipient: EmailRecipient.UserAuthorizer,
                subject: $"Confirm the email account: {userEmail}",
                message:
                    $"<p>Please confirm this account as a user " +
                    $"with edit priviledges by clicking this link: " +
                    $"<a href='{HtmlEncoder.Default.Encode(link)}'>link</a>" +
                    $"</p><p>Otherwise you may just ignore this email.</p>");
        }
    }
}
