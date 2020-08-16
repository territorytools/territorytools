using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MainSite31.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Certes;
//using FluffySpoon.AspNet.LetsEncrypt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using System.Globalization;
using TerritoryTools.Web.Data.Services;
using WebUI.Areas.Identity.Data;
using WebUI.Services;

namespace WebUI
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
            services.AddDbContext<MainDbContext>(options =>
              options.UseSqlServer(
                  Configuration.GetConnectionString("MainDbContextConnection")));

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.AddAuthentication()
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            //services.AddMvc()
            //  .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //  .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            //  .AddDataAnnotationsLocalization();

            services.AddScoped<IAlbaCredentials>(ac => new AlbaCredentials(
                Configuration["AlbaAccount"],
                Configuration["AlbaUser"],
                Configuration["AlbaPassword"]));

            services.AddScoped<IShortUrlService, ShortUrlService>();
            services.AddScoped<IQRCodeActivityService, QRCodeActivityService>();

            services.Configure<WebUIOptions>(Configuration);

            var users = (Configuration["Users"] ?? string.Empty)
             .Split(';')
             .ToList();

            var adminUsers = (Configuration["AdminUsers"] ?? string.Empty)
                .Split(';')
                .ToList();

            services.AddSingleton<IAccountLists>(l => new AccountLists(
                Configuration["TT_AreaNames"] ?? string.Empty));

            services.AddScoped<IAuthorizationService>(s =>
                new TerritoryAuthorizationService(users, adminUsers));

            services.AddScoped<IAlbaCredentialService, AlbaCredentialAzureVaultService>();

            // New with .NET Core 3.1
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
