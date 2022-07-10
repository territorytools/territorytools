using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/phoneterritory")]
    public class PhoneTerritoryController : Controller
    {
        readonly IPhoneTerritoryCreationService _phoneTerritoryService;
        readonly IPhoneTerritoryAddWriterService _phoneTerritoryAddWriterService;
        readonly ILogger<PhoneTerritoryController> _logger;

        public PhoneTerritoryController(
            IPhoneTerritoryCreationService phoneTerritoryService,
            IPhoneTerritoryAddWriterService phoneTerritoryAddWriterService,
            ILogger<PhoneTerritoryController> logger)
        {
            _phoneTerritoryService = phoneTerritoryService;
            _phoneTerritoryAddWriterService = phoneTerritoryAddWriterService;
            _logger = logger;
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
            AddWriterResult result = _phoneTerritoryAddWriterService
                .AddWriter(documentId, userId);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
