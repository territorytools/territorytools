using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Import,"AlbaAddressFile")]
    public class ImportAlbaAddressFile : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory = true)]
        public string AddressFilePath { get; set; }

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
            new AddressImporter(
                    Connection, 
                    UploadDelayMs, 
                    languages: Languages)
                .AddFrom(AddressFilePath);
        }
    }
}
