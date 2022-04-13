
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;

using System.Linq;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    public partial class ManagePhoneTerritoryController : AuthorizedController
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";
        private readonly AreaService _areaService;
        IAccountLists accountLists;
        WebUIOptions _options;

        public ManagePhoneTerritoryController(
            MainDbContext database,
            AreaService areaService,
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
            _areaService = areaService;
            this.accountLists = accountLists;
            _options = optionsAccessor.Value;
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

                var users = database.TerritoryUser
                    .OrderBy(u => u.GivenName)
                    .ToList();

                var report = new ManagePhoneTerritoryIndexPage()
                {
                    DefaultSourceDocumentId = _options.DefaultPhoneTerritorySourceDocumentId,
                    DefaultSourceSheetName = _options.DefaultPhoneTerritorySourceSheetName,
                    //DefaultOwner = ...
                    Users = users,
                    Areas = _areaService.All()
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
