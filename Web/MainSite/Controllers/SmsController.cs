using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.RegularExpressions;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [AllowAnonymous]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        private readonly IUserFromApiService _userFromApiService;
        readonly ISheetExtractor _sheetExtractor;
        readonly ILogger _logger;
        readonly WebUIOptions _options;

        public SmsController(
            IUserFromApiService userFromApiService,
            ISheetExtractor spreadSheetService,
            ILogger<SmsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _userFromApiService = userFromApiService;
            _sheetExtractor = spreadSheetService;
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpGet("receive")]
        public ActionResult<PhoneTerritoryCreateResult> Receive(string id, string timestamp, string from, string to, string message)
        {
            _logger.LogInformation($"Received SMS: id: {id}, timestamp: {timestamp}, to: {to}, from: {from}, message: {message}");

            string adminMessage = $"tt-sms:{from} message: {message}";
            if (!string.IsNullOrWhiteSpace(_options.SmsAdminRecipient)
                && Regex.IsMatch(_options.SmsAdminRecipient, @"^\d{10}$"))
            {
                SendMessage(_options.SmsAdminRecipient, adminMessage);
            }
            else
            {
                _logger.LogWarning("SmsAdminRecipient is not set, or it is not 10 digits.");
            }

            string response = $"Your message has been recieved by https://territorytools.org and forwarded to a territory servant.";
            SendMessage(from, response);

            return Ok("ok");
        }

        // Currently not used, but I think it will be super useful at some point
        public void LogToGoogleSheet(string id, string timestamp, string from, string to, string message)
        {
            var sms = new SmsMessage()
            {
                LogDocumentId = _options.SmsMessageLogDocumentId,
                LogSheetName = _options.SmsMessageLogSheetName,
                Id = id,
                To = to,
                From = from,
                Timestamp = timestamp,
                Message = message,
                SecurityToken = System.IO.File.ReadAllText("./secrets/GoogleApi.secrets.json")
            };

            _sheetExtractor.LogMessage(sms);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<PhoneTerritoryCreateResult> Send(string from, string to, string message)
        {
            _logger.LogInformation($"Sending SMS: to: {to}, from: {from}, message: {message}");
            if (string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                _logger.LogError("Cancel sending SMS user name was not suppied, unathorized.");
                return Unauthorized();
            }

            UserContract user = _userFromApiService.ByEmail(User.Identity.Name);
            if (!user.IsActive ?? false)
            {
                _logger.LogError("Cancel sending SMS user is not authorized");
                return Unauthorized();
            }

            if (!Regex.IsMatch(to, @"\d{10}"))
            {
                return BadRequest($"Bad phone number {to}");
            }

            SendMessage(to, message);

            return Ok();
        }

        private void SendMessage(string to, string message)
        {
            HttpClient client = new HttpClient();
            string uri = $"https://voip.ms/api/v1/rest.php?api_username={_options.SmsApiUserName}" +
                $"&api_password={_options.SmsApiPassword}" +
                $"&method=sendSMS" +
                $"&did={_options.SmsFromPhoneNumber}" +
                $"&dst={to}" +
                $"&message={message}";

            var response = client.GetAsync(uri).Result;
            _logger.LogInformation($"Response: {response.StatusCode} Message: {response.Content}");
        }
    }
}
