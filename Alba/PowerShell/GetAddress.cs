using Controllers.AlbaServer;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Get,"Address")]
    [OutputType(typeof(AlbaAddressImport))]
    public class GetAddress : PSCmdlet
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

        private List<AlbaAddressImport> Addresses = new List<AlbaAddressImport>();
        
        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin Loading Addresses...");

            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = Format == "TSV" ? "\t" : ",";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                csv.Configuration.BadDataFound = null;
                var addresses = csv.GetRecords<AlbaAddressImport>();
                WriteVerbose("Start looping addresses...");
                foreach (var address in addresses)
                {
                    Addresses.Add(address);
                }

                WriteVerbose($"{addresses.Count()} addresses loaded.");
            }
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteObject(Addresses);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            
            WriteVerbose("End!");
        }
    }
}
