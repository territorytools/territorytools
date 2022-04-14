using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [AllowAnonymous]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        readonly ILogger logger;
        readonly WebUIOptions _options;

        public SmsController(
            ILogger<SmsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpGet("receive")]
        public ActionResult<PhoneTerritoryCreateResult> Receive(string id, string timestamp, string from, string to, string message)
        {
            logger.LogInformation($"Received SMS: id: {id}, timestamp: {timestamp}, to: {to}, from: {from}, message: {message}");

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
