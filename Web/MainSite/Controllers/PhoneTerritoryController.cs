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
        readonly WebUIOptions _options;

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
            _options = optionsAccessor.Value;
        }

        [HttpPost("create")]
        public ActionResult<PhoneTerritoryCreateResult> Create(string sourceDocumentId, string sourceSheetName, string territoryNumber, string userId)
        {
            string userEmail = string.Empty;
            string userFullName = string.Empty;

            bool userIdIsGuid = Guid.TryParse(userId, out Guid userGuid);

            if (userId != "SHARED" && !userIdIsGuid)
            {
                return BadRequest($"Badly formatted userID {userId}.  Select a valid user.");
            }
            else if (userId == "SHARED")
            {
                userEmail = _options.SharedPhoneTerritoryEmailAddress;
                userFullName = _options.SharedPhoneTerritoryFullName;
            }
            else
            {
                var user = _mainDbContext.TerritoryUser.FirstOrDefault(u => u.Id == userGuid);
                if (user == null)
                {
                    return BadRequest($"No such userID {userId}");
                }
                else
                {
                    userEmail = user.Email;
                    userFullName = $"{user.GivenName} {user.Surname}".Trim();
                }
            }
            
            var service = new SheetExtractor();
            var request = new SheetExtractionRequest()
            {
                FromDocumentId = sourceDocumentId,
                PublisherEmail = userEmail,
                FromSheetName = sourceSheetName,
                OwnerEmail = userEmail,
                PublisherName = userFullName,
                TerritoryNumber = territoryNumber,
                SecurityToken = System.IO.File.ReadAllText("./GoogleApi.secrets.json")
            };

            string uri = service.Extract(request);

            return Ok(
               new PhoneTerritoryCreateResult
               {
                   Success = true,
                   Message = $"Successfully created and assigned to {userEmail}",
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

        [HttpPost("add-writer")]
        public ActionResult<PhoneTerritoryCreateResult> AddWriter(string documentId, string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest($"Badly formatted userID {userId}.  Select a valid user.");
            }

            var user = _mainDbContext.TerritoryUser.FirstOrDefault(u => u.Id == userGuid);

            if (user == null)
            {
                return BadRequest($"No such userID {userId}");
            }

            var service = new SheetExtractor();
            var request = new AddSheetWriterRequest()
            {
                DocumentId = documentId,
                UserEmail = user.Email,
                SecurityToken = System.IO.File.ReadAllText("./GoogleApi.secrets.json")
            };

            string uri = service.AddSheetWriter(request);

            return Ok(
               new AddWriterResult
               {
                   Success = true,
                   Message = $"Successfully added writer: {user.Email}",
                   Uri = uri
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

    public class AddWriterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Uri { get; set; } = string.Empty;
    }
}
