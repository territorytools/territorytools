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
            this.logger = logger;
            options = optionsAccessor.Value;
        }

        [HttpGet("[action]")]
        public IActionResult Assign(int territoryId, int userId)
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authenticate(credentials);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string result = client.DownloadString(
                $"/ts?mod=assigned&cmd=assign&id={territoryId}&date={date}&user={userId}");

            var myUser = GetUsersFor(credentials.AlbaAccountId)
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
            client.Authenticate(credentials);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string result = client.DownloadString(
                $"/ts?mod=assigned&cmd=unassign&id={territoryId}");

            return Redirect($"/Home/UnassignSuccess?territoryId={territoryId}");
        }

        [HttpGet("[action]")]
        public IEnumerable<Assignment> All(string account, string user, string password)
        {
            return GetAllAssignments();
        }

        [HttpGet("[action]")]
        public IEnumerable<Assignment> NeverCompleted()
        {
            try
            {
                return GetAllAssignments()
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
                var groups = GetAllAssignments()
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
            // TODO: Fix clock tick, need a user or something here
            //Load();
        }

        [HttpGet("[action]")]
        public IActionResult LoadAssignments()
        {
            Guid id = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);
            Load(id);

            // TODO: Use with React or other UI
            // return new LoadAssignmentsResult() { Successful = true };
            return Redirect("/Home/Load");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadCsvFiles()
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);
            string path = string.Format(options.AlbaAssignmentsHtmlPath, albaAccountId);

            var client = AuthorizationClient();

            var downloader = new DownloadTerritoryAssignments(client);

            string html = System.IO.File.ReadAllText(path);

            string csvFilePath = "wwwroot/assignments.csv";
            if (System.IO.File.Exists(csvFilePath))
            {
                System.IO.File.Delete(csvFilePath);
            }

            if (System.IO.File.Exists(path))
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
            client.Authenticate(credentials);

            string filePath = "wwwroot/borders.kml";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var territories = new DownloadKmlFile(client)
                .SaveAs(filePath);

            return Redirect("/Home/Reports");
        }

        private void Load(Guid albaAccountId)
        {
            string path = string.Format(options.AlbaAssignmentsHtmlPath, albaAccountId);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var client = AuthorizationClient();

            var useCase = new DownloadTerritoryAssignments(client);

            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            client.Authenticate(credentials);

            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            System.IO.File.WriteAllText(path, html);
        }

        IEnumerable<Assignment> GetAllAssignments()
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);

            string path = string.Format(options.AlbaAssignmentsHtmlPath, albaAccountId);

            if (!System.IO.File.Exists(path))
            {
                Load(albaAccountId);
            }

            var client = AuthorizationClient();

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(client); 

            string html = System.IO.File.ReadAllText(path);

            return useCase.GetAssignments(html);
        }

        IEnumerable<cuc.User> GetUsersFor(Guid albaAccountId)
        {
            string path = string.Format(options.AlbaUsersHtmlPath, albaAccountId);

            if (!System.IO.File.Exists(path))
            {
                LoadUserData(albaAccountId);
            }

            string html = System.IO.File.ReadAllText(path);

            return cuc.DownloadUsers.GetUsers(html);
        }

        void LoadUserData(Guid albaAccountId)
        {
            string path = string.Format(options.AlbaUsersHtmlPath, albaAccountId);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            // TODO: Get credentials with albaAccountId
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizationClient();
            client.Authenticate(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            System.IO.File.WriteAllText(path, usersHtml);
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
