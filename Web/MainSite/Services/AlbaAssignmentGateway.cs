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
        readonly WebUIOptions _options;

        public AlbaAssignmentGateway(
            IAlbaAuthClientService albaAuthClientService,
            IAlbaCredentialService albaCredentialService,
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<AlbaAssignmentGateway> logger)
        {
            _albaAuthClientService = albaAuthClientService;
            _albaCredentialService = albaCredentialService;
            _memoryCache = memoryCache;
            _logger = logger;
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
                        AssignmentValues = assignments
                    };
                }

                _logger.LogInformation($"Loaded {cacheValue.Count} assignments from memory cache for userName: {userName} albaAccountID: {albaAccountId}");

                return new GetAlbaAssignmentsResult
                {
                    AssignmentValues = cacheValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading assignments for userName: {userName} Error: {ex}");
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
