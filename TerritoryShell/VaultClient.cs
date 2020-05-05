using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerritoryShell
{
    public class VaultClient
    {
        const string BASESECRETURI = "https://territorywebvault.vault.azure.net";
        static KeyVaultClient keyVaultClient = null;
        static string _clientId;
        static string _clientSecret;

        public static string GetSecret(
            string clientId, 
            string clientSecret, 
            string name)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            return GetSecretFrom(name); 
        }

        public static void WriteSecret(
            string clientId, 
            string clientSecret, 
            string key, 
            string value)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            WriteKeyVault(key, value);
        }

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
        public static string GetSecretFrom(string name)
        {
            try
            {
                keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(GetToken));

                SecretBundle secret = Task
                    .Run(() => keyVaultClient.GetSecretAsync(
                        $"{BASESECRETURI}/secrets/{name}"))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

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
            keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(GetToken));

            var attribs = new SecretAttributes
            {
                Enabled = true
            };

            SecretBundle bundle = await keyVaultClient.SetSecretAsync(
                vaultBaseUrl: BASESECRETURI,
                secretName: name,
                value: value,
                tags: new Dictionary<string, string>(),
                contentType: string.Empty,
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
            AuthenticationResult result = await authContext
                .AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}
