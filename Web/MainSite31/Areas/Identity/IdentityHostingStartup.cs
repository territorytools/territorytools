using System;
using MainSite31.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebUI.Areas.Identity.Data;

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

                services.AddDefaultIdentity<IdentityUser>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<MainDbContext>();
            });
        }
    }
}