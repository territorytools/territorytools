using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TerritoryTools.Web.Data;
using TerritoryTools.Entities;
using TerritoryTools.Web.MainSite.Areas.UrlShortener.Models;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

[assembly: HostingStartup(typeof(TerritoryTools.Web.MainSite.Areas.Identity.IdentityHostingStartup))]
namespace TerritoryTools.Web.MainSite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MainDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MainDbContextConnection")));

                services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<MainDbContext>();

                //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1
                //services.AddDataProtection()
                //    .PersistKeysToDbContext<MainDbContext>();
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"/data/keys"));

                //services.AddDefaultIdentity<IdentityUser>(
                //    options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<MainDbContext>();
            });
        }
    }
}