using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [ApiController]
    [Route("/api/v2")]
    public class ApiProxyController : ControllerBase
    {
        readonly ILogger _logger;

        public ApiProxyController(
            ILogger<AssignmentsApiController> logger)
        {
            _logger = logger;
        }

        [Route("{a?}/{b?}/{c?}/{d?}/{e?}/{f?}")]
        public void ForwardToApi()
        {
            if(!User.Identity.IsAuthenticated)
            {
                ControllerContext.HttpContext.Response.StatusCode = 401;
                return;
            }

            string path = ControllerContext.HttpContext.Request.Path.Value;
            string queryString = ControllerContext.HttpContext.Request.QueryString.Value;

            if (path.StartsWith("/api/v2/"))
            {
                path = $"{path.Substring(8)}";
            }

            HttpClient client = new();
            HttpRequestMessage requestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"http://localhost:5123/{path}{queryString}");

            var context = ControllerContext.HttpContext;
            using (var responseMessage = client.SendAsync(
                requestMessage, 
                HttpCompletionOption.ResponseHeadersRead, 
                context.RequestAborted).Result)
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (var header in responseMessage.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
                context.Response.Headers.Remove("transfer-encoding");
                responseMessage.Content.CopyToAsync(context.Response.Body).Wait();
            }
        }
    }
}
