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
        const string BASESECRETURI = "https://territorywebvault.vault.azure.net";
        static KeyVaultClient keyVaultClient = null;
        static string _clientId;
        static string _clientSecret;

        public static string GetSecret(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            var client = new VaultClient();
            Task<string> task = VaultClient.GetSecretFrom(clientId, clientSecret);
            
            string message = task.Result;
            
            return message;
        }
        public static async Task<string> GetSecretFrom(string clientId, string clientSecret)
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
                    @"/secrets/" + "alba-account-user-seattleeastchinese")).ConfigureAwait(false).GetAwaiter().GetResult();

                return secret.Value;
            }
            catch (KeyVaultErrorException keyVaultException)
            {
                Console.WriteLine(keyVaultException.Message);
                return keyVaultException.Message;
            }
        }

        private static async void WriteKeyVault(string name, string value)
        {
            var attribs = new SecretAttributes
            {
                Enabled = true//,
                //Expires = DateTime.UtcNow.AddYears(2), // if you want to expire the info
                //NotBefore = DateTime.UtcNow.AddDays(1) // if you want the info to 
                // start being available later
            };

            IDictionary<string, string> alltags = new Dictionary<string, string>();
            //alltags.Add("Test1", "This is a test1 value");
            //alltags.Add("Test2", "This is a test2 value");
            //alltags.Add("CanBeAnything", "Including a long encrypted string if you choose");
            //string TestName = "TestSecret";
            //string TestValue = "searchValue"; // this is what you will use to search for the item later
            string contentType = "SecretInfo"; // whatever you want to categorize it by; you name it

            SecretBundle bundle = await keyVaultClient.SetSecretAsync(
                vaultBaseUrl: BASESECRETURI,
                secretName: name,
                value: value,
                tags: alltags,
                contentType: contentType,
                secretAttributes: attribs); ;

            Console.WriteLine($"Secret written to key vault: {name}");
        }

        public static async Task<string> GetToken(
            string authority, 
            string resource, 
            string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(_clientId, _clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
