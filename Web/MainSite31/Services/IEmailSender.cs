using System.Threading.Tasks;

namespace WebUI.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);

        Task SendEmailAsync(EmailRecipient recipient, string subject, string message);
    }
}
