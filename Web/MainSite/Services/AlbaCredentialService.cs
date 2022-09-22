using TerritoryTools.Alba.Controllers.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TerritoryTools.Vault;
using TerritoryTools.Web.Data;
using Microsoft.Extensions.Caching.Memory;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaCredentialService
    {
        Credentials GetCredentialsFrom(string userName);
        Guid GetAlbaAccountIdFor(string userName);
        void SaveCredentials(Credentials credentials);
    }

    public class AlbaCredentialAzureVaultService : IAlbaCredentialService
    {
        string _appId;
        string _secret;
        string _vaultName;
        private readonly IMemoryCache _memoryCache;
        MainDbContext _database;

        public AlbaCredentialAzureVaultService(
            IMemoryCache memoryCache,
            MainDbContext database,
            IOptions<WebUIOptions> optionAccessor)
        {
            _memoryCache = memoryCache;
            _database = database;
            _appId = optionAccessor.Value.AzureAppId;
            _secret = optionAccessor.Value.AzureClientSecret;
            _vaultName = "territorywebvault";
        }

        public Credentials GetCredentialsFrom(string userName)
        {
            Guid id = GetAlbaAccountIdFor(userName);
           
            string acct = GetSecret($"alba-account-name-{id}");
            string usr = GetSecret($"alba-account-user-{id}");
            string pwd = GetSecret($"alba-account-password-{id}");

            var credentials = new Credentials(acct, usr, pwd)
            {
                AlbaAccountId = id
            };

            return credentials;
        }

        string GetSecret(string key)
        {
            if (!_memoryCache.TryGetValue(key, out string cacheValue))
            {
                var vault = new AzureKeyVaultClient(
                clientId: _appId,
                clientSecret: _secret,
                vaultName: _vaultName);

                string secret = vault.GetSecret(key);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(8));

                _memoryCache.Set(key, secret, cacheEntryOptions);

                return secret;
            }

            return cacheValue;
        }

        public void SaveCredentials(Credentials credentials)
        {
            var vault = new AzureKeyVaultClient(
                clientId: _appId,
                clientSecret: _secret,
                vaultName: _vaultName);

            Guid id = credentials.AlbaAccountId;

            vault.WriteSecret($"alba-account-name-{id}", credentials.Account);
            vault.WriteSecret($"alba-account-user-{id}", credentials.User);
            vault.WriteSecret($"alba-account-password-{id}", credentials.Password);
        }

        public Guid GetAlbaAccountIdFor(string userName)
        {
            var identityUser = _database
               .Users
               .SingleOrDefault(u => u.NormalizedEmail == userName);

            if(identityUser == null)
            {
                throw new AlbaCredentialException(
                    $"An identity with the user name '{userName}' does not exist!");
            }

            var territoryUser = _database
                .TerritoryUser
                .SingleOrDefault(u => u.AspNetUserId == identityUser.Id);

            if (territoryUser == null)
            {
                territoryUser = _database
                    .TerritoryUser
                    .SingleOrDefault(u => u.Email.ToUpper() == userName.ToUpper());

                if (territoryUser != null && territoryUser.AspNetUserId == null)
                {
                    territoryUser.AspNetUserId = identityUser.Id;
                    _database.Update(territoryUser);
                    _database.SaveChanges();
                }
            }

            if (territoryUser == null)
            {
                throw new AlbaCredentialException(
                    $"A territory user with identity user ID '{identityUser.Id}' and email '{userName}' does not exist!  Make sure you were invited with the correct email address.");
            }

            var accountLink = _database
                .TerritoryUserAlbaAccountLink
                .FirstOrDefault(l => l.TerritoryUserId == territoryUser.Id);

            if (accountLink == null)
            {
                throw new AlbaCredentialException(
                    $"An Alba account link for territory user id '{territoryUser.Id}' does not exist!");
            }

            return accountLink.AlbaAccountId;
        }
    }
}
