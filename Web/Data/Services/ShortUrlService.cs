using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data.Models;
using TerritoryTools.Web.MainSite.Areas.Identity.Data;
using TerritoryTools.Web.MainSite.Areas.UrlShortener.Models;

namespace TerritoryTools.Web.Data.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly MainDbContext context;

        public ShortUrlService(MainDbContext context)
        {
            this.context = context;
        }

        public ShortUrl GetById(int id)
        {
            return context.ShortUrls.Find(id);
        }

        public ShortUrl GetByPath(string alpha)
        {
            int id = AlphaNumberId.ToIntegerId(alpha);
            var activity = new ShortUrlActivity
            {
                ShortUrlId = id,
                TimeStamp = DateTime.Now
            };

            context.ShortUrlActivity.Add(activity);
            context.SaveChanges();

            return context.ShortUrls.Find(id);
        }

        public ShortUrl GetByPath(string path, string ip, string host)
        {
            int id = AlphaNumberId.ToIntegerId(path);
            var activity = new ShortUrlActivity
            {
                ShortUrlId = id,
                TimeStamp = DateTime.Now,
                IPAddress = ip
            };

            context.ShortUrlActivity.Add(activity);
            context.SaveChanges();

            var urls = from u in context.ShortUrls
                join h in context.ShortUrlHosts on u.HostId equals h.Id
                where u.Id == id 
                    && string.Equals(h.Name, host, StringComparison.OrdinalIgnoreCase)
                select u;

            return urls.FirstOrDefault();
        }

        public ShortUrl GetByOriginalUrl(string url)
        {
            foreach (var shortUrl in context.ShortUrls) {
                if (shortUrl.OriginalUrl == url) {
                    return shortUrl;
                }
            }

            return null;
        }

        public int Save(ShortUrl url)
        {
            context.ShortUrls.Add(url);
            context.SaveChanges();

            return url.Id;
        }

        public int Save(ShortUrlCreationRequest request)
        {
            ShortUrlHost host = HostByName(request.HostName);

            if (host == null)
            {
                throw new Exception($"Host name '{request.HostName}' does not exist!");
            }

            var url = new ShortUrl
            {
                HostId = host.Id,
                Path = request.Path,
                OriginalUrl = request.OriginalUrl,
                LetterLink = request.LetterLink,
                Note = request.Note,
                UserName = request.UserName,
                Subject = request.Subject,
                Created = DateTime.Now
            };

            context.ShortUrls.Add(url);
            context.SaveChanges();

            return url.Id;
        }

        public List<ShortUrlHost> GetHostList()
        {
            return context
                .ShortUrlHosts
                .Where(u => u.AllowNewUrls)
                .ToList();
        }

        private ShortUrlHost HostByName(string name)
        {
            return context
                .ShortUrlHosts
                .SingleOrDefault(
                    h => string.Equals(
                        h.Name,
                        name,
                        StringComparison.OrdinalIgnoreCase));
        }

        public void Update(ShortUrl shortUrl)
        {
            context.ShortUrls.Update(shortUrl);
            context.SaveChanges();
        }
    }
}
