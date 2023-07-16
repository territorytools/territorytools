using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.Data.Services;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite
{
    public class Startup
    {
        static public bool NoSsl = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#tracktrace
            //https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger
            services.AddApplicationInsightsTelemetry();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
                
            //    // These three lines come from here: https://github.com/dotnet/aspnetcore/issues/14996
            //    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            //    options.OnAppendCookie = cookieContext =>
            //    {
            //        cookieContext.CookieOptions.Expires = DateTime.UtcNow.AddDays(90);
            //        cookieContext.CookieOptions.MaxAge = TimeSpan.FromDays(90);
            //        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //    };
            //    options.OnDeleteCookie = cookieContext =>
            //        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //});
            
            string connectionString = Configuration.GetConnectionString("MainDbContextConnection");
            
            Console.WriteLine($"Connection String: {connectionString}");
            
            services.AddDbContext<MainDbContext>(options =>
                options.UseSqlServer(connectionString));


            services.AddAuthentication(options =>
            {
                //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            })
            //.AddCookie(options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromDays(90);
            //    options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
            //    options.SlidingExpiration = true;
            //})
            //.AddJwtBearer("Asymmetric", options =>
            .AddJwtBearer(options =>
            {
                SecurityKey rsa = services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();

                options.IncludeErrorDetails = true; // <- great for debugging

                // Configure the actual Bearer validation
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = rsa,
                    ValidAudience = "jwt-test",
                    ValidIssuer = "jwt-test",
                    RequireSignedTokens = true,
                    RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
                    ValidateLifetime = true, // <- the "exp" will be validated
                    ValidateAudience = true,
                    ValidateIssuer = true,
                };
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services.Configure<MvcOptions>(options => options.EnableEndpointRouting = false);

            services.AddApplicationInsightsTelemetry();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddSingleton<ITelemetryService, TelemetryService>();

            services.AddScoped<IAlbaCredentials>(ac => new AlbaCredentials(
                Configuration["AlbaAccount"],
                Configuration["AlbaUser"],
                Configuration["AlbaPassword"]));

            services.AddScoped<IShortUrlService, ShortUrlService>();
            services.AddScoped<IQRCodeActivityService, QRCodeActivityService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ITerritoryAssignmentService, TerritoryAssignmentService>();
            services.AddScoped<AreaService>();
            services.AddScoped<IPhoneTerritoryAssignmentGateway, PhoneTerritoryAssignmentGateway>();
            services.AddScoped<IPhoneTerritoryCreationService, PhoneTerritoryCreationService>();
            services.AddScoped<IPhoneTerritoryAddWriterService, PhoneTerritoryAddWriterService>();
            services.AddScoped<ISpreadSheetService>(s => new GoogleSheets(
                System.IO.File.ReadAllText("./secrets/GoogleApi.secrets.json")));
            services.AddScoped<ISheetExtractor, SheetExtractor>();
            services.AddScoped<IAlbaAuthClientService, AlbaAuthClientService>();
            services.AddScoped<IAlbaAssignmentGateway, AlbaAssignmentGateway>();
            services.AddScoped<IAlbaAuthClientService, AlbaAuthClientService>();
            services.AddScoped<IAlbaManagementUserGateway, AlbaManagementUserGateway>();
            services.AddScoped<IAlbaUserGateway, AlbaUserGateway>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITerritoryUserService, TerritoryUserService>();
            services.AddScoped<IPhoneTerritoryAssignmentService, PhoneTerritoryAssignmentService>();
            services.AddScoped<ICombinedAssignmentService, AllCombinedAssignmentService>();
            services.AddScoped<IAssignLatestService, AssignLatestService>();
            services.AddScoped<ITerritoryApiService, TerritoryApiService>();
            services.AddScoped<IUserFromApiService, UserFromApiService>();
            services.AddScoped<KmlFileService>();
            services.AddScoped<AssignmentsCsvFileService>();

            // The ReverseProxyMiddleware can only get Singleton services, it does not work with Scoped services
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IDatabaseInfoService, DatabaseInfoService>();
            services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();

            services.Configure<WebUIOptions>(Configuration);

            string commitPath = "wwwroot/commit.txt";
            string commit = "dev";
            if(System.IO.File.Exists(commitPath)) 
            {
                string commitFileText = System.IO.File.ReadAllText(commitPath);
                if(!string.IsNullOrWhiteSpace(commitFileText) && commitFileText.Length >= 8)
                {
                   commit = commitFileText.TrimEnd()[..8];
                }
            }

             Configuration["GitCommit"] = commit;

            var users = (Configuration["Users"] ?? string.Empty)
               .Split(';')
               .ToList();

            Console.WriteLine($"Users Loaded from Configuration:");
            foreach (string user in users)
            {
                Console.WriteLine($"    {user}");
            }

            var adminUsers = (Configuration["AdminUsers"] ?? string.Empty)
                .Split(';')
                .ToList();

            Console.WriteLine($"Admin Users Loaded from Configuration:");
            foreach (string user in adminUsers)
            {
                Console.WriteLine($"    {user}");
            }

            services.AddSingleton<IAccountLists>(l => new AccountLists(
                Configuration["TT_AreaNames"] ?? string.Empty));

            services.AddScoped<IAuthorizationService>(s =>
                new TerritoryAuthorizationService(users, adminUsers));

            services.AddTransient<IAlbaCredentialService, AlbaCredentialAzureVaultService>();

            services.AddHostedService<TimedHostedService>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.MaxAge = TimeSpan.FromDays(180);
            });

            //services.AddAuthentication();
               //.AddJwtBearer("Asymmetric", options => {
               //    SecurityKey rsa = services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();

               //    options.IncludeErrorDetails = true; // <- great for debugging

               //     // Configure the actual Bearer validation
               //     options.TokenValidationParameters = new TokenValidationParameters
               //    {
               //        IssuerSigningKey = rsa,
               //        ValidAudience = "jwt-test",
               //        ValidIssuer = "jwt-test",
               //        RequireSignedTokens = true,
               //        RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
               //        ValidateLifetime = true, // <- the "exp" will be validated
               //        ValidateAudience = true,
               //        ValidateIssuer = true,
               //    };
               //});

            if (!NoSsl)
            {
                //ConfigureLetsEncryptServices(services);
            }
        }

        IPAddress ParseIPAddress(string ipAddressString)
        {
            try
            {
                return IPAddress.Parse(ipAddressString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing IP Address: '{ipAddressString ?? ""}'", ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                // Microsoft document about X-Forwarded headers
                // https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0

                if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues fwdIpAddress))
                {
                    ctx.Connection.RemoteIpAddress = ParseIPAddress(fwdIpAddress.First());
                }

                if (ctx.Request.Headers.TryGetValue("X-Forwarded-Proto", out StringValues fwdScheme))
                {
                    ctx.Request.Scheme = fwdScheme.First();
                }

                if (ctx.Request.Headers.TryGetValue("X-Forwarded-Host", out StringValues fwdHost))
                {
                    ctx.Request.Host = new HostString(fwdHost.First());
                }

                await next();
            });

            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change 
                // this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                //////app.UseHsts();
            }

            if (!NoSsl)
            {
               // app.UseFluffySpoonLetsEncryptChallengeApprovalMiddleware();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-AU"),
                new CultureInfo("zh"),
                new CultureInfo("zh-Hant"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".kml"] = "application/vnd.google-earth.kml+xml";
            provider.Mappings[".kmz"] = "application/vnd.google-earth.kmz";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            UpdateDatabase(app);

            //app.UseCookiePolicy(); // Before UseAuthentication or anything else that writes cookies.
            ////var cookieOptions = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>().Value;

            app.UseAuthentication();
            app.AddReverseProxy();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=ShortUrls}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<MainDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            options.Expires = DateTime.Now.AddDays(90);
         
            // Error Message: An error was encountered while handling the remote login. Correlation failed.
            // Error here: https://portal.azure.com/#blade/AppInsightsExtension/DetailsV2Blade/DataModel/%7B%22eventId%22:%227160fcae-281b-11ed-97dd-000d3a3f8942%22,%22timestamp%22:%222022-08-30T04:22:33.945Z%22%7D/ComponentId/%7B%22Name%22:%22territorytools%22,%22ResourceGroup%22:%22Experiments%22,%22SubscriptionId%22:%22410c5468-58aa-403b-b82b-a2ed191fdfb3%22%7D
            // This is a fix for old browsers:
            // From: https://github.com/dotnet/aspnetcore/issues/14996
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here. 
                    /* UserAgent doesn’t support new behavior */
                //I have no idea what user agent to use, or what a "user agent library" is
                if (userAgent == "Mozilla/5.0 (Linux; Android 13) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.97 Mobile Safari/537.36")
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

    }
}
