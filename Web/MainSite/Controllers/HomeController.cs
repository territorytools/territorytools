using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class HomeController : AuthorizedController
    {
        private readonly IUserFromApiService _userFromApiService;
        private readonly IUserService _userService;
        private readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly Services.IQRCodeActivityService qrCodeActivityService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IUserFromApiService userFromApiService,
            IUserService userService,
            ICombinedAssignmentService combinedAssignmentService,
            Services.IAuthorizationService authorizationService,
            Services.IQRCodeActivityService qrCodeActivityService,
            IConfiguration configuration,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<HomeController> logger) : base(
                userFromApiService,
                userService,
                authorizationService,
                optionsAccessor)
        {
            _userFromApiService = userFromApiService;
            _userService = userService;
            _combinedAssignmentService = combinedAssignmentService;
            this.qrCodeActivityService = qrCodeActivityService;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index(string? impersonate = null)
        {
            try
            {
                var publisher = new Models.Publisher()
                {
                    SharedPhoneTerritoryLink = _configuration.GetValue<string>("SharedPhoneTerritoryLink"),
                    Email = User.Identity.Name,
                    UserSelfCompleteFeatureEnabled = _options.Features.UserSelfComplete
                };

                var user = _userFromApiService.ByEmail(publisher.Email);
                if (user == null || !(user.IsActive ?? false)) //!IsUser())
                {
                    return View(publisher);
                }

                publisher.IsAdmin = user.CanAssignTerritories;

                string myName = User.Identity.Name;

                

                try
                {
                    //var users = _userService.GetUsers(User.Identity.Name);
                    //var me = users.FirstOrDefault(
                    //    u => string.Equals(
                    //        u.Email,
                    //        User.Identity.Name,
                    //        StringComparison.OrdinalIgnoreCase));

                    //if(me == null)
                    //{
                    //    throw new Exception($"Email not found in users table that matches {User.Identity.Name}");
                    //}

                    //myName = me.Name;
                    if (publisher.IsAdmin && !string.IsNullOrWhiteSpace(impersonate))
                    {
                        myName = impersonate;
                    }
                    else
                    {
                        myName = user.AlbaFullName;
                    }
                }
                catch(Exception e)
                {
                    _logger.LogError($"Home.Index: Error: {e.Message}");
                    return NotFound(e.Message);
                }

                /*
                GetAllAssignmentsResult allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
                List<AlbaAssignmentValues> assignments = allAssignments.Rows
                    .Where(a => string.Equals(a.SignedOutTo, myName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                publisher.PhoneTerritorySuccess = allAssignments.PhoneSuccess;
                */

                publisher.Name = myName;

                /*
                foreach (var item in assignments.OrderByDescending(a => a.SignedOut))
                {
                    string oldServer = _configuration.GetValue<string>("OldMobileServerFqdn");
                    string newServer = _configuration.GetValue<string>("NewMobileServerFqdn");
                    // TODO: Get newest, unexpired, link that matches item.Number && item.SignedOutTo == User.Identity.Name (var me)
                    // TODO: ...or get territories from somewhere else with links
                    item.MobileLink = item.MobileLink.Replace($"{oldServer}", $"{newServer}");
                    publisher.Territories.Add(item);
                }
                */

                /*
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
                */

                return View(publisher);
            }
            catch (Exception e)
            {
                return Redirect($"~/Home/LoginError?message={WebUtility.UrlEncode(e.Message)}");
            }
        }

        public IActionResult MyPhoneTerritories()
        {
            try
            {
                var publisher = new Models.Publisher()
                {
                    Email = User.Identity.Name,
                    UserSelfCompleteFeatureEnabled = _options.Features.UserSelfComplete
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

                    if (me == null)
                    {
                        throw new Exception($"Email not found in users table that matches {User.Identity.Name}");
                    }

                    myName = me.Name;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Home.Index: Error: {e.Message}");
                    return NotFound(e.Message);
                }

                /*
                GetAllAssignmentsResult allAssignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
                List<AlbaAssignmentValues> assignments = allAssignments.Rows
                    .Where(a => string.Equals(a.SignedOutTo, myName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                publisher.PhoneTerritorySuccess = allAssignments.PhoneSuccess;
                */

                publisher.Name = myName;

                /*
                foreach (var item in assignments.OrderByDescending(a => a.SignedOut))
                {
                    string oldServer = _configuration.GetValue<string>("OldMobileServerFqdn");
                    string newServer = _configuration.GetValue<string>("NewMobileServerFqdn");
                    // TODO: Get newest, unexpired, link that matches item.Number && item.SignedOutTo == User.Identity.Name (var me)
                    // TODO: ...or get territories from somewhere else with links
                    item.MobileLink = item.MobileLink.Replace($"{oldServer}", $"{newServer}");
                    publisher.Territories.Add(item);
                }
                */

                /*
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
                */

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
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Console.WriteLine($"Request Cookie");
                Console.WriteLine($"Key: {cookie.Key}");
                Console.WriteLine($"Value: {cookie.Value}");
                Response.Cookies.Delete(cookie.Key);
            }

            return View();
        }

        [Authorize]
        [Route("assign/t/{number}")]
        public IActionResult AssignSingleTerritory(string number)
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                _logger.LogError($"Home.AssignSingleTerritory: Number: {number} User: {User.Identity.Name}");

                var users = _userService.GetUsers(User.Identity.Name)
                    .OrderBy(u => u.Name)
                    .ToList();

                var result = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);
                var assignment = result.Rows
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
            catch (Exception e)
            {
                _logger.LogError($"Home.AssignSingleTerritory: Error: {e.Message}");
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
