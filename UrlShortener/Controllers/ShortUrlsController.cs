using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    public class ShortUrlsController : Controller
    {
        private readonly IShortUrlService _service;
        private readonly IActionContextAccessor _accessor;

        public ShortUrlsController(
            IShortUrlService service, 
            IActionContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;
        }

        [HttpGet("/{path:required}")]
        public IActionResult RedirectTo(string path)
        {
            if (path == null) 
            {
                return NotFound();
            }

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            var shortUrl = _service.GetByPath(path, ip);
            if (shortUrl == null) 
            {
                return NotFound();
            }

            return Redirect(shortUrl.OriginalUrl);
        }
    }
}
