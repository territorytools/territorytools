using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaAssignmentGateway
    {
        GetAlbaAssignmentsResult GetAlbaAssignments(string userName);
        void LoadAlbaAssignments(string userName);
    }

    public class GetAlbaAssignmentsResult
    {
        public bool Success { get; set; }
        public List<AlbaAssignmentValues> AssignmentValues { get; set; } = new List<AlbaAssignmentValues>();
    }

    public class AlbaAssignmentGateway : IAlbaAssignmentGateway
    {
        readonly IAlbaAuthClientService _albaAuthClientService;
        readonly IAlbaCredentialService _albaCredentialService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AlbaAssignmentGateway> _logger;
        private readonly TelemetryClient _telemetryClient;
        readonly WebUIOptions _options;

        public AlbaAssignmentGateway(
            IAlbaAuthClientService albaAuthClientService,
            IAlbaCredentialService albaCredentialService,
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<AlbaAssignmentGateway> logger,
            TelemetryClient telemetryClient)
        {
            _albaAuthClientService = albaAuthClientService;
            _albaCredentialService = albaCredentialService;
            _memoryCache = memoryCache;
            _logger = logger;
            _telemetryClient = telemetryClient;
            _options = optionsAccessor.Value;
        }

        public GetAlbaAssignmentsResult GetAlbaAssignments(string userName)
        {
            try
            {
                Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);

                if (!_memoryCache.TryGetValue(
                      $"AllAlbaTerritoryAssignments:Account_{albaAccountId}",
                      out List<AlbaAssignmentValues> cacheValue))
                {
                    List<AlbaAssignmentValues> assignments = DownloadAssignments(userName, albaAccountId);

                    return new GetAlbaAssignmentsResult
                    {
                        Success = true,
                        AssignmentValues = assignments
                    };
                }

                //_logger.LogInformation($"Loaded {cacheValue.Count} assignments from memory cache for userName: {userName} albaAccountID: {albaAccountId}");
                _logger.LogInformation("Custom telemetry is active");
                _telemetryClient.TrackTrace(
                    message: $"Loaded {cacheValue.Count} assignments from memory cache for userName: {userName} albaAccountID: {albaAccountId}",
                    severityLevel: SeverityLevel.Information,
                    properties: new Dictionary<string, string>()
                    {
                        { "AssignmentCount", $"{cacheValue.Count}" },
                        { "UserName", $"{userName}" }
                    });

                return new GetAlbaAssignmentsResult
                {
                    Success = true,
                    AssignmentValues = cacheValue
                };
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error loading assignments for userName: {userName} Error: {ex}");
                _telemetryClient.TrackException(
                   ex,
                   //message: $"Error loading assignments for userName: {userName} Error: {ex}",
                   //severityLevel: SeverityLevel.Error,
                   properties: new Dictionary<string, string>()
                   {
                        //{ "AssignmentCount", $"{cacheValue.Count}" },
                        { "UserName", $"{userName}" }
                   });
                return new GetAlbaAssignmentsResult
                {
                    Success = false
                };
            }
        }

        public void LoadAlbaAssignments(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            DownloadAssignments(userName, albaAccountId);
        }

        List<AlbaAssignmentValues> DownloadAssignments(string userName, Guid albaAccountId)
        {
            _logger.LogInformation($"Downloading assignments from Alba and caching them for userName: {userName} albaAccountID: {albaAccountId}");

            var credentials = _albaCredentialService.GetCredentialsFrom(userName);

            var client = _albaAuthClientService.AuthClient();
            client.Authenticate(credentials);

            var assignmentsJson = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string assignmentsHtml = TerritoryAssignmentParser.Parse(assignmentsJson);

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(_albaAuthClientService.AuthClient());

            var assignments = useCase.GetAssignments(assignmentsHtml);


            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15));

            _memoryCache.Set($"AllAlbaTerritoryAssignments:Account_{albaAccountId}", assignments, cacheEntryOptions);

            return assignments;
        }
    }
}
