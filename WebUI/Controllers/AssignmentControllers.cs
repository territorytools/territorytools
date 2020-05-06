using System;
using System.Collections.Generic;
using System.Linq;
using AlbaClient;
using AlbaClient.AlbaServer;
using AlbaClient.Controllers.AlbaServer;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using cuc = Controllers.UseCases;
using Microsoft.Extensions.Options;
using WebUI.Services;

namespace WebUI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AssignmentsController : Controller
    {
        string account;
        string user;
        string password;
        readonly IAlbaCredentialService albaCredentialService;
        readonly ILogger logger;
        readonly WebUIOptions options;

        public AssignmentsController(
            IAlbaCredentialService albaCredentialService,
            IAlbaCredentials credentials,
            ILogger<AssignmentsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.albaCredentialService = albaCredentialService;
            account = credentials.Account;
            user = credentials.User;
            password = credentials.Password;
            this.logger = logger;
            options = optionsAccessor.Value;
        }

        [HttpGet("[action]")]
        public IActionResult Assign(int territoryId, int userId)
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authorize(credentials);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string result = client.DownloadString(
                $"/ts?mod=assigned&cmd=assign&id={territoryId}&date={date}&user={userId}");

            var myUser = GetUsers(account, user, password)
                .FirstOrDefault(u => u.Id == userId);
            
            string userName = "Somebody";
            if (myUser != null)
            {
                userName = myUser.Name;
            }


            return Redirect($"/Home/AssignSuccess?territoryId={territoryId}&userName={userName}");
        }

        [HttpGet("[action]")]
        public IActionResult Unassign(int territoryId)
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authorize(credentials);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string result = client.DownloadString(
                $"/ts?mod=assigned&cmd=unassign&id={territoryId}");

            return Redirect($"/Home/UnassignSuccess?territoryId={territoryId}");
        }

        [HttpGet("[action]")]
        public IEnumerable<Assignment> All(string account, string user, string password)
        {
            return GetAllAssignments(account, user, password);
        }

        [HttpGet("[action]")]
        public IEnumerable<Assignment> NeverCompleted()
        {
            try
            {
                return GetAllAssignments(account, user, password)
                    // Territories never worked
                    .Where(a => a.LastCompleted == null && a.SignedOut == null) 
                    .OrderBy(a => a.Description)
                    .ThenBy(a => a.Number);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);

                throw;
            }
        }

        public class Publisher
        {
            public string Name { get; set; }
            public List<Assignment> Territories { get; set; } = new List<Assignment>();
        }

        [HttpGet("[action]")]
        public IEnumerable<Publisher> ByPublisher()
        {
            try
            {
                var groups = GetAllAssignments(account, user, password)
                    .Where(a => !string.IsNullOrWhiteSpace(a.SignedOutTo))
                    .GroupBy(a => a.SignedOutTo)
                    .ToList();

                var publishers = new List<Publisher>();
                foreach (var group in groups.OrderBy(g => g.Key))
                {
                    var pub = new Publisher() { Name = group.Key };
                    foreach (var item in group.OrderByDescending(a => a.SignedOut))
                    {
                        pub.Territories.Add(item);
                    }

                    publishers.Add(pub);
                }

                return publishers;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);

                throw;
            }
        }

        public class LoadAssignmentsResult
        {
            public bool Successful { get; set; }
        }

        [AllowAnonymous]
        [Route("/ClockTick")]
        public void ClockTick()
        {
            logger.LogError("Loading Territory Assigments...");
            Load();
        }

        [HttpGet("[action]")]
        public IActionResult LoadAssignments()
        {
            Load();

            // TODO: Use with React or other UI
            // return new LoadAssignmentsResult() { Successful = true };
            return Redirect("/Home/Load");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadCsvFiles()
        {
            var client = AuthorizationClient();

            var downloader = new DownloadTerritoryAssignments(client);

            string html = System.IO.File.ReadAllText(options.AlbaAssignmentsHtmlPath);

            string csvFilePath = "wwwroot/assignments.csv";
            if (System.IO.File.Exists(csvFilePath))
            {
                System.IO.File.Delete(csvFilePath);
            }

            if (System.IO.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                downloader.SaveAs(html, csvFilePath);
            }
            else
            {
                downloader.SaveAs(csvFilePath);
            }

            return Redirect("/Home/Reports");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadBorderKmlFiles()
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authorize(credentials);

            string filePath = "wwwroot/borders.kml";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var territories = new DownloadKmlFile(client)
                .SaveAs(filePath);

            return Redirect("/Home/Reports");
        }

        private void Load()
        {
            if (System.IO.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                System.IO.File.Delete(options.AlbaAssignmentsHtmlPath);
            }

            var client = AuthorizationClient();

            var useCase = new DownloadTerritoryAssignments(client);

            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            client.Authorize(credentials);

            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            System.IO.File.WriteAllText(options.AlbaAssignmentsHtmlPath, html);
        }

        IEnumerable<Assignment> GetAllAssignments(string account, string user, string password)
        {
            if (!System.IO.File.Exists(options.AlbaAssignmentsHtmlPath))
            {
                Load();
            }

            var client = AuthorizationClient();

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(client); 

            //var credentials = new Credentials(account, user, password, k1MagicString);

            //client.Authorize(credentials);

            //var resultString = client.DownloadString(
            //    RelativeUrlBuilder.GetTerritoryAssignments());

            //html = TerritoryAssignmentParser.Parse(resultString);

            string html = System.IO.File.ReadAllText(options.AlbaAssignmentsHtmlPath);

            return useCase.GetAssignments(html);
        }

        IEnumerable<cuc.User> GetUsers(string account, string user, string password)
        {
            if (!System.IO.File.Exists(options.AlbaUsersHtmlPath))
            {
                LoadUserData();
            }

            string html = System.IO.File.ReadAllText(options.AlbaUsersHtmlPath);

            return cuc.DownloadUsers.GetUsers(html);
        }

        void LoadUserData()
        {
            if (System.IO.File.Exists(options.AlbaUsersHtmlPath))
            {
                System.IO.File.Delete(options.AlbaUsersHtmlPath);
            }

            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authorize(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            System.IO.File.WriteAllText(options.AlbaUsersHtmlPath, usersHtml);
        }

        void LoadUserManagementData()
        {
            if (System.IO.File.Exists(options.AlbaUsersHtmlPath))
            {
                System.IO.File.Delete(options.AlbaUsersHtmlPath);
            }

            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authorize(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            System.IO.File.WriteAllText(options.AlbaUsersHtmlPath, usersHtml);
        }

        private static AuthorizationClient AuthorizationClient()
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
