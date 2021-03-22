using System.Collections.Generic;
using TerritoryTools.Web.Data.Models;
using WebUI.Areas.UrlShortener.Models;

namespace TerritoryTools.Web.Data.Services
{
    public interface IShortUrlService
    {
        ShortUrl GetById(int id);

        ShortUrl GetByPath(string path, string ip, string host);

        ShortUrl GetByOriginalUrl(string originalUrl);

        ShortUrl GetByPath(string path);

        int Save(ShortUrl shortUrl);

        int Save(ShortUrlCreationRequest shortUrl);

        void Update(ShortUrl shortUrl);

        List<ShortUrlHost> GetHostList();
    }
}
