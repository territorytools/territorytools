using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using AlbaClient;
using AlbaClient.AlbaServer;
using AlbaClient.Controllers.AlbaServer;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using cuc = Controllers.UseCases;
using Controllers.UseCases;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using io = System.IO;
using Microsoft.AspNetCore.Http;
using static WebUI.BasicStrings;

namespace WebUI.Controllers
{
    public class AuthorizedController : Controller
    {
        protected readonly IStringLocalizer<AuthorizedController> localizer;

        protected string account;
        protected string user;
        protected string password;
        protected WebUI.Services.IAuthorizationService authorizationService;

        public AuthorizedController(
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.localizer = localizer;
            account = credentials.Account;
            user = credentials.User;
            password = credentials.Password;
            this.authorizationService = authorizationService;
            options = optionsAccessor.Value;
        }

        readonly WebUIOptions options;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected void LoadData()
        {
            if (io.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                io.File.Delete(options.AlbaAssignmentsHtmlPath);
            }

            //if (io.File.Exists("users.html"))
            //{
            //    io.File.Delete("users.html");
            //}

            string k1MagicString = LogUserOntoAlba.k1MagicString;

            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "www.alba-website-here.com",
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            var credentials = new Credentials(account, user, password, k1MagicString);

            client.Authorize(credentials);

            var assignmentsJson = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string assignmentsHtml = TerritoryAssignmentParser.Parse(assignmentsJson);

            io.File.WriteAllText(
                options.AlbaAssignmentsHtmlPath, 
                assignmentsHtml);

            //var assignedHtml = client.DownloadString(
            //    RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            //string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            //WriteAllText("users.html", assignmentsHtml);
        }

        protected void LoadUserData()
        {
            if (io.File.Exists(options.AlbaUsersHtmlPath))
            {
                io.File.Delete(options.AlbaUsersHtmlPath);
            }

            string k1MagicString = LogUserOntoAlba.k1MagicString;

            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "www.alba-website-here.com",
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            var credentials = new Credentials(account, user, password, k1MagicString);

            client.Authorize(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            io.File.WriteAllText(options.AlbaUsersHtmlPath, usersHtml);
        }

        protected IEnumerable<Assignment> GetAllAssignments(string account, string user, string password)
        {
            if (!io.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                LoadData();
            }

            //string k1MagicString = LogUserOntoAlba.k1MagicString;

            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "www.alba-website-here.com",
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(client); 

            //var credentials = new Credentials(account, user, password, k1MagicString);

            //client.Authorize(credentials);

            //var resultString = client.DownloadString(
            //    RelativeUrlBuilder.GetTerritoryAssignments());

            //html = TerritoryAssignmentParser.Parse(resultString);

            string html = io.File.ReadAllText(options.AlbaAssignmentsHtmlPath);

            return useCase.GetAssignments(html);
        }

        protected IEnumerable<cuc.User> GetUsers(string account, string user, string password)
        {
            if (!io.File.Exists(options.AlbaUsersHtmlPath))
            {
                var adminUserNames = authorizationService.GetAdminUsers();
                var users = new List<cuc.User>();
                foreach (var name in adminUserNames)
                {
                    users.Add(
                        new cuc.User
                        {
                            Email = name,
                            Name = name,
                        });
                }

                return users;
            }

            string html = io.File.ReadAllText(options.AlbaUsersHtmlPath);

            return DownloadUsers.GetUsers(html);
        }

        protected bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                return authorizationService.IsAdmin(User.Identity.Name);
            }

            return false;
        }

        protected bool IsUser()
        {
            var users = GetUsers(account, user, password);

            foreach (var user in users)
            {
                if (StringsEqual(user.Email, User.Identity.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
