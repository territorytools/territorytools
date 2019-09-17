using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : AuthorizedController
    {
        // readonly IStringLocalizer<HomeController> localizer;

        // string account;
        // string user;
        // string password;
        // WebUI.Services.IAuthorizationService authorizationService;

        public HomeController(
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                localizer,
                credentials,
                authorizationService,
                optionsAccessor)
        {
            // this.localizer = localizer;
            // account = credentials.Account;
            // user = credentials.User;
            // password = credentials.Password;
            // this.authorizationService = authorizationService;
            // options = optionsAccessor.Value;
        }

        readonly WebUIOptions options;

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

                var users = GetUsers(account, user, password);
                var me = users.FirstOrDefault(u => string.Equals(u.Email, User.Identity.Name, StringComparison.OrdinalIgnoreCase));

                if (me == null)
                {
                    return NotFound();
                }

                string myName = me.Name;

                var assignments = GetAllAssignments(account, user, password)
                    .Where(a => string.Equals(a.SignedOutTo, myName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                publisher.Name = myName;

                foreach (var item in assignments.OrderByDescending(a => a.SignedOut))
                {
                    publisher.Territories.Add(item);
                }

                return View(publisher);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Reports()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            return View();
        }

        [ResponseCache(
            Duration = 0, 
            Location = ResponseCacheLocation.None, 
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                }
            );
        }

        [Authorize]
        public IActionResult NeverCompleted()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var publishers = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var assignments = GetAllAssignments(account, user, password)
                    // Territories never worked
                    .Where(a => a.LastCompleted == null && a.SignedOut == null)
                    .OrderBy(a => a.Description)
                    .ThenBy(a => a.Number)
                    .ToList();

                var report = new NeverCompletedReport()
                {
                    Publishers = publishers,
                    Assignments = assignments
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult Available()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var users = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var assignments = GetAllAssignments(account, user, password)
                    .Where(a => string.Equals(
                        a.Status, 
                        "Available", 
                        StringComparison.OrdinalIgnoreCase)) 
                    .OrderBy(a => a.LastCompleted)
                    .ThenBy(a => a.Number)
                    .ToList();

                var report = new NeverCompletedReport()
                {
                    // TODO: Rename Publishers to Users
                    Publishers = users,
                    Assignments = assignments
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult ByPublisher()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var groups = GetAllAssignments(account, user, password)
                    .Where(a => !string.IsNullOrWhiteSpace(a.SignedOutTo))
                    .GroupBy(a => a.SignedOutTo)
                    .ToList();

                var publishers = new List<Publisher>();
                foreach (var group in groups.OrderBy(g => g.Key))
                {
                    var pub = new Publisher() { Name = group.Key };
                    var ordered = group.OrderByDescending(a => a.SignedOut);
                    foreach (var item in ordered)
                    {
                        pub.Territories.Add(item);
                    }

                    publishers.Add(pub);
                }

                return View(publishers);
            }
            catch (Exception)
            {
                throw;
            }
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

                var users = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var assignment = GetAllAssignments(account, user, password)
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

            LoadUserData();

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
             var assignment = GetAllAssignments(account, user, password)
                    .FirstOrDefault(a => a.Id == territoryId);
            
            assignment.SignedOutTo = userName;

            return View(assignment);
        }

        [Authorize]
        public IActionResult UnassignSuccess(int territoryId)
        {
            var assignment = GetAllAssignments(account, user, password)
                   .FirstOrDefault(a => a.Id == territoryId);

            return View(assignment);
        }
    }
}
