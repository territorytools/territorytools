using System;
using TerritoryTools.Entities;
using WebUI.Areas.Identity.Data;
using WebUI.Areas.UrlShortener.Models;

namespace WebUI.Areas.UrlShortener.Services
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
    }
}
