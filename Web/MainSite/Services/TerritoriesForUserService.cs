using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoriesForUserService
    {
        List<TerritoryContract> CheckedOutTo(string userFullName);
    }

    public class TerritoriesForUserService : ITerritoriesForUserService
    {
        readonly IUserService _userService;
        readonly ITerritoryAssignmentService _territoryAssignmentService;
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly IAlbaCredentialService _albaCredentialService;
        readonly AreaService _areaService;
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;
        readonly WebUIOptions _options;

        public TerritoriesForUserService(
            IUserService userService,
            ITerritoryAssignmentService territoryAssignmentService,
            ICombinedAssignmentService combinedAssignmentService,
            IAlbaCredentialService albaCredentialService,
            AreaService areaService,
            ILogger<AssignLatestService> logger,
            IOptions<WebUIOptions> optionsAccessor,
            IConfiguration configuration)
        {
            _userService = userService;
            _territoryAssignmentService = territoryAssignmentService;
            _combinedAssignmentService = combinedAssignmentService;
            _albaCredentialService = albaCredentialService;
            _areaService = areaService;
            _logger = logger;
            _configuration = configuration;
            _options = optionsAccessor.Value;
        }

        public List<TerritoryContract> CheckedOutTo(string userFullName)
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
    }
}