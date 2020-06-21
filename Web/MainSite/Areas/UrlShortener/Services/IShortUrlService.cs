using WebUI.Areas.UrlShortener.Models;

namespace WebUI.Areas.UrlShortener.Services
{
    public interface IShortUrlService
    {
        ShortUrl GetById(int id);

        ShortUrl GetByPath(string path);

        ShortUrl GetByOriginalUrl(string originalUrl);

        int Save(ShortUrl shortUrl);

        void Update(ShortUrl shortUrl);
    }
}
