using System.Threading.Tasks;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);

        Task SendEmailAsync(EmailRecipient recipient, string subject, string message);
    }
}
