using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [AllowAnonymous]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        readonly ISheetExtractor _sheetExtractor;
        readonly ILogger _logger;
        readonly WebUIOptions _options;

        public SmsController(
            ISheetExtractor spreadSheetService,
            ILogger<SmsController> logger,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _sheetExtractor = spreadSheetService;
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [HttpGet("receive")]
        public ActionResult<PhoneTerritoryCreateResult> Receive(string id, string timestamp, string from, string to, string message)
        {
            _logger.LogInformation($"Received SMS: id: {id}, timestamp: {timestamp}, to: {to}, from: {from}, message: {message}");

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
    }
}
