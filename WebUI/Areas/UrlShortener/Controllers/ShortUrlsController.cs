using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
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
        public IActionResult Create(string originalUrl, string subject, string note)
        {
            if (!IsUser())
            {
                return Forbid();
            }

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                Subject = subject,
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

            ViewData["HostName"] = _hostName ?? Request.Host.ToString();
            ViewData["Path"] = AlphaNumberId.ToAlphaNumberId(url.Id);

            return View(url);
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
