using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    public class ShortUrlsController : Controller
    {
        private readonly IShortUrlService _service;

        public ShortUrlsController(IShortUrlService service)
        {
            _service = service;
        }

        [HttpGet("/{path:required}")]
        public IActionResult RedirectTo(string path)
        {
            if (path == null) 
            {
                return NotFound();
            }

            var shortUrl = _service.GetByPath(path);
            if (shortUrl == null) 
            {
                return NotFound();
            }

            return Redirect(shortUrl.OriginalUrl);
        }
    }
}
