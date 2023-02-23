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
        Mock<IApiService> apiService = new Mock<IApiService>();
        Mock<RequestDelegate> _nextMiddleware = new Mock<RequestDelegate>();
        Mock<ILogger<ReverseProxyMiddleware>> logger = new Mock<ILogger<ReverseProxyMiddleware>>();
        ReverseProxyMiddleware proxy;
        Mock<IHttpClientWrapper> mockHttpClient = new();

        public ReverseProxyMiddlewareTests()
        {
            Dictionary<string, string> inMemoryConfigSettings = new()
            {
                { "TerritoryApiBaseUrl", "https://test" },
            };

            var cfgBuilder = new ConfigurationBuilder();
            cfgBuilder.AddInMemoryCollection(inMemoryConfigSettings);
            IConfiguration cfg = cfgBuilder.Build();

            mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new HttpResponseMessage());

            proxy = new(
                mockHttpClient.Object,
                apiService: apiService.Object,
                nextMiddleware: _nextMiddleware.Object,
                configuration: cfg,
                logger: logger.Object);
        }

        [Fact]
        public void SmokeTest()
        {
            Mock<HttpContext> context = new Mock<HttpContext>();
            
            proxy.Invoke(context.Object);
        } 
        
        [Fact]
        public void GivenPathNoMatch_ShouldMoveToNext()
        {
            Mock<HttpContext> context = new Mock<HttpContext>();
            PathString ps = new PathString("/not-api/path");
            context.SetupGet(c => c.Request.Path).Returns(ps);

            Mock<HttpResponse> mockResponse = new();
            mockResponse.SetupSet(r => r.StatusCode);
                
            context.SetupGet(c => c.Response).Returns(mockResponse.Object);

            bool nextWasCalled = false;
            _nextMiddleware.Setup(m => m.Invoke(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(c =>
                {
                    nextWasCalled = true;
                });

            proxy.Invoke(context.Object);

            Assert.True(nextWasCalled);
            mockResponse.Verify(r => r.StatusCode, Times.Never());
        }

        [Fact]
        public void GivenPathMatch_ShouldMoveToNext()
        {
            Mock<HttpContext> context = new Mock<HttpContext>();
            PathString ps = new PathString("/api/path");
            context.SetupGet(c => c.Request.Path).Returns(ps);

            Mock<HttpResponse> mockResponse = new();
            mockResponse.SetupSet(r => r.StatusCode);

            context.SetupGet(c => c.Response).Returns(mockResponse.Object);

            bool nextWasCalled = false;
            _nextMiddleware.Setup(m => m.Invoke(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(c =>
                {
                    nextWasCalled = true;
                });

            proxy.Invoke(context.Object);

            Assert.False(nextWasCalled);
            mockResponse.Verify(r => r.StatusCode, Times.Never());
        }

        [Fact]
        public async Task TestMiddleware_ExpectedResponse()
        {
            // This is just copied, with only a few lines added from here:
            // https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0
            Dictionary<string, string> inMemoryConfigSettings = new()
            {
                { "TerritoryApiBaseUrl", "https://test" },
            };

            using var host = await new HostBuilder()
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
                            services.AddScoped<IApiService, ApiService> ();
                            services.AddScoped<ITerritoryUserService, TerritoryUserService>();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<ReverseProxyMiddleware>(mockHttpClient.Object);
                        });
                })
                .StartAsync();

            var server = host.GetTestServer();
            server.BaseAddress = new Uri("https://example.com/A/Path/");

            var context = await server.SendAsync(c =>
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
        public async Task GivenNotAuthenticated_ShouldReturn401()
        {
            // This is just copied, with only a few lines added from here:
            // https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0
            Dictionary<string, string> inMemoryConfigSettings = new()
            {
                { "TerritoryApiBaseUrl", "https://test" },
            };

            Mock<IApiService> mockApiService = new();
            mockApiService
                .Setup(s => s.Get<TerritoryLinkContract>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new TerritoryLinkContract() { Expires = DateTime.Now.AddDays(3)});

            using var host = await new HostBuilder()
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
                            services.AddScoped<IApiService, ApiService>();
                            services.AddScoped<ITerritoryUserService, TerritoryUserService>();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<ReverseProxyMiddleware>(mockHttpClient.Object, mockApiService.Object);
                        });
                })
                .StartAsync();

            var server = host.GetTestServer();
            server.BaseAddress = new Uri("https://example.com/A/Path/");

            var context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Path = "/api/example";
                c.Request.QueryString = new QueryString("?and=query");
                c.Request.Headers.Add("x-territory-tools-user", "you@domain.com");
            });

            Assert.Equal(401, context.Response.StatusCode);
        }


        [Theory]
        [InlineData(true, 200)]
        public async Task GivenAuthenticated_ShouldReturnOk(bool isAuthenticated, int expectedStatusCode)
        {
            // This is just copied, with only a few lines added from here:
            // https://learn.microsoft.com/en-us/aspnet/core/test/middleware?view=aspnetcore-7.0
            Dictionary<string, string> inMemoryConfigSettings = new()
            {
                { "TerritoryApiBaseUrl", "https://test" },
            };

            Mock<IHttpClientWrapper> mockHttpClient = new();
            //mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
            mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny< HttpCompletionOption>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());

            Mock<IApiService> mockApiService = new();
            mockApiService
                .Setup(s => s.Get<TerritoryLinkContract>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new TerritoryLinkContract() { Expires = DateTime.Now.AddDays(3) });

            using var host = await new HostBuilder()
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
                            //services.AddScoped<IApiService, ApiService>();
                            services.AddScoped<ITerritoryUserService, TerritoryUserService>();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<ReverseProxyMiddleware>(mockApiService.Object, mockHttpClient.Object);
                        });
                })
                .StartAsync();

            var server = host.GetTestServer();
            server.BaseAddress = new Uri("https://example.com/A/Path/");

            Mock<IIdentity> identity = new();
            identity.SetupGet(i => i.IsAuthenticated).Returns(isAuthenticated);
            Mock<ClaimsPrincipal> principle = new();
            principle.SetupGet(p => p.Identity).Returns(identity.Object);
            var context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Path = "/api/example";
                c.Request.QueryString = new QueryString("?and=query");
                c.Request.Headers.Add("x-territory-tools-user", "you@domain.com");
                c.User = principle.Object;
            });

            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        }
    }

    public class FakeHttpContextFakeHttpContext : HttpContext
    {
        public override IFeatureCollection Features => throw new NotImplementedException();

        public override HttpRequest Request => throw new NotImplementedException();

        public HttpResponse ResponseReturns { get; set; } = new FakeHttpResponse();
        public override HttpResponse Response => ResponseReturns;

        public override ConnectionInfo Connection => throw new NotImplementedException();

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IDictionary<object, object?> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext => throw new NotImplementedException();

        public override int StatusCode { get; set; }

        public override IHeaderDictionary Headers => throw new NotImplementedException();

        public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IResponseCookies Cookies => throw new NotImplementedException();

        public override bool HasStarted => throw new NotImplementedException();

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }
}
