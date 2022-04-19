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

            if((message??"").ToLower().Contains("territory"))
            {
                logger.LogInformation("Territory related message detected. Responding...");
                var client = new HttpClient();
                string smsResponseMessage = "Your territory request has been logged, thank you.".Replace(" ","+");
                string uri = $"https://voip.ms/api/v1/rest.php?api_username={_options.SmsApiUserName}&api_password={_options.SmsApiPassword}&method=sendSMS&did={_options.SmsFromPhoneNumber}&dst={from}&message={smsResponseMessage}";
                //var req = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
                logger.LogInformation($"URI: {uri}");
                var response = client.GetAsync(uri).Result;
                logger.LogInformation($"Response: {response.StatusCode} Message: {response.Content}");
            }

            return Ok("ok");
        }
    }
}
