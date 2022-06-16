using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/assignments")]
    public class AssignmentsApiController : Controller
    {
        readonly IAssignLatestService _assignmentService;
        readonly IUserService _userService;
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly KmlFileService _kmlFileService;
        readonly AssignmentsCsvFileService _assignmentsCsvFileService;
        readonly IAlbaCredentialService _albaCredentialService;
        readonly ITerritoryAssignmentService _territoryAssignmentService;
        readonly ILogger _logger;
        readonly WebUIOptions _options;

        public AssignmentsApiController(
            IAssignLatestService assignmentService,
            IUserService userService,
            ICombinedAssignmentService combinedAssignmentService,
            KmlFileService kmlFileService,
            AssignmentsCsvFileService assignmentsCsvFileService,
            IAlbaCredentialService albaCredentialService,
            ITerritoryAssignmentService territoryAssignmentService,
            ILogger<AssignmentsApiController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _assignmentService = assignmentService;
            _userService = userService;
            _combinedAssignmentService = combinedAssignmentService;
            _kmlFileService = kmlFileService;
            _assignmentsCsvFileService = assignmentsCsvFileService;
            _albaCredentialService = albaCredentialService;
            _territoryAssignmentService = territoryAssignmentService;
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpGet("[action]")]
        public IActionResult Assign(int territoryId, int userId)
        {
            _territoryAssignmentService.Assign(territoryId, userId, User.Identity.Name);

            return Redirect($"/Home/AssignSuccess?territoryId={territoryId}&userName={User.Identity.Name}");
        }

        [HttpPost("latest")]
        public ActionResult<AssignmentResult> AssignLatest(
            string userName,
            int userId,
            [Range(1, 99)]
            int count = 1,
            string area = "*")
        {
            var request = new AssignmentLatestRequest
            {
                RealUserName = User.Identity.Name,
                UserId = userId,
                Count = count,
                Area = area
            };

            AssignmentResult result = _assignmentService.AssignmentLatest(request);

            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("[action]")]
        public IActionResult Unassign(int territoryId)
        {
            _territoryAssignmentService.Unassign(territoryId, User.Identity.Name);

            return Redirect($"/Home/UnassignSuccess?territoryId={territoryId}");
        }

        [HttpGet("[action]")]
        public IActionResult Complete(int territoryId)
        {
            _territoryAssignmentService.Complete(territoryId, User.Identity.Name);

            return Redirect($"/Home/UnassignSuccess?territoryId={territoryId}");
        }

        [HttpGet("[action]")]
        public IEnumerable<AlbaAssignmentValues> NeverCompleted()
        {
            return _territoryAssignmentService.NeverCompleted(User.Identity.Name);
        }

        [HttpGet("[action]")]
        public IEnumerable<Publisher> ByPublisher()
        {
            return _territoryAssignmentService.ByPublisher(User.Identity.Name);
        }

        [AllowAnonymous]
        [Route("/ClockTick")]
        public void ClockTick()
        {
            _logger.LogInformation("ClockTick: Not really loading territory assigments...");
            // TODO: Fix clock tick, need a user or something here
            //Load();
        }

        [HttpGet("[action]")]
        public IActionResult LoadAssignments()
        {
            _combinedAssignmentService.LoadAssignments(User.Identity.Name);
            ////LoadForCurrentAccount();
            
            // TODO: Use with React or other UI
            // return new LoadAssignmentsResult() { Successful = true };
            return Redirect("/Home/Load");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadCsvFiles()
        {
            _assignmentsCsvFileService.Download(User.Identity.Name);

            return Redirect("/Report/Index");
        }

        [HttpGet("[action]")]
        public IActionResult DownloadBorderKmlFiles()
        {
            _kmlFileService.Generate(User.Identity.Name);

            return Redirect("/Report/Index");
        }
    }
}
