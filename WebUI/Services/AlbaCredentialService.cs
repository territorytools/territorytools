using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TerritoryTools.Vault;
using WebUI.Areas.Identity.Data;

namespace WebUI.Services
{
    public interface IAlbaCredentialService
    {
        Credentials GetCredentialsFrom(string userName);
    }

    public class AlbaCredentialAzureVaultService : IAlbaCredentialService
    {
        string appId;
        string secret;
        string vaultName;
        MainDbContext database;

        public AlbaCredentialAzureVaultService(
            MainDbContext database,
            IOptions<WebUIOptions> optionAccessor)
        {
            this.database = database;
            appId = optionAccessor.Value.AzureAppId;
            secret = optionAccessor.Value.AzureClientSecret;
            vaultName = "territorywebvault";
        }

        public Credentials GetCredentialsFrom(string userName)
        {
            var vault = new AzureKeyVaultClient(
                clientId: appId,
                clientSecret: secret,
                vaultName: vaultName);

            var identityUser = database
                .Users
                .SingleOrDefault(u => u.NormalizedEmail == userName);

            var territoryUser = database
                .TerritoryUser
                .SingleOrDefault(u => u.AspNetUserId == identityUser.Id);

            var accountLink = database
                .TerritoryUserAlbaAccountLink
                .FirstOrDefault(l => l.TerritoryUserId == territoryUser.Id);

            Guid albaAccountId = accountLink.AlbaAccountId;
            string acct = vault.GetSecret($"alba-account-name-{albaAccountId}");
            string usr = vault.GetSecret($"alba-account-user-{albaAccountId}");
            string pwd = vault.GetSecret($"alba-account-password-{albaAccountId}");

            var credentials = new Credentials(acct, usr, pwd, LogUserOntoAlba.k1MagicString)
            {
                AlbaAccountId = albaAccountId
            };

            return credentials;
        }
    }
}
