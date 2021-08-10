using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Services;
using cuc = Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Controllers
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

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            string result = client.DownloadString(
                RelativeUrlBuilder.AssignTerritory(
                    territoryId,
                    userId,
                    DateTime.Now));

            var myUser = GetUsersFor(credentials.AlbaAccountId)
                .FirstOrDefault(u => u.Id == userId);
            
            string userName = "Somebody";
            if (myUser != null)
            {
                userName = myUser.Name;
            }

            // This should refresh the mobile territory link to send to the user
            LoadForCurrentAccount();

            return Redirect($"/Home/AssignSuccess?territoryId={territoryId}&userName={userName}");
        }

        [HttpGet("[action]")]
        public IActionResult AssignLatest(int userId)
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            var territories = GetAllAssignments();

            if(territories.Count() == 0)
            {
                throw new Exception("There are no territories to assign!");
            }

            // TODO: Remove this magic RegEx string...
            var includePattern = new Regex(
                @"^\w{3}\d{3}$");

            var excludePattern = new Regex(
                @"(^(MER|BIZ|LETTER|TELEPHONE|NOT).*|.*\-BUSINESS)");

            var queryMatching =
                from t in territories
                where includePattern.IsMatch(t.Number) && !excludePattern.IsMatch(t.Number)
                select t;

            if(queryMatching.Count() == 0)
            {
                throw new Exception($"There are {territories.Count()} territories, but none match the pattern!");
            }

            var queryMatchingFiles = queryMatching.Take(1);

            var first = queryMatchingFiles
                .First(t => excludePattern.IsMatch(t.Number));

            try
            {
                string result = client.DownloadString(
                    RelativeUrlBuilder.AssignTerritory(
                        first.Id,
                        userId,
                        DateTime.Now));
            }
            catch(Exception)
            {
                throw new Exception($"Cannot assign territory {first.Id} to user {userId}");
            }

            string userName = "Somebody";

            try
            {
                var myUser = GetUsersFor(credentials.AlbaAccountId)
                    .FirstOrDefault(u => u.Id == userId);

                if (myUser != null)
                {
                    userName = myUser.Name;
                }
            }
            catch(Exception)
            {
                throw new Exception($"Cannot get user name for user id {userId}");
            }


            try
            {
                // This should refresh the mobile territory link to send to the user
                LoadForCurrentAccount();
            }
            catch(Exception)
            {
                throw new Exception("Cannot refresh mobile territory link");
            }

            return Redirect($"/Home/AssignSuccess?territoryId={first.Id}&userName={userName}");
        }

        [HttpGet("[action]")]
        public IActionResult Unassign(int territoryId)
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string result = client.DownloadString(
                RelativeUrlBuilder.UnassignTerritory(territoryId));

            LoadForCurrentAccount();

            return Redirect($"/Home/UnassignSuccess?territoryId={territoryId}");
        }

        [HttpGet("[action]")]
        public IEnumerable<AlbaAssignmentValues> All(string account, string user, string password)
        {
            return GetAllAssignments();
        }

        [HttpGet("[action]")]
        public IEnumerable<AlbaAssignmentValues> NeverCompleted()
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
            public List<AlbaAssignmentValues> Territories { get; set; } = new List<AlbaAssignmentValues>();
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
            LoadForCurrentAccount();

            // TODO: Use with React or other UI
            // return new LoadAssignmentsResult() { Successful = true };
            return Redirect("/Home/Load");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadCsvFiles()
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);
            string path = string.Format(options.AlbaAssignmentsHtmlPath, albaAccountId);

            var client = AuthorizedConnection();

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

            return Redirect("/Report/Index");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadBorderKmlFiles()
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            string filePath = "wwwroot/borders.kml";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var territories = new DownloadKmlFile(client)
                .SaveAs(filePath);

            return Redirect("/Report/Index");
        }

        void LoadForCurrentAccount()
        {
            Guid albaAccountId = albaCredentialService
                .GetAlbaAccountIdFor(User.Identity.Name);

            Load(albaAccountId);
        }

        void Load(Guid albaAccountId)
        {
            string path = string.Format(
                options.AlbaAssignmentsHtmlPath, 
                albaAccountId);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var client = AuthorizedConnection();

            var useCase = new DownloadTerritoryAssignments(client);

            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            client.Authenticate(credentials);

            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            System.IO.File.WriteAllText(path, html);
        }

        IEnumerable<AlbaAssignmentValues> GetAllAssignments()
        {
            Guid albaAccountId = albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);

            string path = string.Format(options.AlbaAssignmentsHtmlPath, albaAccountId);

            if (!System.IO.File.Exists(path))
            {
                LoadForCurrentAccount();
            }

            var client = AuthorizedConnection();

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

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            var assignedHtml = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            string usersHtml = cuc.DownloadUsers.GetUsersHtml(assignedHtml);

            System.IO.File.WriteAllText(path, usersHtml);
        }

        AlbaConnection AuthorizedConnection()
        {
            return AlbaConnection.From(options.AlbaHost);
        }
    }
}
