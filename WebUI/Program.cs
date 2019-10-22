﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using FluffySpoon.AspNet.LetsEncrypt;
using System;
using System.Linq;

namespace WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);

            string noSsl = Environment.GetEnvironmentVariable("NoSsl");

            if (!args.Contains("no-ssl") 
                && !args.Contains("NoSsl") 
                && (noSsl == null 
                || string.Equals(noSsl,"false",StringComparison.OrdinalIgnoreCase)))
            {
                builder.UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(
                   httpsOptions => httpsOptions.ServerCertificateSelector
                       = (c, s) => LetsEncryptRenewalService.Certificate));
            }
            else
            {
                Startup.NoSsl = true;
            }

            builder.UseStartup<Startup>();

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
