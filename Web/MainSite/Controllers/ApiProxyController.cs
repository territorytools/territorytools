using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;

namespace TerritoryTools.Web.MainSite.Controllers
{
    [ApiController]
    [Route("/api")]
    public class ApiProxyController : ControllerBase
    {
        readonly WebUIOptions _options;
        readonly ILogger _logger;

        public ApiProxyController(
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<AssignmentsApiController> logger)
        {
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        [Route("{a?}/{b?}/{c?}/{d?}/{e?}/{f?}")]
        public void ForwardToApi()
        {
            if(!User.Identity.IsAuthenticated)
            {
                //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-6.0#httpcontext-isnt-thread-safe
                //HttpContext isn't thread safe
                ControllerContext.HttpContext.Response.StatusCode = 401;
                return;
            }

            string baseUrl = _options.TerritoryApiBaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            string path = ControllerContext.HttpContext.Request.Path.Value;
            string queryString = ControllerContext.HttpContext.Request.QueryString.Value;

            string pathPrefix = "/api/"; // Same as controler Route attribute but with a trailing slash
            if (path.StartsWith(pathPrefix))
            {
                path = $"{path.Substring(pathPrefix.Length)}";
            }

            HttpClient client = new();
            HttpRequestMessage requestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseUrl}/{path}{queryString}");

            requestMessage.Headers.Add("x-territory-tools-user", User.Identity.Name);

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
