using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using WebUI.Services;
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
using TerritoryTools.Vault;
using WebUI.Areas.Identity.Data;
using System.Linq;

namespace WebUI.Controllers
{
    public class AuthorizedController : Controller
    {
        string k1MagicString = LogUserOntoAlba.k1MagicString;

        protected readonly IStringLocalizer<AuthorizedController> localizer;
        protected readonly MainDbContext database;
        protected string account;
        protected string user;
        protected string password;
        protected WebUI.Services.IAuthorizationService authorizationService;

        public AuthorizedController(
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.database = database;
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

        protected void LoadAssignmentData()
        {
            if (io.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                io.File.Delete(options.AlbaAssignmentsHtmlPath);
            }

            var client = AuthClient();

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

            var client = AuthClient();

            var vault = new AzureKeyVaultClient(
                clientId: options.AzureAppId,
                clientSecret: options.AzureClientSecret,
                "territorywebvault");

            string currentUserName = User.Identity.Name.ToUpper();
            var identityUser = database
                .Users
                .SingleOrDefault(u => u.NormalizedEmail == currentUserName);

            var territoryUser = database
                .TerritoryUser
                .SingleOrDefault(u => u.AspNetUserId == identityUser.Id);

            var accountLink = database
                .TerritoryUserAlbaAccountLink
                .FirstOrDefault(l => l.TerritoryUserId == territoryUser.Id);

            string id = accountLink.AlbaAccountId.ToString();

            string acct = vault.GetSecret($"alba-account-name-{id}");
            string usr = vault.GetSecret($"alba-account-user-{id}");
            string pwd = vault.GetSecret($"alba-account-password-{id}");

            //var credentials = new Credentials(account, user, password, k1MagicString);
            var credentials = new Credentials(acct, usr, pwd, k1MagicString);

            client.Authorize(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            io.File.WriteAllText(options.AlbaUsersHtmlPath, usersHtml);
        }

        protected void LoadUserManagementData()
        {
            if (io.File.Exists(options.AlbaUserManagementHtmlPath))
            {
                io.File.Delete(options.AlbaUserManagementHtmlPath);
            }

            var client = AuthClient();

            var credentials = new Credentials(account, user, password, k1MagicString);

            client.Authorize(credentials);

            var json = client.DownloadString(
                RelativeUrlBuilder.GetUserManagementPage());

            string html = AlbaJsonResultParser.ParseDataHtml(json, "users");

            io.File.WriteAllText(options.AlbaUserManagementHtmlPath, html);
        }

        protected IEnumerable<Assignment> GetAllAssignments()
        {
            if (!io.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                LoadAssignmentData();
            }

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(AuthClient());

            string html = io.File.ReadAllText(options.AlbaAssignmentsHtmlPath);

            return useCase.GetAssignments(html);
        }

        protected IEnumerable<cuc.User> GetUsers(string account, string user, string password)
        {
            var users = new List<cuc.User>();

            if (io.File.Exists(options.AlbaUsersHtmlPath))
            {
                string html = io.File.ReadAllText(options.AlbaUsersHtmlPath);
                users = DownloadUsers.GetUsers(html);
            }
                
            var adminUserNames = authorizationService.GetAdminUsers();
            foreach (var name in adminUserNames)
            {
                users.Add(
                    new cuc.User
                    {
                        Email = name,
                        Name = name,
                    });
            }

            var userNames = authorizationService.GetUsers();
            foreach (var name in userNames)
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

        protected IEnumerable<AlbaUserView> GetAlbaUsers()
        {
            if (!io.File.Exists(options.AlbaUserManagementHtmlPath))
            {
                LoadUserManagementData();
            }

            string html = io.File.ReadAllText(options.AlbaUserManagementHtmlPath);

            var users = DownloadUserManagementData.GetUsers(html);

            var albaUsers = new List<AlbaUserView>();
            foreach (var u in users)
            {
                albaUsers.Add(
                    new AlbaUserView
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role,
                        Telephone = u.Telephone,
                        Created = u.Created,
                    }
                );
            }

            return albaUsers;
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

        static AuthorizationClient AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "www.alba-website-here.com",
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            return client;
        }
    }
}
