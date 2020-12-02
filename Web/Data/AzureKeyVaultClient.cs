using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerritoryTools.Vault
{
    // Resources
    // https://www.codeproject.com/Tips/1430794/Using-Csharp-NET-to-Read-and-Write-from-Azure-Key
    // https://docs.microsoft.com/en-us/azure/key-vault/general/tutorial-net-create-vault-azure-web-app#log-in-to-azure
    //
    // Prerequisites:
    // 1. Create an 'App Registration'
    // 2. Click on 'Certifcates & secrets'
    // 3. Click 'New client secret', name it whatever, it's not used here
    // 4. Go to the KeyVault
    // 5. Click on Access Policies, add a principle, find the name you created in step 1
    // 6. Give it read/write permission in the vault
    public class AzureKeyVaultClient : IVaultClient
    {
        const string BaseUrlTemplate = "https://{0}.vault.azure.net";

        KeyVaultClient _client;
        string _clientId;
        string _clientSecret;
        string _vaultName;
        string _baseUrl;

        public AzureKeyVaultClient(
            string clientId,
            string clientSecret,
            string vaultName)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _vaultName = vaultName;
            _client = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(GetTokenAsync));
            
            _baseUrl = string.Format(BaseUrlTemplate, _vaultName);
        }

        public string GetSecret(string name)
        {
            try
            {
                SecretBundle secret = Task
                    .Run(() => _client.GetSecretAsync(
                        $"{_baseUrl}/secrets/{name}"))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                return secret.Value;
            }
            catch (KeyVaultErrorException e)
            {
                throw new Exception($"Error getting secret {name} to vault", e);
            }
        }

        public void WriteSecret(string name, string value)
        {
            try
            {
                WriteKeyAsync(name, value);
            }
            catch (KeyVaultErrorException e)
            {
                throw new Exception($"Error writing secret {name} to vault", e);
            }
        }

        async void WriteKeyAsync(string name, string value)
        {
            var attributes = new SecretAttributes
            {
                Enabled = true
            };

            await _client.SetSecretAsync(
                vaultBaseUrl: _baseUrl,
                secretName: name,
                value: value,
                tags: new Dictionary<string, string>(),
                contentType: string.Empty,
                secretAttributes: attributes); ;
        }

        async Task<string> GetTokenAsync(
            string authority, 
            string resource, 
            string scope)
        {
            var context = new AuthenticationContext(authority);
            var credential = new ClientCredential(_clientId, _clientSecret);
            AuthenticationResult result = await context
                .AcquireTokenAsync(resource, credential);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}
