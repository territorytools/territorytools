using System;
using TerritoryTools.Entities;
using WebUI.Areas.Identity.Data;
using WebUI.Areas.UrlShortener.Models;

namespace UrlShortener.Services
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

        public ShortUrl GetByPath(string path)
        {
            int id = AlphaNumberId.ToIntegerId(path);
            var activity = new ShortUrlActivity
            {
                ShortUrlId = id,
                TimeStamp = DateTime.Now
            };

            context.ShortUrlActivity.Add(activity);
            context.SaveChanges();

            return context.ShortUrls.Find(id);
        }

        public ShortUrl GetByOriginalUrl(string originalUrl)
        {
            foreach (var shortUrl in context.ShortUrls) {
                if (shortUrl.OriginalUrl == originalUrl) {
                    return shortUrl;
                }
            }

            return null;
        }

        public int Save(ShortUrl shortUrl)
        {
            context.ShortUrls.Add(shortUrl);
            context.SaveChanges();

            return shortUrl.Id;
        }
    }
}
