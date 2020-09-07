using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TerritoryTools.Web.MainSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // New .NET Core 3.1 Version
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

            string noSsl = Environment.GetEnvironmentVariable("NoSsl");

            if (!args.Contains("no-ssl")
                && !args.Contains("NoSsl")
                && (noSsl == null
                || string.Equals(noSsl, "false", StringComparison.OrdinalIgnoreCase)))
            {
                //builder.UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(
                //   httpsOptions => httpsOptions.ServerCertificateSelector
                //       = (c, s) => LetsEncryptRenewalService.Certificate));
            }
            else
            {
                Startup.NoSsl = true;
            }

            return builder;

            // Use the ASPNETCORE_URLS environment variable to set URLs
            // Use a semi-colon ; to separate each URL
            // Example: ASPNETCORE_URLS=http://*:80;http://localhost:5000
            // Or specify the URLs on the command line with the --urls parameter
            // Example: dotnet run --urls "http://*:5000;http://*:6000"
            // Do not use https in either case
        }
    }
}
