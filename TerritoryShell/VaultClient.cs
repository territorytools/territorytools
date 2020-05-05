using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TerritoryShell
{
    public class VaultClient
    {
        const string CLIENTSECRET = "xxxxxxx";
        const string CLIENTID = "xxxxxxx";
        const string BASESECRETURI = "https://territorywebvault.vault.azure.net";
        static KeyVaultClient keyVaultClient = null;

        public static string GetSecret()
        {
            var client = new VaultClient();
            Task<string> task = VaultClient.GetSecretFrom();
            
            string message = task.Result;
            
            return message;
        }
        public static async Task<string> GetSecretFrom()
        {
            // Resources
            // https://www.codeproject.com/Tips/1430794/Using-Csharp-NET-to-Read-and-Write-from-Azure-Key
            // https://docs.microsoft.com/en-us/azure/key-vault/general/tutorial-net-create-vault-azure-web-app#log-in-to-azure
            // Prerequisites
            // 1. Create an 'App Registration'
            // 2. Click on 'Certifcates & secrets'
            // 3. Click 'New client secret', name it whatever, it's not used here
            // 4. Go to the KeyVault
            // 5. Click on Access Policies, add a principle, find the name you created in step 1
            // 6. Give it read/write permission in the vault
            /* The next four lines of code show you how to use AppAuthentication library to fetch secrets from your key vault */
            try
            {
                //var azureServiceTokenProvider = new AzureServiceTokenProvider();
                //var keyVaultClient = new KeyVaultClient(
                //    new KeyVaultClient.AuthenticationCallback(
                //        azureServiceTokenProvider.KeyVaultTokenCallback));
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

                SecretBundle secret = Task.Run(() => keyVaultClient.GetSecretAsync(BASESECRETURI +
                    @"/secrets/" + "TestUserName")).ConfigureAwait(false).GetAwaiter().GetResult();

                return secret.Value;
            }
            catch (KeyVaultErrorException keyVaultException)
            {
                Console.WriteLine(keyVaultException.Message);
                return keyVaultException.Message;
            }
        }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(CLIENTID, CLIENTSECRET);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
