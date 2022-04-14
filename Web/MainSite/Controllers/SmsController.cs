using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        private readonly MainDbContext _mainDbContext;
        readonly IAlbaCredentialService albaCredentialService;
        private readonly AreaService _areaService;
        readonly ITerritoryAssignmentService territoryAssignmentService;
        readonly ILogger logger;
        readonly WebUIOptions _options;

        public SmsController(
            MainDbContext mainDbContext,
            IAlbaCredentialService albaCredentialService,
            AreaService areaService,
            ITerritoryAssignmentService territoryAssignmentService,
            IAlbaCredentials credentials,
            ILogger<AssignmentsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _mainDbContext = mainDbContext;
            this.albaCredentialService = albaCredentialService;
            _areaService = areaService;
            this.territoryAssignmentService = territoryAssignmentService;
            this.logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpPost("receive")]
        public ActionResult<PhoneTerritoryCreateResult> Receive(string id, string timestamp, string from, string to, string message)
        {
            var service = new SheetExtractor();
            var sms = new SmsMessage()
            {
                FromDocumentId = _options.DefaultPhoneTerritorySourceDocumentId,
                Id = id,
                To = to,
                From = from,
                Timestamp = timestamp,
                Message = message,
                SecurityToken = System.IO.File.ReadAllText("./GoogleApi.secrets.json")
            };

            service.LogMessage(sms);

            return Ok();
        }
    }
}
