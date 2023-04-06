using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TerritoryTools.Web.Data;

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
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(366))
                    .SetApplicationName("Territory Tools Web")
                    .PersistKeysToFileSystem(new DirectoryInfo(@"/data/keys"));

                //services.AddDefaultIdentity<IdentityUser>(
                //    options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<MainDbContext>();
            });
        }
    }
}