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
    [Route("api/areas")]
    public class AreasApiController : Controller
    {
        private readonly ITerritoryApiService _territoryApiService;
        readonly ILogger _logger;

        public AreasApiController(
            ITerritoryApiService territoryApiService,
            ILogger<AssignmentsApiController> logger)
        {
            _territoryApiService = territoryApiService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<AreaContract>> Areas()
        {
            _logger.LogInformation($"Getting all areas");
            return _territoryApiService.AllAreas();
        }
    }
}
