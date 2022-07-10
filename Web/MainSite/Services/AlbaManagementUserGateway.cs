using Controllers.UseCases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaManagementUserGateway
    {
        List<AlbaUserView> GetAlbaManagementUsers(string userName);
        void LoadUsers(string userName);
    }

    public class AlbaManagementUserGateway : IAlbaManagementUserGateway
    {
        readonly WebUIOptions options;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly IAlbaAuthClientService _albaAuthClientService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AlbaManagementUserGateway> _logger;

        public AlbaManagementUserGateway(
            IAlbaCredentialService albaCredentialService,
            IAlbaAuthClientService albaAuthClientService, 
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<AlbaManagementUserGateway> logger)
        {
            options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _albaAuthClientService = albaAuthClientService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public List<AlbaUserView> GetAlbaManagementUsers(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            if (!_memoryCache.TryGetValue(
                   $"AlbaManagementUsers:AccountID_{albaAccountId}",
                   out List<AlbaUserView> cacheValue))
            {
                List<AlbaUserView> albaUsers = DownloadUsersFor(userName, albaAccountId);

                return albaUsers;
            }

            _logger.LogInformation($"Loading {cacheValue.Count} users from cache for userName: {userName}");

            return cacheValue;
        }

        public void LoadUsers(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            DownloadUsersFor(userName, albaAccountId);
        }

        List<AlbaUserView> DownloadUsersFor(string userName, Guid albaAccountId)
        {
            _logger.LogInformation($"Downloading users from Alba and caching them for userName: {userName}");

            var credentials = _albaCredentialService.GetCredentialsFrom(userName);

            var client = _albaAuthClientService.AuthClient();
            client.Authenticate(credentials);

            var json = client.DownloadString(
                RelativeUrlBuilder.GetUserManagementPage());

            string html = AlbaJsonResultParser.ParseDataHtml(json, "users");

            var users = DownloadUserManagementData.GetUsers(html);

            var albaUsers = new List<AlbaUserView>();
            foreach (var u in users)
            {
                albaUsers.Add(
                    new AlbaUserView
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role,
                        Telephone = u.Telephone,
                        Created = u.Created,
                    }
                );
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15));

            _memoryCache.Set($"AlbaManagementUsers:AccountID_{albaAccountId}", albaUsers, cacheEntryOptions);

            return albaUsers;
        }
    }
}
