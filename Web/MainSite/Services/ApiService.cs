using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IApiService
    {
        T Get<T>(string relativePath, string queryString);
        T Post<T, B>(string relativePath, string queryString, B body);
    }

    public class ApiService : IApiService
    {
        private readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;

        public ApiService(
            ILogger<AssignLatestService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public T Post<T,B>(string relativePath, string queryString, B body)
        {
            string territoryApiHostAndPort = _configuration.GetValue<string>("TerritoryApiHostAndPort");
            if (string.IsNullOrWhiteSpace(territoryApiHostAndPort))
            {
                throw new ArgumentNullException(nameof(territoryApiHostAndPort));
            }

            HttpClient client = new();
            HttpResponseMessage? result = client.PostAsJsonAsync(
                $"http://{territoryApiHostAndPort}/{relativePath}{queryString}", body)
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return default(T);
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,
            };

            T contracts = JsonSerializer
                 .Deserialize<T>(json, jsonOptions)
                 ?? default(T);

            // TODO: Just use?  httpClient.GetFromJsonAsync<Stock>($"https://localhost:12345/smoething/{param}");
            // Good link: https://makolyte.com/csharp-get-and-send-json-with-httpclient/#:~:text=C%23%20%E2%80%93%20Get%20and%20send%20JSON%20with%20HttpClient,%28%29%20extension%20methods%20found%20in%20System.Net.Http.Json%2C%20like%20this%3A
            //var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            //options.Converters.Add(new JsonStringEnumConverter());

            //await httpClient.PostAsJsonAsync<Stock>("https://localhost:12345/stocks/", stock, options);

            return contracts;
        }

        public T Get<T>(string relativePath, string queryString)
        {
            if (!string.IsNullOrWhiteSpace(queryString) && !queryString.StartsWith("?"))
                throw new ArgumentException($"Query string must start with a question mark ?");

            string territoryApiHostAndPort = _configuration.GetValue<string>("TerritoryApiHostAndPort");
            if (string.IsNullOrWhiteSpace(territoryApiHostAndPort))
            {
                throw new ArgumentNullException(nameof(territoryApiHostAndPort));
            }

            HttpClient client = new();
            HttpResponseMessage? result = client.GetAsync(
                $"http://{territoryApiHostAndPort}/{relativePath}{queryString}")
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return default(T);
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            T contracts = JsonSerializer
                 .Deserialize<T>(json, jsonOptions)
                 ?? default(T);

            return contracts;
        }
    }
}