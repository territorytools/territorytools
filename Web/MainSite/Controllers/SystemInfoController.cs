
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{

    public class SystemInfo
    {
        public string? ApiDatabaseName { get; set; }
        public string? ApiGitCommit { get; set; }
        public DatabaseInfo DatabaseInfo { get; set; } = new();
        public string? MachineName { get; set; }
        public WebUIOptions WebUIOptions { get; set; } = new();
    }

    [ApiController]
    [Route("system-info")]
    public partial class SystemInfoController : Controller
    {
        private readonly IDatabaseInfoService _databaseInfoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SystemInfoController> _logger;

        private SystemInfo _systemInfo = new();

        public SystemInfoController(
            IDatabaseInfoService databaseInfoService,
            IConfiguration configuration,
            ILogger<SystemInfoController> logger)
        {
            _databaseInfoService = databaseInfoService;
            _configuration = configuration;
            _logger = logger;

            _configuration.Bind(_systemInfo.WebUIOptions);
        }

        [HttpGet]
        public ActionResult<SystemInfo> Get()
        {
            _systemInfo.DatabaseInfo = _databaseInfoService.Info();
            _systemInfo.MachineName = Environment.MachineName;
            _systemInfo.ApiGitCommit = _configuration.GetValue<string>("GitCommit");

            return Ok(_systemInfo);
        }
    }
}