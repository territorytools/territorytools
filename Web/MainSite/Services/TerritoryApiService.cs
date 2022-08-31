using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoryApiService
    {
        List<TerritoryContract> TerritoriesCheckedOutTo(string userFullName);
        List<TerritoryContract> AllTerritories();
        List<AreaContract> AllAreas();
    }

    public class TerritoryApiService : ITerritoryApiService
    {
        private readonly IApiService _apiService;
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;

        public TerritoryApiService(
            IApiService apiService,
            ILogger<AssignLatestService> logger,
            IConfiguration configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration;
        }

        public List<TerritoryContract> TerritoriesCheckedOutTo(string userFullName)
        {
            string mobileBaseUrl = _configuration.GetValue<string>("MobileBaseUrl");
            if (string.IsNullOrWhiteSpace(mobileBaseUrl))
            {
                throw new ArgumentNullException(nameof(mobileBaseUrl));
            }

            List<TerritoryContract> contracts = _apiService.Get<List<TerritoryContract>>(
                "territories/checked-out", $"?userFullName={userFullName}");

            if(contracts == null)
                return new List<TerritoryContract>();

            foreach (var contract in contracts)
            {
                contract.AssigneeMobileLink = $"{mobileBaseUrl}/mtk/{contract.AssigneeLinkKey}";
            }

            return contracts;
        }

        public List<TerritoryContract> AllTerritories()
        {
            string mobileBaseUrl = _configuration.GetValue<string>("MobileBaseUrl");
            if (string.IsNullOrWhiteSpace(mobileBaseUrl))
            {
                throw new ArgumentNullException(nameof(mobileBaseUrl));
            }

            List<TerritoryContract> contracts = _apiService.Get<List<TerritoryContract>>("territories/all-v2", "");

            if (contracts == null)
                return new List<TerritoryContract>();

            foreach (var contract in contracts)
            {
                contract.AssigneeMobileLink = $"{mobileBaseUrl}/mtk/{contract.AssigneeLinkKey}";
            }

            return contracts;
        }

        public List<AreaContract> AllAreas()
        {
            return _apiService.Get<List<AreaContract>>("areas","");
        }
    }
}