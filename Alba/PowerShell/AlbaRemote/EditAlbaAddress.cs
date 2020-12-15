using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Edit,"AlbaAddress")]
    public class EditAlbaAddress : PSCmdlet
    {
        [Parameter]
        public string LanguageFilePath { get; set; }

        [Parameter]
        public AlbaConnection Connection { get; set; }

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
            if (Connection == null)
            {
                Connection = SessionState
                    .PSVariable
                    .Get(nameof(Names.CurrentAlbaConnection))?
                    .Value as AlbaConnection
                    ?? throw new MissingConnectionException();
            }

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
