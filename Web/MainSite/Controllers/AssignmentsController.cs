using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Services;
using cuc = Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/assignments")]
    public class AssignmentsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly IAlbaCredentialService albaCredentialService;
        private readonly AreaService _areaService;
        readonly ITerritoryAssignmentService territoryAssignmentService;
        readonly ILogger logger;
        readonly WebUIOptions options;

        public AssignmentsController(
            IUserService userService,
            ICombinedAssignmentService combinedAssignmentService,
            IAlbaCredentialService albaCredentialService,
            AreaService areaService,
            ITerritoryAssignmentService territoryAssignmentService,
            ILogger<AssignmentsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _userService = userService;
            _combinedAssignmentService = combinedAssignmentService;
            this.albaCredentialService = albaCredentialService;
            _areaService = areaService;
            this.territoryAssignmentService = territoryAssignmentService;
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
            int userId,
            [Range(1, 99)]
            int count = 1,
            string area = "*")
        {
            var credentials = albaCredentialService.GetCredentialsFrom(User.Identity.Name);

            var client = AuthorizedConnection();
            client.Authenticate(credentials);

            var territories = territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name);

            if (territories.Count() == 0)
            {
                string message = "There are no territories to assign!";
                logger.LogError(message);
                return BadRequest(message);
            }

            var areas = _areaService.All();
            var includePattern = new Regex("^\\w{3}\\d{3}$");
            if(area != "*")
            {
                var matchedArea = areas.FirstOrDefault(a => a.Code == area);
                if (matchedArea == null)
                {
                    string message = $"There are {territories.Count()} territories, but none in the area you have requested! (1)";
                    logger.LogError(message);
                    return BadRequest(message);
                }

                if (matchedArea.IsParent)
                {
                    string areaPattern = "";
                    var children = areas.Where(a => a.Parent == matchedArea.Parent).Select(a => a.Code);
                    if (children.Count() > 0)
                    {
                        areaPattern = string.Join('|', children);
                        includePattern = new Regex("^(" + areaPattern + ")\\d{3}$");
                    }
                }
                else
                {
                    includePattern = new Regex("^" + area + "\\d{3}$");

                }
            }

            var queryInclude =
                from t in territories
                where includePattern.IsMatch(t.Description)
                    && t.Status != null
                    && t.Status.ToUpper() == "AVAILABLE"
                select t;

            if (queryInclude.Count() == 0)
            {
                string message = $"There are {territories.Count()} territories, but none in the area you have requested!";
                logger.LogError(message);
                return BadRequest(message);
            }

            // TODO: Remove this magic RegEx string...
            var excludePattern = new Regex(
                "(^(MER|BIZ|LETTER|TELEPHONE|NOT).*|.*\\-BUSINESS)");

            var queryExclude =
                from t in queryInclude
                where !excludePattern.IsMatch(t.Description)
                select t;

            if (queryExclude.Count() == 0)
            {
                string message = $"There are {territories.Count()} territories, include includes {queryExclude.Count()}, but none match the exclude pattern!";
                logger.LogError(message);
                return BadRequest(message);
            }

            var latestTerritories = queryExclude
                .OrderBy(t => t.LastCompleted ?? DateTime.MinValue)
                .Take(count);

            var latestTerritoryIds = new List<int>();
            foreach (var territory in latestTerritories)
            {
                try
                {
                    latestTerritoryIds.Add(territory.Id);
                    string result = client.DownloadString(
                        RelativeUrlBuilder.AssignTerritory(
                            territory.Id,
                            userId,
                            DateTime.Now));

                }
                catch (Exception)
                {
                    string message = $"Cannot assign territory {territory.Id} to user {userId}";
                    logger.LogError(message);
                    return BadRequest(message);
                }
            }

            string userName = "Somebody";

            try
            {
                cuc.User myUser = _userService.GetUsers(User.Identity.Name)
                    .FirstOrDefault(u => u.Id == userId);

                if (myUser != null)
                {
                    userName = myUser.Name;
                }
            }
            catch(Exception)
            {
                string message = $"Cannot get user name for user id {userId}";
                logger.LogError(message);
                return BadRequest(message);
            }

            var refreshedTerritories = new List<AlbaAssignmentValues>();

            try
            {
                // This should refresh the mobile territory link to send to the user
                var allAssignments = territoryAssignmentService
                    .GetAllAssignmentsFresh(User.Identity.Name);

                refreshedTerritories = allAssignments
                    .Where(a => latestTerritoryIds.Contains(a.Id))
                    .ToList();
            }
            catch(Exception)
            {
                string message = "Cannot refresh mobile territory link";
                logger.LogError(message);
                return BadRequest(message);
            }

            var items = refreshedTerritories.Select(a =>
                new TerritoryResultItem
                {
                    Uri = a.MobileLink,
                    Description = $"{a.Number} {a.Description}"
                })
                .ToList();

            return Ok(
                new AssignmentResult
                {
                    Success = true,
                    Message = $"Successfully assigned to {userName}",
                    Items = items
                });
        }

        public class AssignmentResult
        {
            public bool Success { get; internal set; }
            public string Message { get; set; }
            public List<TerritoryResultItem> Items { get; set; } = new List<TerritoryResultItem>();
        }

        public class TerritoryResultItem
        {
            public string Uri { get; set;  }
            public string Description { get; internal set; }

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
            return territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name);
        }

        [HttpGet("[action]")]
        public IEnumerable<AlbaAssignmentValues> NeverCompleted()
        {
            try
            {
                return territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name)
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
                var groups = territoryAssignmentService.GetAllAssignmentsFresh(User.Identity.Name)
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
            _combinedAssignmentService.LoadAssignments(User.Identity.Name);
            ////LoadForCurrentAccount();
            
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
            _combinedAssignmentService.LoadAssignments(User.Identity.Name);
        }
        
        AlbaConnection AuthorizedConnection()
        {
            return AlbaConnection.From(options.AlbaHost);
        }
    }
}
