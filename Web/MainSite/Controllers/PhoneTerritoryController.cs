using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/phoneterritory")]
    public class PhoneTerritoryController : Controller
    {
        private readonly IPhoneTerritoryService _phoneTerritoryService;
        readonly MainDbContext _mainDbContext;
        readonly ILogger<PhoneTerritoryController> _logger;
        readonly WebUIOptions _options;

        public PhoneTerritoryController(
            IPhoneTerritoryService phoneTerritoryService,
            MainDbContext mainDbContext,
            ILogger<PhoneTerritoryController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _phoneTerritoryService = phoneTerritoryService;
            _mainDbContext = mainDbContext;
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpPost("create")]
        public ActionResult<PhoneTerritoryCreateResult> Create(
            string sourceDocumentId, 
            string sourceSheetName,
            string territoryNumber, 
            string userId)
        {
            PhoneTerritoryCreateResult result = _phoneTerritoryService
                .CreateTerritory(
                    sourceDocumentId, 
                    sourceSheetName, 
                    territoryNumber, 
                    userId);

            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
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
