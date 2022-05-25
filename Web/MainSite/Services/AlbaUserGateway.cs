using Controllers.UseCases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaUserGateway
    {
        List<AlbaUserView> GetAlbaUsers(string userName);
    }

    public class AlbaUserGateway : IAlbaUserGateway
    {
        readonly WebUIOptions options;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly IAlbaAuthClientService _albaAuthClientService;
        private readonly IMemoryCache _memoryCache;

        public AlbaUserGateway(
            IAlbaCredentialService albaCredentialService,
            IAlbaAuthClientService albaAuthClientService, 
            IMemoryCache memoryCache,
            IOptions<WebUIOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _albaAuthClientService = albaAuthClientService;
            _memoryCache = memoryCache;
        }

        public List<AlbaUserView> GetAlbaUsers(string userName)
        {
            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            if (!_memoryCache.TryGetValue(
                   $"AlbaUsers:AccountID_{albaAccountId}",
                   out List<AlbaUserView> cacheValue))
            {
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

                _memoryCache.Set($"AlbaUsers:AccountID_{albaAccountId}", albaUsers, cacheEntryOptions);

                return albaUsers;
            }

            return cacheValue;
        }
    }
}
