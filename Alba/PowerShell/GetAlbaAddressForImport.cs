using Controllers.AlbaServer;
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaAddressForImport")]
    [OutputType(typeof(AlbaAddressImport))]
    public class GetAlbaAddressForImport : PSCmdlet
    {
        [Parameter(
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Text{ get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true)]
        [ValidateSet("TSV", "CSV")]
        public string Format { get; set; } = "TSV";

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public string Path { get; set; }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteObject(new AlbaAddressImport
            { 
                 Address = "Test Address"
            });
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
