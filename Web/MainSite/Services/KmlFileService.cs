using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public class KmlFileService
    {
        private readonly WebUIOptions _options;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly ILogger<KmlFileService> _logger;

        public KmlFileService(
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<KmlFileService> logger)
        {
            _options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _logger = logger;
        }

        public void Generate(string userName)
        {
            _logger.LogInformation("Downloading border KML files...");

            var credentials = _albaCredentialService.GetCredentialsFrom(userName);

            var client = AlbaConnection.From(_options.AlbaHost);
            client.Authenticate(credentials);

            string filePath = "wwwroot/borders.kml";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var territories = new DownloadKmlFile(client)
                .SaveAs(filePath);
        }
    }
}
