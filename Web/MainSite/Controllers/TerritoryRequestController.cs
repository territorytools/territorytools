using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers.Models;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [AllowAnonymous]
    [Route("api/territory-request")]
    public class TerritoryRequestController : Controller
    {
        readonly IAlbaCredentialService albaCredentialService;
        readonly ITerritoryAssignmentService territoryAssignmentService;
        readonly ILogger logger;
        readonly WebUIOptions options;

        public TerritoryRequestController(
            IAlbaCredentialService albaCredentialService,
            ITerritoryAssignmentService territoryAssignmentService,
            IAlbaCredentials credentials,
            ILogger<AssignmentsApiController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.albaCredentialService = albaCredentialService;
            this.territoryAssignmentService = territoryAssignmentService;
            this.logger = logger;
            options = optionsAccessor.Value;
        }

        [HttpGet]
        public IActionResult Request(string fullName, int count = 1)
        {
            Credentials credentials = albaCredentialService
                .GetCredentialsFrom(User.Identity.Name);

            //AlbaConnection client = AlbaConnection.From(options.AlbaHost);
            //client.Authenticate(credentials);

            int territoryId = 0;
            int userId = 0;

            //string result = client.DownloadString(
            //    RelativeUrlBuilder.AssignTerritory(
            //        territoryId,
            //        userId,
            //        DateTime.Now));

            return Ok("Yes");

            //var myUser = GetUsersFor(credentials.AlbaAccountId)
            //    .FirstOrDefault(u => u.Id == userId);
            
            //string userName = "Somebody";
            //if (myUser != null)
            //{
            //    userName = myUser.Name;
            //}

            //// This should refresh the mobile territory link to send to the user
            //LoadForCurrentAccount();

            //return Redirect($"/Home/AssignSuccess?territoryId={territoryId}&userName={userName}");
        }
    }
}
