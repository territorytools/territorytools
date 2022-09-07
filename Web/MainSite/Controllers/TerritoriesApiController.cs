using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    // TODO: Try this: [ApiController]
    [Route("api/territories")]
    public class TerritoriesApiController : Controller
    {
        private readonly ITerritoryApiService _territoryApiService;
        readonly ILogger _logger;

        public TerritoriesApiController(
            ITerritoryApiService territoriesForUserService,
            ILogger<AssignmentsApiController> logger)
        {
            _territoryApiService = territoriesForUserService;
            _logger = logger;
        }

        [HttpGet("checked-out-to")]
        public ActionResult<List<TerritoryContract>> CheckedOutTo(string userFullName)
        {
            _logger.LogInformation($"Getting territories for user full name '{userFullName}'");
            return _territoryApiService.TerritoriesCheckedOutTo(userFullName);
        }

        [HttpGet("all")]
        public ActionResult<List<TerritoryContract>> All(string filter)
        {
            _logger.LogInformation($"Getting territories with filter '{filter}'");
            if(string.IsNullOrWhiteSpace(filter))
            {
                return _territoryApiService.AllTerritories();
            }

            var filtered = _territoryApiService.AllTerritories()
                .Where(t => t.Number != null && t.Number.ToUpper().Contains(filter.ToUpper()) 
                || t.SignedOutTo != null && t.SignedOutTo.ToUpper().Contains(filter.ToUpper()) 
                || t.Description != null && t.Description.ToUpper().Contains(filter.ToUpper()))
                .ToList();
            
            return filtered;
        }
    }
}
