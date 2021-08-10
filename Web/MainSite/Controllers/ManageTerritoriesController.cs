using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TerritoryTools.Web.MainSite.Services;
using TerritoryTools.Web.Data;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Models;

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
    }
}
