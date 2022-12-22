using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.Data.Services;
using TerritoryTools.Web.MainSite.Services;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Mvc.Razor;

namespace TerritoryTools.Web.MainSite;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables(prefix: "TT_");
        builder.Configuration.AddUserSecrets<Program>(optional: false);

        IServiceCollection services = builder.Services;

        //https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#tracktrace
        //https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger
        //not in net6?//services.AddApplicationInsightsTelemetry();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplicationInsightsTelemetry();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
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
        services.Configure<CookiePolicyOptions>(options =>
        {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;

                // These three lines come from here: https://github.com/dotnet/aspnetcore/issues/14996
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.OnAppendCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        string connectionString = builder.Configuration.GetSection("ConnectionStrings")["MainDbContextConnection"];
        services.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

        //services.AddAuthentication()
        //    .AddMicrosoftAccount(options =>
        //    {
        //        options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
        //        options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
        //    });

       /// services.Configure<MvcOptions>(options => options.EnableEndpointRouting = false);

        services.AddApplicationInsightsTelemetry();

        //services.AddMvc()
        //    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        //    .AddDataAnnotationsLocalization();

        services
            .AddLocalization(options => options.ResourcesPath = "Resources");

        services
          .AddMvc()
          .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

        services.AddSingleton<ITelemetryService, TelemetryService>();

        services.AddScoped<IAlbaCredentials>(ac => new AlbaCredentials(
            builder.Configuration["AlbaAccount"],
            builder.Configuration["AlbaUser"],
            builder.Configuration["AlbaPassword"]));

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
        services.AddScoped<IApiService, ApiService>();
        services.AddScoped<KmlFileService>();
        services.AddScoped<AssignmentsCsvFileService>();

        services.Configure<WebUIOptions>(builder.Configuration);

        string commitPath = "wwwroot/commit.txt";
        builder.Configuration["GitCommit"] = System.IO.File.Exists(commitPath)
            ? System.IO.File.ReadAllText(commitPath).TrimEnd()[..8]
            : "dev";

        var users = (builder.Configuration["Users"] ?? string.Empty)
           .Split(';')
           .ToList();

        Console.WriteLine($"Users Loaded from Configuration:");
        foreach (string user in users)
        {
            Console.WriteLine($"    {user}");
        }

        var adminUsers = (builder.Configuration["AdminUsers"] ?? string.Empty)
            .Split(';')
            .ToList();

        Console.WriteLine($"Admin Users Loaded from Configuration:");
        foreach (string user in adminUsers)
        {
            Console.WriteLine($"    {user}");
        }

        services.AddSingleton<IAccountLists>(l => new AccountLists(
            builder.Configuration["TT_AreaNames"] ?? string.Empty));

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


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();

        //CreateWebHostBuilder(args).Build().Run();
    }

    // Use the ASPNETCORE_URLS environment variable to set URLs
    // Use a semi-colon ; to separate each URL
    // Example: ASPNETCORE_URLS=http://*:80;http://localhost:5000
    // Or specify the URLs on the command line with the --urls parameter
    // Example: dotnet run --urls "http://*:5000;http://*:6000"
    // Do not use https in either case


    static void UpdateDatabase(IApplicationBuilder app)
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

    static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
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
