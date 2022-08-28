using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    // TODO: Try this: [ApiController]
    [Route("api/territories")]
    public class TerritoriesApiController : Controller
    {
        private readonly ITerritoriesForUserService _territoriesForUserService;
        readonly ILogger _logger;

        public TerritoriesApiController(
            ITerritoriesForUserService territoriesForUserService,
            ILogger<AssignmentsApiController> logger)
        {
            _territoriesForUserService = territoriesForUserService;
            _logger = logger;
        }

        [HttpGet("checked-out-to")]
        public ActionResult<List<TerritoryContract>> CheckedOutTo(string userFullName)
        {
            _logger.LogInformation($"Getting territories for user full name '{userFullName}'");
            return _territoriesForUserService.CheckedOutTo(userFullName);
        }

        [HttpGet("all")]
        public ActionResult<List<TerritoryContract>> All(string userFullName)
        {
            _logger.LogInformation($"Getting territories for user full name '{userFullName}'");
            return _territoriesForUserService.All();
        }
    }
}
