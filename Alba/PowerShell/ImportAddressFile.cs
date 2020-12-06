using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace PowerShell
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

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            Import();
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
        }

        public void Import()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LanguageFilePath)
                    || string.IsNullOrWhiteSpace(AddressFilePath))
                {
                    return;
                }

                new ImportAddress(Connection, UploadDelayMs)
                    .Upload(AddressFilePath, LanguageFilePath);
            }
            catch (UserException)
            {
                throw;
            }
        }
    }
}
