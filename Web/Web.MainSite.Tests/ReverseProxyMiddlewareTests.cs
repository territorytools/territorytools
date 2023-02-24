using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Security.Principal;
using TerritoryTools.Web.MainSite;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;

namespace Web.MainSite.Tests
{
    public class ReverseProxyMiddlewareTests
    {
        Mock<IHttpClientWrapper> _mockHttpClient = new();
        Mock<IApiService> _mockApiService = new();
        bool _sendWasCalled = false;
        HttpRequestMessage? _actualRequest = null;
        TestServer? _server;

        public ReverseProxyMiddlewareTests()
        {
            // This is just copied, with only a few lines added from here:
            // https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0
            Dictionary<string, string> inMemoryConfigSettings = new()
            {
                { "TerritoryApiBaseUrl", "https://test" },
            };

            _mockHttpClient
              .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
              .Callback<HttpRequestMessage, HttpCompletionOption, CancellationToken>((m, o, t) =>
              {
                  _actualRequest = m;
                  _sendWasCalled = true;
              })
              .ReturnsAsync(new HttpResponseMessage());
            
            _mockApiService
                .Setup(s => s.Get<TerritoryLinkContract>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new TerritoryLinkContract() { Expires = DateTime.Now.AddDays(3) });

            var host = new HostBuilder()
               .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureAppConfiguration(app =>
                        {
                            app.AddInMemoryCollection(inMemoryConfigSettings);
                        })
                        .ConfigureServices(services =>
                        {
                            services.AddScoped<ITerritoryUserService, TerritoryUserService>();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<ReverseProxyMiddleware>(_mockApiService.Object, _mockHttpClient.Object);
                        });
                })
                .StartAsync().Result;

            _server = host.GetTestServer();
            _server.BaseAddress = new Uri("https://example.com/A/Path/");
        }

        [Fact]
        public async Task TestManyPossiblyIrrelevantContextProperties()
        {
            // This is just copied, with only a few lines added from here:
            // https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0

            var context = await _server!.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Post;
                c.Request.Path = "/and/file.txt";
                c.Request.QueryString = new QueryString("?and=query");
            });

            Assert.True(context.RequestAborted.CanBeCanceled);
            Assert.Equal(HttpProtocol.Http11, context.Request.Protocol);
            Assert.Equal("POST", context.Request.Method);
            Assert.Equal("https", context.Request.Scheme);
            Assert.Equal("example.com", context.Request.Host.Value);
            Assert.Equal("/A/Path", context.Request.PathBase.Value);
            Assert.Equal("/and/file.txt", context.Request.Path.Value);
            Assert.Equal("?and=query", context.Request.QueryString.Value);
            Assert.NotNull(context.Request.Body);
            Assert.NotNull(context.Request.Headers);
            Assert.NotNull(context.Response.Headers);
            Assert.NotNull(context.Response.Body);
            Assert.Equal(404, context.Response.StatusCode);
            Assert.Null(context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        }

        [Fact]
        public async Task GivenNonApiPath_ShouldNotForward()
        {
            HttpContext? context = await _server!.SendAsync(c =>
            {
                c.Request.Path = "/not-api/skip-me";
                c.Request.Headers.Add("x-territory-tools-user", "you@domain.com");
                c.User = FakeUser(isAuthenticated: true, "authenticated@user").Object;
            });

            Assert.False(_sendWasCalled);
        }

        [Fact]
        public async Task GivenUserHeaderInRequest_ShouldRemoveUserHeader()
        {
            HttpContext? context = await _server!.SendAsync(c =>
            {
                c.Request.Path = "/api/example";
                c.Request.Headers.Add("x-territory-tools-user", "hacker@user");
                c.User = FakeUser(isAuthenticated: true, "authenticated@user").Object;
            });

            Assert.True(context.Request.Headers.ContainsKey("x-territory-tools-user"));
            Assert.Equal("authenticated@user", _actualRequest!.Headers.GetValues("x-territory-tools-user").Single());
        }

        [Fact]
        public async Task GivenApiPath_ShouldForward_TrimmedPath()
        {
            HttpContext? context = await _server!.SendAsync(c =>
            {
                c.Request.Path = "/api/example";
                c.Request.QueryString = new QueryString("?thing=what");
                c.Request.Headers.Add("x-territory-tools-user", "hacker@user");
                c.User = FakeUser(isAuthenticated: true, "authenticated@user").Object;
            });

            Assert.Equal("/example", _actualRequest!.RequestUri.LocalPath);
            Assert.Equal("?thing=what", _actualRequest.RequestUri.Query);
        }

        [Theory]
        [InlineData("good-mtk", true, 3, true, 200)]
        [InlineData("expired-mtk", true, -3, false, 403)] // Forbidden
        [InlineData(null, false, -3, false, 401)] // Unauthenticated
        public async Task GivenMtkParameter_NotAuthenticated_ShouldCheckExpiration(
            string mtk,
            bool getLinkSuccess,
            int daysFromToday, 
            bool expectedToSend, 
            int expectedStatusCode)
        {
            _mockApiService
               .Setup(s => s.Get<TerritoryLinkContract>(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new TerritoryLinkContract() 
               {
                   Expires = DateTime.Now.AddDays(daysFromToday),
                   Successful = getLinkSuccess,
               });

            HttpContext? context = await _server!.SendAsync(c =>
            {
                c.Request.Path = "/api/example";
                c.Request.QueryString = new QueryString($"?mtk={mtk}");
                c.User = FakeUser(isAuthenticated: false, "no@user").Object;
            });

            Assert.Equal(expectedToSend, _sendWasCalled);
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        }

        [Theory]
        [InlineData(true, 200)]
        [InlineData(false, 401)]
        public async Task GivenIsAuthenticatedValue_ShouldCorrectStatusCode(bool isAuthenticated, int expectedStatusCode)
        {
            HttpContext? context = await _server!.SendAsync(c =>
            {
                c.Request.Path = "/api/example";
                c.Request.Headers.Add("x-territory-tools-user", "you@domain.com");
                c.User = FakeUser(isAuthenticated, "authenticated@user").Object;
            });

            Assert.Equal(expectedStatusCode, context.Response.StatusCode);

            Assert.Equal(isAuthenticated, _sendWasCalled);
        }

        private static Mock<ClaimsPrincipal> FakeUser(bool isAuthenticated, string name)
        {
            Mock<IIdentity> identity = new();
            identity.SetupGet(i => i.IsAuthenticated).Returns(isAuthenticated);
            identity.SetupGet(i => i.Name).Returns(name);

            Mock<ClaimsPrincipal> principle = new();
            principle.SetupGet(p => p.Identity).Returns(identity.Object);
            return principle;
        }
    }
}
