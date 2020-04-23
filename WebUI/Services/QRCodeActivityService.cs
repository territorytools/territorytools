using System.Collections.Generic;
using System.Linq;
using WebUI.Areas.Identity.Data;

namespace WebUI.Services
{
    public interface IQRCodeActivityService
    {
        List<QRCodeHit> QRCodeHitsForUser(string userName);
    }

    public class QRCodeActivityService : IQRCodeActivityService
    {
        readonly MainDbContext context;

        public QRCodeActivityService(MainDbContext context)
        {
            this.context = context;
        }

        public List<QRCodeHit> QRCodeHitsForUser(string userName)
        {
            return context
                .ShortUrls
                .Where(url => string.Equals(
                    url.UserName,
                    userName,
                    System.StringComparison.OrdinalIgnoreCase))
                .Select(url => new QRCodeHit
                {
                    UserName = url.UserName,
                    Subject = url.Subject,
                    Note = url.Note
                })
                .ToList();
        }
    }

    public class QRCodeHit
    {
        public string ShortUrl { get; set; }
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
    }
}
