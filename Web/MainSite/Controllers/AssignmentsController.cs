using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/assignments")]
    public class AssignmentsController : Controller
    {
        private readonly IAssignLatestService _assignmentService;
        readonly IUserService _userService;
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly IAlbaCredentialService _albaCredentialService;
        readonly ITerritoryAssignmentService _territoryAssignmentService;
        readonly ILogger _logger;
        readonly WebUIOptions _options;

        public AssignmentsController(
            IAssignLatestService assignmentService,
            IUserService userService,
            ICombinedAssignmentService combinedAssignmentService,
            IAlbaCredentialService albaCredentialService,
            ITerritoryAssignmentService territoryAssignmentService,
            ILogger<AssignmentsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _assignmentService = assignmentService;
            _userService = userService;
            _combinedAssignmentService = combinedAssignmentService;
            _albaCredentialService = albaCredentialService;
            _territoryAssignmentService = territoryAssignmentService;
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpGet("[action]")]
        public IActionResult Assign(int territoryId, int userId)
        {
            _logger.LogInformation($"Assigning territoryId {territoryId} to userId: {userId} ({User.Identity.Name})");

            var credentials = _albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            string result = client.DownloadString(
                RelativeUrlBuilder.AssignTerritory(
                    territoryId,
                    userId,
                    DateTime.Now));

            var myUser = _userService.GetUsers(User.Identity.Name)
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

        [HttpPost("latest")]
        public ActionResult<AssignmentResult> AssignLatest(
            string userName,
            int userId,
            [Range(1, 99)]
            int count = 1,
            string area = "*")
        {
            var request = new AssignmentLatestRequest
            {
                RealUserName = User.Identity.Name,
                UserId = userId,
                Count = count,
                Area = area
            };

            AssignmentResult result = _assignmentService.AssignmentLatest(request);

            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        public class TerritoryResultItem
        {
            public string Uri { get; set;  }
            public string Description { get; internal set; }

        }

        [HttpGet("[action]")]
        public IActionResult Unassign(int territoryId)
        {
            _logger.LogInformation($"Unassigning territoryId {territoryId} ({User.Identity.Name})");

            var credentials = _albaCredentialService.GetCredentialsFrom(User.Identity.Name);

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
            return _territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name);
        }

        [HttpGet("[action]")]
        public IEnumerable<AlbaAssignmentValues> NeverCompleted()
        {
            try
            {
                return _territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name)
                    // Territories never worked
                    .Where(a => a.LastCompleted == null && a.SignedOut == null) 
                    .OrderBy(a => a.Description)
                    .ThenBy(a => a.Number);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

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
                var groups = _territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name)
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
                _logger.LogError(e.Message);

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
            _logger.LogInformation("ClockTick: Not really loading territory assigments...");
            // TODO: Fix clock tick, need a user or something here
            //Load();
        }

        [HttpGet("[action]")]
        public IActionResult LoadAssignments()
        {
            _combinedAssignmentService.LoadAssignments(User.Identity.Name);
            ////LoadForCurrentAccount();
            
            // TODO: Use with React or other UI
            // return new LoadAssignmentsResult() { Successful = true };
            return Redirect("/Home/Load");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadCsvFiles()
        {
            _logger.LogInformation("Downloading CSV files...");

            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(User.Identity.Name);
            string path = string.Format(_options.AlbaAssignmentsHtmlPath, albaAccountId);

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
            _logger.LogInformation("Downloading border KML files...");

            var credentials = _albaCredentialService.GetCredentialsFrom(User.Identity.Name);

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
            _combinedAssignmentService.LoadAssignments(User.Identity.Name);
        }
        
        AlbaConnection AuthorizedConnection()
        {
            return AlbaConnection.From(_options.AlbaHost);
        }
    }
}
