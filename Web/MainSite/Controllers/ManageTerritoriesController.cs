using Controllers.AlbaServer;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ManageTerritoriesController : AuthorizedController
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";

        IAccountLists accountLists;
        public ManageTerritoriesController(
            MainDbContext database,
            IAccountLists accountLists,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                database,
                localizer,
                credentials,
                authorizationService,
                albaCredentialService,
                optionsAccessor)
        {
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

                var report = new ReportIndexPage()
                {
                    Users= users,
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

                using (var reader = new StringReader(text))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = "\t";
                    csv.Configuration.BadDataFound = null;

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
