using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using TerritoryTools.Web.Data;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Services;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class HomeController : AuthorizedController
    {
        private readonly IUserService _userService;
        private readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly Services.IQRCodeActivityService qrCodeActivityService;

        public HomeController(
            IUserService userService,
            ICombinedAssignmentService combinedAssignmentService,
            IAlbaAuthClientService albaAuthClientService,
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
            Services.IQRCodeActivityService qrCodeActivityService,
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                userService,
                authorizationService,
                optionsAccessor)
        {
            _userService = userService;
            _combinedAssignmentService = combinedAssignmentService;
            this.qrCodeActivityService = qrCodeActivityService;
        }

        public IActionResult Index()
        {
            try
            {
                var publisher = new Publisher()
                {
                    Email = User.Identity.Name
                };

                if (!IsUser())
                {
                    return View(publisher);
                }

                publisher.IsAdmin = IsAdmin();

                string myName = User.Identity.Name;

                try
                {
                    var users = _userService.GetUsers(User.Identity.Name);
                    var me = users.FirstOrDefault(
                        u => string.Equals(
                            u.Email,
                            User.Identity.Name,
                            StringComparison.OrdinalIgnoreCase));

                    if(me == null)
                    {
                        throw new Exception($"Email not found in users table that matches {User.Identity.Name}");
                    }

                    myName = me.Name;
                }
                catch(Exception e)
                {
                     return NotFound(e.Message);
                }

                var allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
                var assignments = allAssignments.Rows
                    .Where(a => string.Equals(a.SignedOutTo, myName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                publisher.PhoneTerritorySuccess = allAssignments.PhoneSuccess;

                publisher.Name = myName;

                foreach (var item in assignments.OrderByDescending(a => a.SignedOut))
                {
                    publisher.Territories.Add(item);
                }

                var qrCodeHits = qrCodeActivityService.QRCodeHitsForUser(
                    publisher.Email);

                foreach (var hit in qrCodeHits)
                {
                    publisher.QRCodeActivity.Add(
                        new Models.QRCodeHit
                        {
                            Id = hit.Id,
                            ShortUrl = hit.ShortUrl,
                            OriginalUrl = hit.OriginalUrl,
                            Created = hit.Created.ToString("yyyy-MM-dd HH:mm:ss"),
                            HitCount = hit.HitCount.ToString(),
                            LastIPAddress = hit.LastIPAddress,
                            LastTimeStamp = hit.LastTimeStamp?.ToString("yyyy-MM-dd HH:mm:ss"),
                            Subject = hit.Subject,
                            Note = hit.Note
                        });
                }

                return View(publisher);
            }
            catch (Exception e)
            {
                return Redirect($"~/Home/LoginError?message={WebUtility.UrlEncode(e.Message)}");
            }
        }

        public IActionResult LoginError(string message)
        {
            ViewData["ErrorMessage"] = message;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        new public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                }
            );
        }

        [Route("/Cookies")]
        public IActionResult Cookies()
        {
            return View();
        }

        [Route("/DeleteCookies")]
        public IActionResult DeleteCookies()
        {
            /*
            string domain = "territorytools.org";
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
        
                // SameSite.None Cookies won't be accepted by Google Chrome and other modern browsers if they're not secure, which would lead in a "non-deletion" bug.
                // in this specific scenario, we need to avoid emitting the SameSite attribute to ensure that the cookie will be deleted.
                if (cookie.SameSite == SameSiteMode.None && !cookie.Secure)
                    cookie.SameSite = (SameSiteMode)(-1);
        
                if (String.IsNullOrEmpty(keyName))
                {
                    cookie.Expires = DateTime.UtcNow.AddYears(-1);
                    if (!String.IsNullOrEmpty(domain)) cookie.Domain = domain;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    //HttpContext.Current.Request.Cookies.Remove(cookieName);
                }
                else
                {
                    cookie.Values.Remove(keyName);
                    if (!String.IsNullOrEmpty(domain)) cookie.Domain = domain;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }*/

            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Console.WriteLine($"Request Cookie");
                Console.WriteLine($"Key: {cookie.Key}");
                Console.WriteLine($"Value: {cookie.Value}");
                Response.Cookies.Delete(cookie.Key);
                // Console.WriteLine($"Domain: {cookie.Domain}");
                // Console.WriteLine($"Key: {cookie.Key}");
                // Console.WriteLine($"Expires: {cookie.Expires.ToString()}");
                //HttpContext.Current.Request.Cookies.Delete(cookie.Key);
            }

            // foreach (var cookie in HttpContext.Response.Cookies)
            // {
            //     Console.WriteLine($"Response Cookie");
            //     Console.WriteLine($"Domain: {cookie.Domain}");
            //     Console.WriteLine($"Key: {cookie.Key}");
            //     Console.WriteLine($"Expires: {cookie.Expires.ToString()}");
            //     //Response.Cookies.Delete(cookie.Key);
            // }

            // foreach (var cookie in HttpContext.Response.Cookies)
            // {
            //     Response.Cookies.Delete(cookie.Key);
            // }

            return View();
        }

        [Authorize]
        [Route("t/{number}")]
        public IActionResult AssignSingleTerritory(string number)
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var users = _userService.GetUsers(User.Identity.Name)
                    .OrderBy(u => u.Name)
                    .ToList();

                var allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
                var assignment = allAssignments.Rows
                    .Where(a => string.Equals(
                        a.Number,
                        number,
                        StringComparison.OrdinalIgnoreCase))
                    .SingleOrDefault();

                if (assignment == null)
                {
                    return NotFound();
                }

                var form = new AssignSingleTerritoryForm()
                {
                    // TODO: Rename Publishers to Users
                    Users = users,
                    Assignment = assignment
                };

                return View(form);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult Load()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            return View();
        }


        [Authorize]
        public IActionResult LoadUsers()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            _userService.LoadUsers(User.Identity.Name);

            return LocalRedirect("~/Home/Load");
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public IActionResult AssignSuccess(int territoryId, string userName)
        {
            var allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
            var assignment = allAssignments.Rows
               .FirstOrDefault(a => a.Id == territoryId);

            assignment.SignedOutTo = userName;

            return View(assignment);
        }

        [Authorize]
        public IActionResult UnassignSuccess(int territoryId)
        {
            var allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
            var assignment = allAssignments.Rows
               .FirstOrDefault(a => a.Id == territoryId);

            return View(assignment);
        }
    }
}
