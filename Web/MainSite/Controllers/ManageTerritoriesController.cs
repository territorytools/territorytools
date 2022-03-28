using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    public partial class ManageTerritoriesController : AuthorizedController
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";
        private readonly IConfiguration _configuration;
        IAccountLists accountLists;

        public ManageTerritoriesController(
            MainDbContext database,
            IConfiguration configuration,
            IAccountLists accountLists,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            ITerritoryAssignmentService assignmentService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                database,
                localizer,
                credentials,
                authorizationService,
                albaCredentialService,
                assignmentService,
                optionsAccessor)
        {
            _configuration = configuration;
            this.accountLists = accountLists;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var users = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var areas = new List<Models.Area>();
                string areaNamesString = _configuration.GetValue<string>("TT_AreaNames");
                if (!string.IsNullOrWhiteSpace(areaNamesString))
                {
                    string[] areaRows = areaNamesString.Split(";", StringSplitOptions.RemoveEmptyEntries);
                    foreach (string areaRow in areaRows)
                    {
                        string[] areaRowParts = areaRow.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        Models.Area area = new Models.Area
                        {
                            Code = areaRowParts.Length > 0 ? areaRowParts[0] : "",
                            Name = areaRowParts.Length > 1 ? areaRowParts[1] : "",
                            Parent = areaRowParts.Length > 2 ? areaRowParts[2] : "",
                        };

                        areas.Add(area);
                    }
                }

                var parents = areas.GroupBy(a => a.Parent).Select(a => a.Key).ToList();
                foreach(var parent in parents)
                {
                    areas.Add(new Models.Area { Code = "PARENT-" + parent, Parent = parent });
                }

                areas = areas
                    .OrderBy(a => a.Parent)
                    .ThenBy(a => a.Name)
                    .ToList();

                var report = new ReportIndexPage()
                {
                    Users= users,
                    Areas = areas
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult AddressSearch(string searchText)
        {
            try
            {
                if(string.IsNullOrEmpty(searchText))
                {
                    return View(new AddressSearchPage());
                }

                if (!IsUser())
                {
                    return Forbid();
                }

                var connection = GetAlbaConnection();

                List<AddressSearchResult> addresses = new List<AddressSearchResult>();

                var resultString = connection.DownloadString(
                    RelativeUrlBuilder.SearchAddresses(
                        accountId: connection.AccountId,
                        addressesPerPage: 1000,
                        searchText: searchText));

                string text = AddressExportParser.Parse(resultString);

                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "\t",
                    BadDataFound = null
                };

                using (var reader = new StringReader(text))
                using (var csv = new CsvReader(reader, configuration))
                {
                    var addressExport = csv.GetRecords<AlbaAddressExport>().ToList();
                    foreach(AlbaAddressExport address in addressExport)
                    {
                        var viewModel = new AddressSearchResult
                        {
                            Territory = address.Territory_number,
                            LanguageStatus = $"{address.Language}/{address.Status}",
                            Name = address.Name,
                            Address = address.OneLine()
                        };
                        addresses.Add(viewModel);
                    }
                }

                var report = new AddressSearchPage()
                {
                    Addresses = addresses
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
