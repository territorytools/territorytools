using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Import,"AddressFile")]
    public class ImportAddressFile : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string LanguageFilePath { get; set; }

        [Parameter(Mandatory = true)]
        public string AddressFilePath { get; set; }

        [Parameter(Mandatory = true)]
        public AuthorizationClient Connection { get; set; }

        [Parameter]
        public int UploadDelayMs { get; set; } = 300;

        protected override void BeginProcessing()
        {
            try
            {
                Import();
            }
            catch (UserException)
            {
                throw;
            }
        }

        public void Import()
        {
            if (string.IsNullOrWhiteSpace(LanguageFilePath)
                || string.IsNullOrWhiteSpace(AddressFilePath))
            {
                return;
            }

            new AddressImporter(Connection, UploadDelayMs, LanguageFilePath)
                .Upload(AddressFilePath);
        }
    }
}
