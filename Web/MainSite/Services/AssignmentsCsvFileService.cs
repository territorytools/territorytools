using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public class AssignmentsCsvFileService
    {
        private readonly WebUIOptions _options;
        private readonly IAlbaCredentialService _albaCredentialService;
        private readonly ILogger<KmlFileService> _logger;

        public AssignmentsCsvFileService(
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<KmlFileService> logger)
        {
            _options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _logger = logger;
        }

        public void Download(string userName)
        {
            _logger.LogInformation("Downloading CSV files...");

            Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(userName);
            string path = string.Format(_options.AlbaAssignmentsHtmlPath, albaAccountId);

            var client = AlbaConnection.From(_options.AlbaHost);

            var downloader = new DownloadTerritoryAssignments(client);

            string html = System.IO.File.ReadAllText(path);

            string csvFilePath = "wwwroot/assignments.csv";
            if (System.IO.File.Exists(csvFilePath))
            {
                System.IO.File.Delete(csvFilePath);
            }

            if (System.IO.File.Exists(path))
            {
                downloader.SaveAs(html, csvFilePath);
            }
            else
            {
                downloader.SaveAs(csvFilePath);
            }
        }
    }
}
