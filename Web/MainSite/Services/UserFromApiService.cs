using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IUserFromApiService
    {
        UserContract ByFullName(string userFullName);
        UserContract ByEmail(string userEmail);
    }

    public class UserFromApiService : IUserFromApiService
    {
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;
        readonly WebUIOptions _options;

        public UserFromApiService(
            ILogger<AssignLatestService> logger,
            IOptions<WebUIOptions> optionsAccessor,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _options = optionsAccessor.Value;
        }

        public UserContract ByFullName(string userFullName)
        {
            string territoryApiHostAndPort = _configuration.GetValue<string>("TerritoryApiHostAndPort");
            if (string.IsNullOrWhiteSpace(territoryApiHostAndPort))
            {
                throw new ArgumentNullException(nameof(territoryApiHostAndPort));
            }

            string mobileBaseUrl = _configuration.GetValue<string>("MobileBaseUrl");
            if (string.IsNullOrWhiteSpace(mobileBaseUrl))
            {
                throw new ArgumentNullException(nameof(mobileBaseUrl));
            }

            HttpClient client = new();
            HttpResponseMessage? result = client.GetAsync(
                $"http://{territoryApiHostAndPort}/users?userFullName={userFullName}")
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return new UserContract();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            UserContract contracts = JsonSerializer
                 .Deserialize<UserContract>(json, jsonOptions)
                 ?? new UserContract();

            return contracts;
        }

        public UserContract ByEmail(string userEmail)
        {
            string territoryApiHostAndPort = _configuration.GetValue<string>("TerritoryApiHostAndPort");
            if (string.IsNullOrWhiteSpace(territoryApiHostAndPort))
            {
                throw new ArgumentNullException(nameof(territoryApiHostAndPort));
            }

            string mobileBaseUrl = _configuration.GetValue<string>("MobileBaseUrl");
            if (string.IsNullOrWhiteSpace(mobileBaseUrl))
            {
                throw new ArgumentNullException(nameof(mobileBaseUrl));
            }

            HttpClient client = new();
            HttpResponseMessage? result = client.GetAsync(
                $"http://{territoryApiHostAndPort}/users/single?email={userEmail}")
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return new UserContract();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            UserContract contracts = JsonSerializer
                 .Deserialize<UserContract>(json, jsonOptions)
                 ?? new UserContract();

            return contracts;
        }
    }
}