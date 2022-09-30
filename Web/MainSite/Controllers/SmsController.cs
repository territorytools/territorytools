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

            string adminMessage = $"from sms:{from} message: {message}";
            if (!string.IsNullOrWhiteSpace(_options.SmsAdminRecipient)
                && Regex.IsMatch(_options.SmsAdminRecipient, @"^\d{10}$"))
            {
                SendMessage(_options.SmsAdminRecipient, adminMessage);
            }
            else
            {
                _logger.LogWarning("No SmsAdminRecipient is set, or it is not 10 digits.");
            }

            var sms = new SmsMessage()
            {
                LogDocumentId = _options.SmsMessageLogDocumentId,
                LogSheetName = _options.SmsMessageLogSheetName,
                Id = id,
                To = to,
                From = from,
                Timestamp = timestamp,
                Message = message,
                SecurityToken = System.IO.File.ReadAllText("./GoogleApi.secrets.json")
            };

            _sheetExtractor.LogMessage(sms);

            if((message??"").ToLower().Contains("territory"))
            {
                _logger.LogInformation("Territory related message detected. Responding...");
                var client = new HttpClient();
                string smsResponseMessage = "Your territory request has been logged, thank you.".Replace(" ","+");
                string uri = $"https://voip.ms/api/v1/rest.php?api_username={_options.SmsApiUserName}&api_password={_options.SmsApiPassword}&method=sendSMS&did={_options.SmsFromPhoneNumber}&dst={from}&message={smsResponseMessage}";
                //var req = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
                _logger.LogInformation($"URI: {uri}");
                var response = client.GetAsync(uri).Result;
                _logger.LogInformation($"Response: {response.StatusCode} Message: {response.Content}");
            }

            return Ok("ok");
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult<PhoneTerritoryCreateResult> Send(string from, string to, string message)
        {
            _logger.LogInformation($"Sending SMS: to: {to}, from: {from}, message: {message}");
            UserContract user = _userFromApiService.ByEmail(User.Identity.Name);
            if (!user.IsActive ?? false)
            {
                _logger.LogError("Cancel sending SMS user is not authorized");
                return Unauthorized();
            }

            if (!Regex.IsMatch(to, @"\d\d\d\d\d\d\d\d\d\d"))
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
