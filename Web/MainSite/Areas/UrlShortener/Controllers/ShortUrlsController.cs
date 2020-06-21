using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Ocsp;
using System;
using System.Linq;
using TerritoryTools.Entities;
using WebUI.Areas.Identity.Data;
using WebUI.Areas.UrlShortener.Models;
using WebUI.Areas.UrlShortener.Services;
using WebUI.Controllers;
using WebUI.Services;

namespace WebUI.Areas.UrlShortener.Controllers
{
    [Authorize]
    [Area("UrlShortener")]
    [Route("/UrlShortener/[controller]/[action]")]
    public class ShortUrlsController : AuthorizedController
    {
        private readonly IShortUrlService service;
        private readonly string _hostName;

        public ShortUrlsController(
            MainDbContext database,
            IShortUrlService service, 
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                database,
                localizer,
                credentials,
                authorizationService,
                albaCredentialService,
                optionsAccessor)
        {
            this.service = service;
            _hostName = optionsAccessor.Value.UrlShortenerDomain;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if(!IsUser())
            {
                return Forbid();
            }

            return RedirectToAction(actionName: nameof(Create));
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUser())
            {
                return Forbid();
            }

            return View();
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string originalUrl, 
            string subject, 
            string letterLink, 
            string note)
        {
            if (!IsUser())
            {
                return Forbid();
            }

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                Subject = subject,
                LetterLink = letterLink,
                Note = note,
                UserName = User.Identity.Name,
                Created = DateTime.Now
            };

            TryValidateModel(shortUrl);
            if (ModelState.IsValid)
            {
                service.Save(shortUrl);

                return RedirectToAction(actionName: nameof(Show), routeValues: new { id = shortUrl.Id });
            }

            return View(shortUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ShortUrlUpdateRequest update)
        {
            if (!IsUser())
            {
                return Forbid();
            }

            TryValidateModel(update);
            if (ModelState.IsValid)
            {
                var existing = service.GetById(update.Id);

                existing.Subject = update.Subject;
                existing.LetterLink = update.LetterLink;
                existing.Note = update.Note;
                
                service.Update(existing);

                return RedirectToAction(
                    actionName: nameof(Show), 
                    routeValues: new { id = update.Id });
            }

            return Forbid();
        }

        [HttpGet]
        public IActionResult Show(int? id)
        {
            if (!IsUser())
            {
                return Forbid();
            }

            if (!id.HasValue) 
            {
                return NotFound();
            }

            var url = service.GetById(id.Value);
            if (url == null) 
            {
                return NotFound();
            }

            string hostName = _hostName ?? Request.Host.ToString();
            string path = AlphaNumberId.ToAlphaNumberId(url.Id);
            string shortUrlPath = $"https://{hostName}/{path}";
            //ViewData["HostName"] = hostName;
            //ViewData["Path"] = path;

            var activity = database
                   .ShortUrlActivity
                   .Where(a => a.ShortUrlId == url.Id)
                   .OrderBy(a => a.TimeStamp);

            int hitCount = activity.Count();

            var lastHit = hitCount > 0 
                ? activity.Max(h => h.TimeStamp) 
                : (DateTime?)null;

            var model = new ShortUrlShow
            {
                Id = url.Id,
                OriginalUrl = url.OriginalUrl,
                ShortUrl = shortUrlPath,
                HitCount = hitCount,
                LastHit = lastHit,
                Subject = url.Subject,
                LetterLink = url.LetterLink,
                Note = url.Note
            };

            return View(model);
        }

        [HttpGet("/UrlShortener/ShortUrls/RedirectTo/{path:required}")]
        public IActionResult RedirectTo(string alpha)
        {
            if (alpha == null) 
            {
                return NotFound();
            }

            var url = service.GetByPath(alpha);
            if (url == null) 
            {
                return NotFound();
            }

            return Redirect(url.OriginalUrl);
        }
    }
}
