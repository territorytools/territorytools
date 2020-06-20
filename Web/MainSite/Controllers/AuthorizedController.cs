using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using WebUI.Services;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Alba.Controllers.Models;
using cuc = Controllers.UseCases;
using Controllers.UseCases;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using io = System.IO;
using Microsoft.AspNetCore.Http;
using static WebUI.BasicStrings;
using WebUI.Areas.Identity.Data;
using System;

namespace WebUI.Controllers
{
    public class AuthorizedController : Controller
    {
        protected readonly IStringLocalizer<AuthorizedController> localizer;
        protected readonly MainDbContext database;
        protected string account;
        protected string user;
        protected string password;
        protected IAuthorizationService authorizationService;
        protected IAlbaCredentialService albaCredentialService;

        public AuthorizedController(
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.database = database;
            this.localizer = localizer;
            account = credentials.Account;
            user = credentials.User;
            password = credentials.Password;
            this.authorizationService = authorizationService;
            this.albaCredentialService = albaCredentialService;
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
            var credentials = GetCredentialsFrom(User.Identity.Name);
            string path = string.Format(
                options.AlbaAssignmentsHtmlPath,
                credentials.AlbaAccountId);

            if (io.File.Exists(path))
            {
                io.File.Delete(path);
            }

            var client = AuthClient();
            client.Authenticate(credentials);

            var assignmentsJson = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string assignmentsHtml = TerritoryAssignmentParser.Parse(assignmentsJson);

            if (!io.Directory.Exists(io.Path.GetDirectoryName(path)))
            {
                io.Directory.CreateDirectory(io.Path.GetDirectoryName(path));
            }

            io.File.WriteAllText(path, assignmentsHtml);

            //var assignedHtml = client.DownloadString(
            //    RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            //string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            //WriteAllText("users.html", assignmentsHtml);
        }

        protected void LoadUserData()
        {
            var credentials = GetCredentialsFrom(User.Identity.Name); 
            string path = string.Format(
                options.AlbaUsersHtmlPath, 
                credentials.AlbaAccountId);

            if (io.File.Exists(path))
            {
                io.File.Delete(path);
            }

            var client = AuthClient();
            client.Authenticate(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            if (!io.Directory.Exists(io.Path.GetDirectoryName(path)))
            {
                io.Directory.CreateDirectory(io.Path.GetDirectoryName(path));
            }

            io.File.WriteAllText(path, usersHtml);
        }

        protected void LoadUserManagementData()
        {
            var credentials = GetCredentialsFrom(User.Identity.Name);
            string path = string.Format(
               options.AlbaUserManagementHtmlPath,
               credentials.AlbaAccountId);

            if (io.File.Exists(path))
            {
                io.File.Delete(path);
            }

            var client = AuthClient();
            client.Authenticate(credentials);

            var json = client.DownloadString(
                RelativeUrlBuilder.GetUserManagementPage());

            string html = AlbaJsonResultParser.ParseDataHtml(json, "users");

            if (!io.Directory.Exists(io.Path.GetDirectoryName(path)))
            {
                io.Directory.CreateDirectory(io.Path.GetDirectoryName(path));
            }

            io.File.WriteAllText(path, html);
        }

        protected IEnumerable<Assignment> GetAllAssignments()
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);
            string path = string.Format(
               options.AlbaAssignmentsHtmlPath,
               albaAccountId);

            if (!io.File.Exists(path))
            {
                LoadAssignmentData();
            }

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(AuthClient());

            string html = io.File.ReadAllText(path);

            return useCase.GetAssignments(html);
        }

        protected IEnumerable<cuc.User> GetUsers(string account, string user, string password)
        {
            var users = new List<cuc.User>();

            string currentUser = User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(currentUser);
                string path = string.Format(
                   options.AlbaUsersHtmlPath,
                   albaAccountId);

                if (io.File.Exists(path))
                {
                    string html = io.File.ReadAllText(path);
                    users = DownloadUsers.GetUsers(html);
                }
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

        protected IEnumerable<AlbaUserView> GetAlbaUsers(Guid albaAccountId)
        {
            string path = string.Format(
                options.AlbaUserManagementHtmlPath,
                albaAccountId);

            if (!io.File.Exists(path))
            {
                LoadUserManagementData();
            }

            string html = io.File.ReadAllText(path);

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

        Credentials GetCredentialsFrom(string userName)
        {
            return albaCredentialService.GetCredentialsFrom(userName);
        }

        AuthorizationClient AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: options.AlbaHost,
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            return client;
        }
    }
}
