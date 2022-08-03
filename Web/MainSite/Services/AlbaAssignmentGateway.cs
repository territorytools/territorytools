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
        private readonly ITelemetryService _telemetryService;
        readonly WebUIOptions _options;

        public AlbaAssignmentGateway(
            IAlbaAuthClientService albaAuthClientService,
            IAlbaCredentialService albaCredentialService,
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor,
            ITelemetryService telemetryService)
        {
            _albaAuthClientService = albaAuthClientService;
            _albaCredentialService = albaCredentialService;
            _memoryCache = memoryCache;
            _telemetryService = telemetryService;
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

                _telemetryService.Trace(
                    message: $"Loaded {cacheValue.Count} assignments from memory cache for userName: {userName} albaAccountID: {albaAccountId}",
                    userName);

                return new GetAlbaAssignmentsResult
                {
                    Success = true,
                    AssignmentValues = cacheValue
                };
            }
            catch (Exception ex)
            {
                _telemetryService.Exception(ex, userName);

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
            _telemetryService.Trace(
                $"Downloading assignments from Alba and caching them for userName: {userName} albaAccountID: {albaAccountId}",
                userName);

            var assignmentsJson = _albaAuthClientService.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments(), 
                userName);

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
