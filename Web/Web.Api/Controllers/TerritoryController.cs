using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    public class TerritoryController : Controller
    {
        private readonly ILogger<TerritoryController> _logger;

        public TerritoryController(ILogger<TerritoryController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogTrace("Index Called");
            Console.WriteLine("Downloading assignments...");

            var client = Program.AlbaClient();

            client.Authenticate(Program.GetCredentials());

            var useCase = new DownloadTerritoryAssignments(client);

            useCase.SaveAs(OutputFilePath);

            
            return View();
        }
    }
}
