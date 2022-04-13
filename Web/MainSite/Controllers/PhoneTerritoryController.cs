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
    [Route("api/phoneterritory")]
    public class PhoneTerritoryController : Controller
    {
        private readonly MainDbContext _mainDbContext;
        readonly IAlbaCredentialService albaCredentialService;
        private readonly AreaService _areaService;
        readonly ITerritoryAssignmentService territoryAssignmentService;
        readonly ILogger logger;
        readonly WebUIOptions options;

        public PhoneTerritoryController(
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
            options = optionsAccessor.Value;
        }

        [HttpPost("create")]
        public ActionResult<PhoneTerritoryCreateResult> Create(string sourceDocumentId, string sourceSheetName, string territoryNumber, string userId)
        {
            if(!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest($"Badly formatted userID {userId}.  Select a valid user.");
            }
            
            var user = _mainDbContext.TerritoryUser.FirstOrDefault(u => u.Id == userGuid);

            if(user == null)
            {
                return BadRequest($"No such userID {userId}");
            }

            var service = new SheetExtractor();
            var request = new SheetExtractionRequest()
            {
                FromDocumentId = sourceDocumentId,
                PublisherEmail = user.Email,
                FromSheetName = sourceSheetName,
                OwnerEmail = user.Email,
                PublisherName = user.GivenName,
                TerritoryNumber = territoryNumber,
                SecurityToken = System.IO.File.ReadAllText("./GoogleApi.secrets.json")
            };

            string uri = service.Extract(request);

            return Ok(
               new PhoneTerritoryCreateResult
               {
                   Success = true,
                   Message = $"Successfully created and assigned to {user.Email}",
                   Items = new List<PhoneTerritoryCreateItem>
                   {
                       new PhoneTerritoryCreateItem
                       { 
                           Description = $"Territory {territoryNumber}", 
                           Uri = uri
                       }
                   }
               });
        }
    }

    public class PhoneTerritoryCreateResult
    {
        public bool Success { get; internal set; }
        public string Message { get; set; }
        public List<PhoneTerritoryCreateItem> Items { get; set; } = new List<PhoneTerritoryCreateItem>();
    }
    public class PhoneTerritoryCreateItem
    {
        public string Uri { get; set; }
        public string Description { get; internal set; }
    }
}
