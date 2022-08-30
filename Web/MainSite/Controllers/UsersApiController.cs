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
        private readonly IApiService _apiService;
        readonly ILogger _logger;

        public UsersApiController(
            IApiService apiService,
            ILogger<AssignmentsApiController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<UserContract>> Users(bool? active)
        {
            _logger.LogInformation($"Getting all users");
            string queryString = string.Empty;
            
            if (active.HasValue)
                queryString = $"?active={active}";

            return _apiService
                .ApiCall<List<UserContract>>("users", queryString);
        }
    }
}
