using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
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
using System;
using System.Globalization;
using System.Linq;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.Data.Services;
using TerritoryTools.Web.MainSite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

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
                    ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            string connectionString = Configuration.GetConnectionString("MainDbContextConnection");
            services.AddDbContext<MainDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
            
            //services.AddAuthentication()
            //    .AddMicrosoftAccount(options =>
            //    {
            //        options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            //        options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            //    });

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
                System.IO.File.ReadAllText("./GoogleApi.secrets.json")));
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
            services.AddScoped<KmlFileService>();
            services.AddScoped<AssignmentsCsvFileService>();

            services.Configure<WebUIOptions>(Configuration);

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

            services.AddAuthentication();
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

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                ctx.Request.Scheme = "https";
                ctx.Request.Host = new HostString(Configuration.GetValue<string>("HOST_NAME"));

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
                app.UseHsts();
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

            app.UseAuthentication();

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

        void ConfigureLetsEncryptServices(IServiceCollection services)
        {
            // Configuration code copied from: 
            // https://github.com/ffMathy/FluffySpoon.AspNet.LetsEncrypt/blob/master/README.md
            // The following line adds the automatic renewal service.
            services.AddFluffySpoonLetsEncryptRenewalService(
                new LetsEncryptOptions()
                {
                    // LetsEncrypt will send you an e-mail here when the 
                    // certificate is about to expire
                    Email = Configuration["LetsEncrypt:Email"],
                    UseStaging = bool.Parse(Configuration["LetsEncrypt:UseStaging"]), //switch to true for testing
                    Domains = Configuration["LetsEncrypt:Domains"]
                        .ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries),
                    // Renew automatically 30 days before expiry
                    TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
                    //these are your certificate details
                    CertificateSigningRequest = new CsrInfo()
                    {
                        CountryName = Configuration["LetsEncrypt:CountryName"],
                        Locality = Configuration["LetsEncrypt:Locality"],
                        Organization = Configuration["LetsEncrypt:Organization"],
                        OrganizationUnit = Configuration["LetsEncrypt:OrganizationUnit"],
                        State = Configuration["LetsEncrypt:State"]
                    }
                });

            // The following line tells the library to persist the certificate
            // to a file, so that if the server restarts, the certificate can 
            // be re-used without generating a new one.
            //services.AddFluffySpoonLetsEncryptFileCertificatePersistence("/data/certificate");

            // The following line tells the library to persist challenges 
            // in-memory. challenges are the "/.well-known" URL codes that 
            // LetsEncrypt will call.
            //services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
        }
    }
}
