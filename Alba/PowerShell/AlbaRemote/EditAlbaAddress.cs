using Controllers.AlbaServer;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Edit,"AlbaAddress")]
    public class EditAlbaAddress : AlbaConnectedCmdlet
    {
        [Parameter]
        public string LanguageFilePath { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public int UploadDelayMs { get; set; } = 300;

        AddressImporter importer;

        protected override void BeginProcessing()
        {
            importer = new AddressImporter(
                Connection, 
                UploadDelayMs, 
                LanguageFilePath);
        }

        protected override void ProcessRecord()
        {
            try
            {
                string result = importer.Update(Address);
                
                WriteVerbose($"Result: {result}");
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
