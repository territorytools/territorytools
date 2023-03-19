
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [ApiController]
    [Route("system-info")]
    public partial class SystemInfoController : Controller
    {
        private readonly IDatabaseInfoService _databaseInfoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SystemInfoController> _logger;

        private SystemInfo _systemInfo = new();
        private WebUIOptions _webUIOptions = new();

        public SystemInfoController(
            IDatabaseInfoService databaseInfoService,
            IConfiguration configuration,
            ILogger<SystemInfoController> logger)
        {
            _databaseInfoService = databaseInfoService;
            _configuration = configuration;
            _logger = logger;

            _configuration.Bind(_webUIOptions);
        }

        [HttpGet]
        public ActionResult<SystemInfo> Get()
        {
            _systemInfo.DatabaseInfo = _databaseInfoService.Info();
            _systemInfo.MachineName = Environment.MachineName;
            _systemInfo.ApiGitCommit = _configuration.GetValue<string>("GitCommit");

            // From WebUIOptions except secrets
            _systemInfo.AlbaHost = _webUIOptions.AlbaHost;
            _systemInfo.AlbaUserManagementHtmlPath = _webUIOptions.AlbaUserManagementHtmlPath;
            _systemInfo.AlbaUsersHtmlPath = _webUIOptions.AlbaUsersHtmlPath;
            _systemInfo.AlbaAssignmentsHtmlPath = _webUIOptions.AlbaAssignmentsHtmlPath;
            _systemInfo.AzureAppId = _webUIOptions.AzureAppId;
            _systemInfo.UrlShortenerDomain = _webUIOptions.UrlShortenerDomain;
            _systemInfo.CompletionMapUrl = _webUIOptions.CompletionMapUrl;
            _systemInfo.Users = _webUIOptions.Users;
            _systemInfo.AdminUsers = _webUIOptions.AdminUsers;
            _systemInfo.DefaultPhoneTerritorySourceDocumentId = _webUIOptions.DefaultPhoneTerritorySourceDocumentId;
            _systemInfo.DefaultPhoneTerritorySourceSheetName = _webUIOptions.DefaultPhoneTerritorySourceSheetName;
            _systemInfo.SharedPhoneTerritoryEmailAddress = _webUIOptions.SharedPhoneTerritoryEmailAddress;
            _systemInfo.SharedPhoneTerritoryFullName = _webUIOptions.SharedPhoneTerritoryFullName;
            _systemInfo.SmsApiUserName = _webUIOptions.SmsApiUserName;
            _systemInfo.SmsFromPhoneNumber = _webUIOptions.SmsFromPhoneNumber;
            _systemInfo.SmsMessageLogDocumentId = _webUIOptions.SmsMessageLogDocumentId;
            _systemInfo.SmsMessageLogSheetName = _webUIOptions.SmsMessageLogSheetName;
            _systemInfo.SmsAdminRecipient = _webUIOptions.SmsAdminRecipient;
            _systemInfo.TerritoryApiBaseUrl = _webUIOptions.TerritoryApiBaseUrl;
            _systemInfo.Features = _webUIOptions.Features;

            return Ok(_systemInfo);
        }
    }
}