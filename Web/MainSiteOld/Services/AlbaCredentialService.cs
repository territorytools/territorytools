﻿using TerritoryTools.Alba.Controllers.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TerritoryTools.Vault;
using TerritoryTools.Web.Data;

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

            Guid id = GetAlbaAccountIdFor(userName);
           
            string acct = vault.GetSecret($"alba-account-name-{id}");
            string usr = vault.GetSecret($"alba-account-user-{id}");
            string pwd = vault.GetSecret($"alba-account-password-{id}");

            var credentials = new Credentials(acct, usr, pwd)
            {
                AlbaAccountId = id
            };

            return credentials;
        }

        public void SaveCredentials(Credentials credentials)
        {
            var vault = new AzureKeyVaultClient(
                clientId: appId,
                clientSecret: secret,
                vaultName: vaultName);

            Guid id = credentials.AlbaAccountId;

            vault.WriteSecret($"alba-account-name-{id}", credentials.Account);
            vault.WriteSecret($"alba-account-user-{id}", credentials.User);
            vault.WriteSecret($"alba-account-password-{id}", credentials.Password);
        }

        public Guid GetAlbaAccountIdFor(string userName)
        {
            var identityUser = database
               .Users
               .SingleOrDefault(u => u.NormalizedEmail == userName);

            if(identityUser == null)
            {
                throw new Exception(
                    $"An identity with the user name '{userName}' does not exist!");
            }

            var territoryUser = database
                .TerritoryUser
                .SingleOrDefault(u => u.AspNetUserId == identityUser.Id);

            if (territoryUser == null)
            {
                throw new Exception(
                    $"A territory user with identity user ID '{identityUser.Id}' does not exist!");
            }

            var accountLink = database
                .TerritoryUserAlbaAccountLink
                .FirstOrDefault(l => l.TerritoryUserId == territoryUser.Id);

            if (accountLink == null)
            {
                throw new Exception(
                    $"An Alba account link for territory user id '{territoryUser.Id}' does not exist!");
            }

            return accountLink.AlbaAccountId;
        }
    }
}
