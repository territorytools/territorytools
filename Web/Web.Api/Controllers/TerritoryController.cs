using Microsoft.AspNetCore.Mvc;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;
using TerritoryTools.Alba.Controllers.UseCases;

namespace Web.Api.Controllers
{
    public class TerritoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TerritoryController> _logger;

        public TerritoryController(IConfiguration configuration, ILogger<TerritoryController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogTrace("Index Called");
            Console.WriteLine("Downloading assignments...");

            var client = AlbaClient();

            client.Authenticate(GetCredentials());

            var useCase = new DownloadTerritoryAssignments(client);

            //useCase.SaveAs(OutputFilePath);

            
            return View();
        }

        public AlbaConnection AlbaClient()
        {
            string albaHost = _configuration.GetValue<string>("alba_host");
            if (string.IsNullOrWhiteSpace(albaHost))
            {
                throw new Exception("ALBA_HOST environment variable is missing!");
            }

            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: albaHost,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }


        public Credentials GetCredentials()
        {
            string host =     _configuration.GetValue<string>("alba_host");
            string account =  _configuration.GetValue<string>("alba_account");
            string user =     _configuration.GetValue<string>("alba_user");
            string password = _configuration.GetValue<string>("alba_password");

            if (string.IsNullOrWhiteSpace(account)
                || string.IsNullOrWhiteSpace(user)
                || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Missing credentials, please set your credentials as environment variables:  alba_host, alba_account, alba_user, alba_password");
            }

            return new Credentials(
                account,
                user,
                password);
        }
    }
}
