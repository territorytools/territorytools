using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;
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
using TerritoryTools.Web.Data;
using TerritoryTools.Entities;
using static TerritoryTools.Web.MainSite.BasicStrings;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class AuthorizedController : Controller
    {
        protected readonly IStringLocalizer<AuthorizedController> localizer;
        protected readonly MainDbContext database;
        protected readonly IPhoneTerritoryAssignmentService _phoneTerritoryAssignmentService;
        protected string account;
        protected string user;
        protected string password;
        protected IAuthorizationService authorizationService;
        protected IAlbaCredentialService albaCredentialService;
        readonly ITerritoryAssignmentService territoryAssignmentService;

        public AuthorizedController(
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            ITerritoryAssignmentService territoryAssignmentService,
            IPhoneTerritoryAssignmentService phoneTerritoryAssignmentService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.database = database;
            this.localizer = localizer;
            account = credentials.Account;
            user = credentials.User;
            password = credentials.Password;
            this.authorizationService = authorizationService;
            this.albaCredentialService = albaCredentialService;
            this.territoryAssignmentService = territoryAssignmentService;
            _phoneTerritoryAssignmentService = phoneTerritoryAssignmentService;
            options = optionsAccessor.Value;
        }

        protected readonly WebUIOptions options;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public AlbaConnection GetAlbaConnection()
        {
            var credentials = GetCredentialsFrom(User.Identity.Name);

            var client = AuthClient();
            
            client.Authenticate(credentials);

            return client;
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

        protected GetAllAssignmentsResult GetAllAssignments()
        {
            var allAssignments = new List<AlbaAssignmentValues>();
            
            allAssignments.AddRange(GetAlbaAssignments(
                User.Identity.Name, 
                options, 
                albaCredentialService));

            var result = GetAllPhoneAssignments();
            allAssignments.AddRange(result.Rows);

            return new GetAllAssignmentsResult
            {
                PhoneSuccess = result.PhoneSuccess,
                Rows = allAssignments
            };
        }

        protected GetAllAssignmentsResult GetAllPhoneAssignments()
        {
            var allAssignments = new List<AlbaAssignmentValues>();
            var result = _phoneTerritoryAssignmentService.GetAllAssignments();

            foreach (var phoneAssignment in result.Rows)
            {
                DateTime.TryParse(phoneAssignment.Date, out DateTime date);
                var assignment = new AlbaAssignmentValues
                {
                    Number = phoneAssignment.TerritoryNumber,
                    SignedOutTo = phoneAssignment.Transaction == "Checked Out" ? phoneAssignment.Publisher : null,
                    SignedOut = phoneAssignment.Transaction == "Checked Out" ? date : null,
                    Description = "PHONE",
                    MonthsAgoCompleted = 0,
                    MobileLink = phoneAssignment.SheetLink
                };

                allAssignments.Add(assignment);
            }

            return new GetAllAssignmentsResult
            {
                PhoneSuccess = result.Success,
                Rows = allAssignments
            };
        }

        public class GetAllAssignmentsResult
        {
            public bool PhoneSuccess { get; set; }
            public IEnumerable<AlbaAssignmentValues>  Rows { get; set;}
        }

        protected IEnumerable<AlbaAssignmentValues> GetAlbaAssignments(
            string userName, 
            WebUIOptions options,
            IAlbaCredentialService albaCredentialService)
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(userName);
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

        AlbaConnection AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: options.AlbaHost,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }
    }
}
