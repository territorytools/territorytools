using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaAssignmentGateway
    {
        List<AlbaAssignmentValues> GetAlbaAssignments(string userName);
        void LoadAlbaAssignments(string userName);
    }

    public class AlbaAssignmentGateway : IAlbaAssignmentGateway
    {
        readonly IAlbaAuthClientService _albaAuthClientService;
        readonly IAlbaCredentialService _albaCredentialService;
        private readonly IMemoryCache _memoryCache;
        readonly WebUIOptions _options;

        public AlbaAssignmentGateway(
            IAlbaAuthClientService albaAuthClientService,
            IAlbaCredentialService albaCredentialService,
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _albaAuthClientService = albaAuthClientService;
            _albaCredentialService = albaCredentialService;
            _memoryCache = memoryCache;
            _options = optionsAccessor.Value;
        }

        public List<AlbaAssignmentValues> GetAlbaAssignments(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);

            if (!_memoryCache.TryGetValue(
                  $"AllAlbaTerritoryAssignments:Account_{albaAccountId}",
                  out List<AlbaAssignmentValues> cacheValue))
            {
                List<AlbaAssignmentValues> assignments = DownloadAssignments(userName);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                _memoryCache.Set($"AllAlbaTerritoryAssignments:Account_{albaAccountId}", assignments, cacheEntryOptions);

                return assignments;
            }

            return cacheValue;
        }

        public void LoadAlbaAssignments(string userName)
        {
            DownloadAssignments(userName);
        }

        List<AlbaAssignmentValues> DownloadAssignments(string userName)
        {
            var credentials = _albaCredentialService.GetCredentialsFrom(userName);

            var client = _albaAuthClientService.AuthClient();
            client.Authenticate(credentials);

            var assignmentsJson = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string assignmentsHtml = TerritoryAssignmentParser.Parse(assignmentsJson);

            // TODO: Probably don't need a dependency on client here
            var useCase = new DownloadTerritoryAssignments(_albaAuthClientService.AuthClient());

            var assignments = useCase.GetAssignments(assignmentsHtml);
            return assignments;
        }
    }
}
