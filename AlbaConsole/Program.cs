using AlbaClient;
using AlbaClient.AlbaServer;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using System;

namespace AlbaConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = string.Empty;
            string account = null;
            string user = null;
            string password = null;
            string k1MagicString = LogUserOntoAlba.k1MagicString;
            if (args.Length != 4)
            {
                Console.WriteLine("Alba Client Console");
                Console.WriteLine("Usage: alba <output-file> <account> <user> <password>");
                return;
            }

            filePath = args[0];
            account = args[1];
            user = args[2];
            password = args[3];

            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://", 
                site: "www.alba-website-here.com", 
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient, 
                basePath: basePath);

            var useCase = new DownloadTerritoryAssignments(client);

            var credentials = new Credentials(account, user, password, k1MagicString);

            client.Authorize(credentials);

            useCase.SaveAs(filePath);
        }
    }
}
