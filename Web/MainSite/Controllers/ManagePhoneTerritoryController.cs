
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    public partial class ManagePhoneTerritoryController : Controller
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";
        
        readonly AreaService _areaService;
        readonly List<string> AllowedRoles = new()
        {
            "INVITED",
            "ADMINISTRATOR",
            "ADDED"
        };

        readonly MainDbContext _database;
        readonly Services.IAuthorizationServiceDeprecated _authorizationService;
        private readonly IUserFromApiService _userFromApiService;
        readonly WebUIOptions _options;

        public ManagePhoneTerritoryController(
            MainDbContext database,
            Services.IAuthorizationServiceDeprecated authorizationService,
            IUserFromApiService userFromApiService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
            _database = database;
            _authorizationService = authorizationService;
            _userFromApiService = userFromApiService;
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

                List<ManagePhoneTerritorIndexPageUser> users = _database
                    .TerritoryUser
                    .Where(u => AllowedRoles.Contains(u.Role.ToUpper()))
                    .OrderBy(u => u.GivenName)
                    .Select(u => new ManagePhoneTerritorIndexPageUser
                        {
                            Id = u.Id.ToString(),
                            FullName = string.Join(' ', u.GivenName, u.Alias, u.Surname)
                        })
                    .ToList();

                users.Insert(0, new ManagePhoneTerritorIndexPageUser
                    {
                        FullName = "Shared",
                        Id = "SHARED"
                    });

                ManagePhoneTerritoryIndexPage report = new()
                {
                    DefaultSourceDocumentId = _options.DefaultPhoneTerritorySourceDocumentId,
                    DefaultSourceSheetName = _options.DefaultPhoneTerritorySourceSheetName,
                    // TODO: DefaultOwner = ...
                    Users = users,
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userFromApiService.ByEmail(User.Identity.Name);
                if (user != null && (user.IsActive ?? false) && user.CanAssignTerritories)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
