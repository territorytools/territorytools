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
    [Route("api/users")]
    public class UsersApiController : Controller
    {
        private readonly ITerritoryApiService _territoryApiService;
        readonly ILogger _logger;

        public UsersApiController(
            ITerritoryApiService territoryApiService,
            ILogger<AssignmentsApiController> logger)
        {
            _territoryApiService = territoryApiService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<UserContract>> Users(bool? active)
        {
            _logger.LogInformation($"Getting all users");
            string queryString = string.Empty;
            
            if (active.HasValue)
                queryString = $"?active={active}";

            return _territoryApiService
                .ApiCall<List<UserContract>>("users", queryString);
        }
    }
}
