using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [Authorize]
    [Route("api/assignments")]
    public class AssignmentsApiController : Controller
    {
        private readonly ITerritoryApiService _territoryApiService;
        readonly IAssignLatestService _assignmentService;
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly KmlFileService _kmlFileService;
        readonly AssignmentsCsvFileService _assignmentsCsvFileService;
        readonly ITerritoryAssignmentService _territoryAssignmentService;
        readonly ILogger _logger;

        public AssignmentsApiController(
            ITerritoryApiService territoriesForUserService,
            IAssignLatestService assignmentService,
            ICombinedAssignmentService combinedAssignmentService,
            KmlFileService kmlFileService,
            AssignmentsCsvFileService assignmentsCsvFileService,
            ITerritoryAssignmentService territoryAssignmentService,
            ILogger<AssignmentsApiController> logger)
        {
            _territoryApiService = territoriesForUserService;
            _assignmentService = assignmentService;
            _combinedAssignmentService = combinedAssignmentService;
            _kmlFileService = kmlFileService;
            _assignmentsCsvFileService = assignmentsCsvFileService;
            _territoryAssignmentService = territoryAssignmentService;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<TerritoryLinkContract> PostAssignment(
            string territoryNumber,
            string userName, // ignored
            int albaUserId)
        {
            if(!User.Identity.IsAuthenticated)
                return Unauthorized();

            TerritoryLinkContract result = _assignmentService.Assign(territoryNumber, userName, albaUserId, User.Identity.Name);

            if (!string.IsNullOrWhiteSpace(result.AlbaMobileTerritoryKey))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("[action]")]
        public ActionResult<TerritoryLinkContract> Assign([FromForm]
            string territoryNumber,
            string userName,
            int albaUserId,
            [Range(1, 99)]
            int count = 1,
            string area = "*")
        {
            TerritoryLinkContract result = _assignmentService.Assign(territoryNumber, userName, albaUserId, User.Identity.Name);

            if (!string.IsNullOrWhiteSpace(result.AlbaMobileTerritoryKey))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("oldest/alba")]
        public ActionResult<TerritoryLinkContract> AssignOldest(
            string userName,
            int userId,
            [Range(1, 99)]
            int count = 1,
            string area = "*")
        {
            var request = new AssignmentLatestRequest
            {
                RealUserName = User.Identity.Name,
                AlbaUserId = userId,
                Count = count,
                Area = area
            };

            TerritoryLinkContract result = _assignmentService.AssignmentLatestV2(request);

            if (!string.IsNullOrWhiteSpace(result.AlbaMobileTerritoryKey))
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

        [HttpDelete("v2")]
        public IActionResult UnassignV2(string territoryNumber)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            _territoryApiService.Unassign(territoryNumber, User.Identity.Name);
            return Ok();
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
        public IEnumerable<Services.Publisher> ByPublisher()
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
