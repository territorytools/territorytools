using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly ITerritoryApiService _territoryApiService;
        private readonly IUserFromApiService _userFromApiService;
        private readonly ICombinedAssignmentService _combinedAssignmentService;
        private readonly IUserService _userService;
        private readonly AreaService _areaService;
        private readonly Services.IAuthorizationServiceDeprecated _authorizationService;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly IAlbaAuthClientService _albaAuthClientService;
        private readonly IConfiguration _configuration;

        public ManageTerritoriesController(
            ITerritoryApiService territoryApiService,
            IUserFromApiService userFromApiService,
            ICombinedAssignmentService combinedAssignmentService,
            IUserService userService,
            AreaService areaService,
            IAccountLists accountLists,
            Services.IAuthorizationServiceDeprecated authorizationService,
            IAlbaCredentialService albaCredentialService,
            IAlbaAuthClientService albaAuthClientService,
            IConfiguration configuration,
            IOptions<WebUIOptions> optionsAccessor) : base(
                userFromApiService,
                userService,
                authorizationService,
                optionsAccessor)
        {
            _territoryApiService = territoryApiService;
            _userFromApiService = userFromApiService;
            _combinedAssignmentService = combinedAssignmentService;
            _userService = userService;
            _areaService = areaService;
            _authorizationService = authorizationService;
            _albaCredentialService = albaCredentialService;
            _albaAuthClientService = albaAuthClientService;
            _configuration = configuration;
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

                var report = new ReportIndexPage()
                {
                    GoogleMyMapLink = _configuration.GetValue<string>("GoogleMyMapLink"),
                    Areas = _areaService.All(),
                    UserName = User.Identity.Name,
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult All()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var page = new ManageTerritoriesAllPage()
                {
                };

                return View(page);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [Route("/t2/{territoryNumber}")]
        public IActionResult SingleV2(string territoryNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(territoryNumber))
                {
                    return View(new SingleTerritoryManagerPageV2());
                }

                if (!IsAdmin())
                {
                    return Forbid();
                }

                TerritoryContract territory = _territoryApiService.TerritoryByNumber(territoryNumber);

                if(territory == null)
                    return View(new SingleTerritoryManagerPage() {  Description = "Not Found"});

                ///var users = _userFromApiService

                var page = new SingleTerritoryManagerPageV2()
                {
                    Id = territory.Id,
                    Number = territoryNumber,
                    Description = territory.Description,
                    MobileLink = $"/mtk/{territory.ActiveLinkKey}",
                    PrintLink = "",
                    SignedOutTo = territory.SignedOutTo,
                    SignedOut = territory.SignedOut?.ToString("yyyy-MM-dd"),
                    LastCompletedBy = territory.LastCompletedBy,
                    LastCompleted = territory.LastCompleted?.ToString("yyyy-MM-dd"),
                    Kind = territory.AlbaKind?.ToString(),
                    Addresses = territory.AlbaKind ?? 0,
                    Status = territory.Status,
                    ///Users = users
                };

                return View(page);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [Route("/t/{territoryNumber}")]
        public IActionResult Single(string territoryNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(territoryNumber))
                {
                    return View(new SingleTerritoryManagerPage());
                }

                if (!IsAdmin())
                {
                    return Forbid();
                }

                var assignments = _combinedAssignmentService.GetAllAssignments(User.Identity.Name);

                var territory = assignments.Rows
                    .Where(t => string.Equals(t.Number, territoryNumber))
                    .SingleOrDefault();

                if(territory == null)
                    return View(new SingleTerritoryManagerPage() {  Description = "Not Found"});


                var users = _userService.GetUsers(User.Identity.Name)
                    .OrderBy(u => u.Name)
                    .ToList();

                var page = new SingleTerritoryManagerPage()
                {
                    Id = territory.Id,
                    Number = territoryNumber,
                    Description = territory.Description,
                    MobileLink = territory.MobileLink,
                    PrintLink = territory.PrintLink,
                    SignedOutTo = territory.SignedOutTo,
                    SignedOut = territory.SignedOut?.ToString("yyyy-MM-dd"),
                    LastCompletedBy = territory.LastCompletedBy,
                    LastCompleted = territory.LastCompleted?.ToString("yyyy-MM-dd"),
                    Kind = territory.Kind,
                    Addresses = territory.Addresses,
                    Status = territory.Status,
                    Users = users
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
