using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    public partial class ManageTerritoriesController : AuthorizedController
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";
        private readonly ICombinedAssignmentService _combinedAssignmentService;
        private readonly IUserService _userService;
        private readonly AreaService _areaService;
        private readonly Services.IAuthorizationService _authorizationService;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly IAlbaAuthClientService _albaAuthClientService;

        public ManageTerritoriesController(
            ICombinedAssignmentService combinedAssignmentService,
            IUserService userService,
            AreaService areaService,
            IAccountLists accountLists,
            Services.IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            IAlbaAuthClientService albaAuthClientService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                userService,
                authorizationService,
                optionsAccessor)
        {
            _combinedAssignmentService = combinedAssignmentService;
            _userService = userService;
            _areaService = areaService;
            _authorizationService = authorizationService;
            _albaCredentialService = albaCredentialService;
            _albaAuthClientService = albaAuthClientService;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                if (!User.Identity.IsAuthenticated || !_authorizationService.IsAdmin(User.Identity.Name))
                {
                    return Forbid();
                }

                var users = _userService.GetUsers(User.Identity.Name)
                    .OrderBy(u => u.Name)
                    .ToList();

                var report = new ReportIndexPage()
                {
                    Users= users,
                    Areas = _areaService.All()
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult Single(string territoryNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(territoryNumber))
                {
                    return View(new SingleTerritoryManagerPage());
                }

                if (!IsUser())
                {
                    return Forbid();
                }

                var assignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);

                var territory = assignments.Rows
                    .Where(t => string.Equals(t.Number, territoryNumber))
                    .SingleOrDefault();

                if(territory == null)
                    return View(new SingleTerritoryManagerPage() {  Description = "Not Found"});

                var page = new SingleTerritoryManagerPage()
                {
                    Number = territoryNumber,
                    Description = territory.Description,
                    MobileLink = territory.MobileLink,
                    SignedOutTo = territory.SignedOutTo

                };

                return View(page);
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

                var credentials = _albaCredentialService.GetCredentialsFrom(User.Identity.Name);
                var connection = _albaAuthClientService.AuthClient();
                connection.Authenticate(credentials);

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
