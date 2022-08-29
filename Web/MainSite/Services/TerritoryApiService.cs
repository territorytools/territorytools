using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoryApiService
    {
        List<TerritoryContract> TerritoriesCheckedOutTo(string userFullName);
        List<TerritoryContract> AllTerritories();
        List<AreaContract> AllAreas();
        T ApiCall<T>(string relativePath, string queryString);
    }

    public class TerritoryApiService : ITerritoryApiService
    {
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;

        public TerritoryApiService(
            ILogger<AssignLatestService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public List<TerritoryContract> TerritoriesCheckedOutTo(string userFullName)
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
                $"http://{territoryApiHostAndPort}/territories/checked-out?userFullName={userFullName}")
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return new List<TerritoryContract>();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            List<TerritoryContract> contracts = JsonSerializer
                 .Deserialize<List<TerritoryContract>>(json, jsonOptions)
                 ?? new List<TerritoryContract>();

            foreach (var contract in contracts)
            {
                contract.AssigneeMobileLink = $"{mobileBaseUrl}/mtk/{contract.AssigneeLinkKey}";
            }

            return contracts;
        }

        public List<TerritoryContract> AllTerritories()
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
                $"http://{territoryApiHostAndPort}/territories/all-v2")
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                string message = $"Error from Territory.Api StatusCode: {result.StatusCode}";
                _logger.LogError(message);
                return new List<TerritoryContract>();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            JsonSerializerOptions jsonOptions = new()
            {
                AllowTrailingCommas = true,
                MaxDepth = 5,
                PropertyNameCaseInsensitive = true,

            };

            List<TerritoryContract> contracts = JsonSerializer
                 .Deserialize<List<TerritoryContract>>(json, jsonOptions)
                 ?? new List<TerritoryContract>();

            foreach (var contract in contracts)
            {
                contract.AssigneeMobileLink = $"{mobileBaseUrl}/mtk/{contract.AssigneeLinkKey}";
            }

            return contracts;
        }

        public List<AreaContract> AllAreas()
        {
            return ApiCall<List<AreaContract>>("areas","");
        }

        public T ApiCall<T>(string relativePath, string queryString)
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