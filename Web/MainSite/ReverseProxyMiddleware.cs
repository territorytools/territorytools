using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite
{

    // From https://github.com/andychiare/netcore2-reverse-proxy
    public class ReverseProxyMiddleware
    {
       // private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IApiService _apiService;
        private readonly RequestDelegate _nextMiddleware;
        private readonly ILogger<ReverseProxyMiddleware> _logger;
        private readonly string _baseUrl;

        public ReverseProxyMiddleware(
            IHttpClientWrapper httpClientWrapper,
            IApiService apiService,
            RequestDelegate nextMiddleware, 
            IConfiguration configuration,
            ILogger<ReverseProxyMiddleware> logger)
        {
            _baseUrl = configuration.GetValue<string>("TerritoryApiBaseUrl");
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new ArgumentNullException("baseUrl");
            }
            _httpClientWrapper = httpClientWrapper;
            _apiService = apiService;
            _nextMiddleware = nextMiddleware;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogTrace($"ReverseProxy: Incoming: Path: {context.Request.Path} QueryString: {context.Request.QueryString}");
            string isAuthMessage = $"{context?.User?.Identity?.IsAuthenticated ?? false}";
            string userNameMessage = $"{context?.User?.Identity?.Name}";
            string userAuthMessage = $"User IsAuthenticated: {isAuthMessage} Name: {userNameMessage}";
            _logger.LogTrace(userAuthMessage);

            Uri targetUri = BuildTargetUri(context.Request);

            // Security bypass Starts:  Comment this block out
            //if (targetUri != null)
            //{
            //    HttpRequestMessage targetRequestMessage = CreateTargetMessage(context, targetUri);
            //    using (HttpResponseMessage responseMessage = await _httpClientWrapper.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            //    {
            //        context.Response.StatusCode = (int)responseMessage.StatusCode;
            //        CopyFromTargetResponseHeaders(context, responseMessage);
            //        await responseMessage.Content.CopyToAsync(context.Response.Body);
            //    }
            //    return;
            //}
            //await _nextMiddleware(context);
            //return;
            // End of Security bypass.  Comment this block out


            if (targetUri != null)
            {
                _logger.LogTrace($"ReverseProxy: Forwarded to: {targetUri}");
                if(context.Request.Query.TryGetValue("mtk", out StringValues value) && !string.IsNullOrWhiteSpace(value))
                {
                    _logger.LogTrace($"MTK query param detected: {value}");
                    TerritoryLinkContract link = _apiService.Get<TerritoryLinkContract>($"territory-links/{value}", "");
                    if (link == null && !context.User.Identity.IsAuthenticated)
                    {
                        string message = $"ReverseProxy: Territory key does not exist";
                        context.Response.StatusCode = 401;
                        context.Response.Body = StreamFrom(message);
                        _logger.LogTrace(message);
                        return;
                    }
                    else if ((!link.Successful || link.Expires != null && link.Expires < DateTime.UtcNow) 
                        && !context.User.Identity.IsAuthenticated)
                    {
                        string message = $"ReverseProxy: Territory key expired {link.Expires}";
                        context.Response.StatusCode = 403;
                        context.Response.Body = StreamFrom(message);
                        _logger.LogTrace(message);
                        return;
                    }
                }
                else if (!context.User.Identity.IsAuthenticated)
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-6.0#httpcontext-isnt-thread-safe
                    //HttpContext isn't thread safe
                    string message = $"ReverseProxy: User Not Authenticated and MTK query param not detected";
                    context.Response.StatusCode = 401;
                    context.Response.Body = StreamFrom(message);
                    _logger.LogTrace(message);
                    return;
                }

                HttpRequestMessage targetRequestMessage = CreateTargetMessage(context, targetUri);

                using (HttpResponseMessage responseMessage = await _httpClientWrapper.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
                {
                    _logger.LogTrace($"HttpResponseMessage: StatusCode: {(int)responseMessage.StatusCode}");
                    context.Response.StatusCode = (int)responseMessage.StatusCode;
                    CopyFromTargetResponseHeaders(context, responseMessage);
                    await responseMessage.Content.CopyToAsync(context.Response.Body);
                }

                _logger.LogTrace($"ReverseProxy: Done");
                return;
            }
            else
            {
                _logger.LogTrace($"ReverseProxy: Not forwarded");
            }

            await _nextMiddleware(context);
        }

        Stream StreamFrom(string text)
        {
            MemoryStream stream = new ();
            StreamWriter writer = new (stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(context.Request.Method);
            
            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            string requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            string headers = string.Empty;
            foreach (var header in context.Request.Headers)
            {
                //_logger.LogTrace($"context.Request.Headers: {header.Key}: {string.Join(',', header.Value.ToArray())}");
                headers += $"{header.Key}: {string.Join(',', header.Value.ToArray())}\n";
                // Don't allow this header to be added by the client
                if (!"x-territory-tools-user".Equals(header.Key, StringComparison.OrdinalIgnoreCase))
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
                else
                {
                    _logger.LogTrace($"Removing x-territory-tools-user header! Value: {string.Join(',', header.Value.ToArray())}");
                }
            }

            _logger.LogTrace($"context.Request.Headers.List: \n{headers}\n(***User Header) {context.User.Identity.Name}");            
            requestMessage.Headers.TryAddWithoutValidation("x-territory-tools-user", context.User.Identity.Name);
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                //_logger.LogTrace($"CopyFromTargetResponseHeaders: Headers: {header.Key}: {string.Join(',', header.Value.ToArray())}");
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                //_logger.LogTrace($"CopyFromTargetResponseHeaders: Content.Headers: {header.Key}: {string.Join(',', header.Value.ToArray())}");
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private Uri BuildTargetUri(HttpRequest request)
        {
            Uri targetUri = null;

            if (request.Path.StartsWithSegments("/api", out var remainingPath) 
                && !request.Path.StartsWithSegments("/api/personal-territories")
                && !request.Path.StartsWithSegments("/api/phoneterritory"))
            {
                targetUri = new Uri($"{_baseUrl}{remainingPath}{request.QueryString}");
            }

            return targetUri;
        }
    }

    public static class ReverseProxyMiddlewareExtensions
    {
        public static IApplicationBuilder AddReverseProxy(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReverseProxyMiddleware>();
        }
    }
}
