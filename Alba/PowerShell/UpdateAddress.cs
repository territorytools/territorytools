using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace PowerShell
{
    [Cmdlet(VerbsData.Update,"Address")]
    public class UpdateAddress : PSCmdlet
    {
        List<string> errors = new List<string>();

        [Parameter]
        public string LanguageFilePath { get; set; }

        [Parameter]
        public AuthorizationClient Connection { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public int UploadDelayMs { get; set; } = 300;

        AddressImporter importer;

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {

            importer = new AddressImporter(
                Connection, 
                UploadDelayMs, 
                LanguageFilePath);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            try
            {
                string result = importer.Update(Address);
                
                WriteVerbose($"Result: {result}");
            }
            catch(Exception)
            {
                throw;
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
        }
    }
}
