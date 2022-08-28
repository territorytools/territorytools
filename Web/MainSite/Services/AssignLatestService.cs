using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Models;
using cuc = Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAssignLatestService
    {
        AssignmentResult AssignmentLatest(AssignmentLatestRequest request);
        TerritoryLinkContract AssignmentLatestV2(AssignmentLatestRequest request);
    }

    public class AssignmentLatestRequest
    {
        public string RealUserName { get; set; }
        public int AlbaUserId { get; set; }
        public int Count { get; set; } = 1;
        public string Area { get; set; } = "*";
    }

    public class AssignmentResult
    {
        public bool Success { get; internal set; }
        public string Message { get; set; }
        public List<TerritoryResultItem> Items { get; set; } = new List<TerritoryResultItem>();
    }

    public class AssignLatestService : IAssignLatestService
    {
        readonly IUserService _userService;
        readonly ITerritoryAssignmentService _territoryAssignmentService;
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly IAlbaCredentialService _albaCredentialService;
        readonly AreaService _areaService;
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;
        readonly WebUIOptions _options;

        public AssignLatestService(
            IUserService userService,
            ITerritoryAssignmentService territoryAssignmentService,
            ICombinedAssignmentService combinedAssignmentService,
            IAlbaCredentialService albaCredentialService,
            AreaService areaService,
            ILogger<AssignLatestService> logger,
            IOptions<WebUIOptions> optionsAccessor,
            IConfiguration configuration)
        {
            _userService = userService;
            _territoryAssignmentService = territoryAssignmentService;
            _combinedAssignmentService = combinedAssignmentService;
            _albaCredentialService = albaCredentialService;
            _areaService = areaService;
            _logger = logger;
            _configuration = configuration;
            _options = optionsAccessor.Value;
        }

        public AssignmentResult AssignmentLatest(AssignmentLatestRequest request)
        {
            string realUserName = request.RealUserName;
            int userId = request.AlbaUserId;
            int count = request.Count;
            string area = request.Area;

            _logger.LogInformation($"Assigning latest territory count: {count} area: {area} to userId: {userId} As: ({realUserName})...");

            var credentials = _albaCredentialService.GetCredentialsFrom(realUserName);

            var client = AlbaConnection.From(_options.AlbaHost);
            client.Authenticate(credentials);

            var territories = _combinedAssignmentService.GetAllAssignments(realUserName)
                .Rows;

            if (territories.Count() == 0)
            {
                string message = "There are no territories to assign!";
                _logger.LogError(message);
                return new AssignmentResult
                {
                    Success = false,
                    Message = message
                };
            }

            var areas = _areaService.All();
            var includePattern = new Regex("^\\w{3}\\d{3}$|\\s+\\w{3}$");
            if (area != "*")
            {
                var matchedArea = areas.FirstOrDefault(a => a.Code == area);
                if (matchedArea == null)
                {
                    string message = $"There are {territories.Count()} territories, but none in the area you have requested! (1)";
                    _logger.LogError(message);
                    return new AssignmentResult
                    {
                        Success = false,
                        Message = message
                    };
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
                where includePattern.IsMatch(t.Description ?? string.Empty)
                    && t.Status != null
                    && t.Status.ToUpper() == "AVAILABLE"
                select t;

            if (queryInclude.Count() == 0)
            {
                string message = $"There are {territories.Count()} territories, but none in the area you have requested!";
                _logger.LogError(message);
                return new AssignmentResult
                {
                    Success = false,
                    Message = message
                };
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
                _logger.LogError(message);
                return new AssignmentResult
                {
                    Success = false,
                    Message = message
                };
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
                    _logger.LogError(message);
                    return new AssignmentResult
                    {
                        Success = false,
                        Message = message
                    };
                }
            }

            string currentUserName = "Somebody";

            try
            {
                cuc.User myUser = _userService.GetUsers(realUserName)
                    .FirstOrDefault(u => u.Id == userId);

                if (myUser != null)
                {
                    //wrong//realUserName = myUser.Name;
                    currentUserName = myUser.Name;
                }
            }
            catch (Exception)
            {
                string message = $"Cannot get user name for user id {userId}";
                _logger.LogError(message);
                return new AssignmentResult
                {
                    Success = false,
                    Message = message
                };
            }

            var refreshedTerritories = new List<AlbaAssignmentValues>();

            try
            {
                // This should refresh the mobile territory link to send to the user
                _combinedAssignmentService.LoadAssignments(realUserName);

                GetAllAssignmentsResult result = _combinedAssignmentService.GetAllAssignments(realUserName);

                refreshedTerritories = result.Rows
                    .Where(a => latestTerritoryIds.Contains(a.Id))
                    .ToList();
            }
            catch (Exception)
            {
                string message = "Cannot refresh mobile territory link";
                _logger.LogError(message);
                return new AssignmentResult
                {
                    Success = false,
                    Message = message
                };
            }

            var items = refreshedTerritories.Select(a =>
                new TerritoryResultItem
                {
                    Uri = a.MobileLink,
                    Description = $"{a.Number} {a.Description}"
                })
                .ToList();

            _logger.LogInformation($"Successfully assigned {items.Count} territories to {currentUserName} As: ({realUserName})");
            foreach (var item in items)
            {
                _logger.LogInformation($"Successfully assigned {item.Description} to {currentUserName} As: ({realUserName})");
            }

            return new AssignmentResult
            {
                Success = true,
                Message = $"Successfully assigned to {currentUserName}",
                Items = items
            };
        }

        public TerritoryLinkContract AssignmentLatestV2(AssignmentLatestRequest request)
        {
            string territoryApiHostAndPort = _configuration.GetValue<string>("TerritoryApiHostAndPort");
            if (string.IsNullOrWhiteSpace(territoryApiHostAndPort))
            {
                throw new ArgumentNullException(nameof(territoryApiHostAndPort));
            }

            HttpClient client = new();
            HttpResponseMessage? result = client.PostAsync(
                $"http://{territoryApiHostAndPort}/territory-assignment/assignments/oldest/alba?area={request.Area}&albaUserId={request.AlbaUserId}", null)
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return new TerritoryLinkContract();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            return JsonSerializer
                 .Deserialize<TerritoryLinkContract>(json, jsonOptions)
                 ?? new TerritoryLinkContract();
        }
    }

    public class TerritoryResultItem
    {
        public string Uri { get; set; }
        public string Description { get; set; }
    }
}
