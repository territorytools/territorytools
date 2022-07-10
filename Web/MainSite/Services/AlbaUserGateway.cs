using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using cuc = Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaUserGateway
    {
        List<cuc.User> GetAlbaUsers(string userName);
        void LoadAlbaUsers(string userName);
    }

    public class AlbaUserGateway : IAlbaUserGateway
    {
        readonly WebUIOptions options;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly IAlbaAuthClientService _albaAuthClientService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AlbaUserGateway> _logger;

        public AlbaUserGateway(
            IAlbaCredentialService albaCredentialService,
            IAlbaAuthClientService albaAuthClientService, 
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<AlbaUserGateway> logger)
        {
            options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _albaAuthClientService = albaAuthClientService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public List<cuc.User> GetAlbaUsers(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            if (!_memoryCache.TryGetValue(
                   $"AlbaUsers:AccountID_{albaAccountId}",
                   out List<cuc.User> cacheValue))
            {
                List<cuc.User> users = DownloadUsers(userName, albaAccountId);

                return users;
            }

            _logger.LogInformation($"Loaded {cacheValue.Count} users from cache for userName: {userName} albaAccountID: {albaAccountId}");

            return cacheValue;
        }

        public void LoadAlbaUsers(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            DownloadUsers(userName, albaAccountId);
        }

        List<cuc.User> DownloadUsers(string userName, Guid albaAccountId)
        {
            _logger.LogInformation($"Downloading users from Alba and caching them for userName: {userName} albaAccountID: {albaAccountId}");

            var credentials = _albaCredentialService.GetCredentialsFrom(userName);

            var client = _albaAuthClientService.AuthClient();
            client.Authenticate(credentials);

            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignmentsPage());

            List<cuc.User> users = cuc.DownloadUsers.GetUsers(cuc.DownloadUsers.GetUsersHtml(resultString));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15));

            _memoryCache.Set($"AlbaUsers:AccountID_{albaAccountId}", users, cacheEntryOptions);
            return users;
        }
    }
}
