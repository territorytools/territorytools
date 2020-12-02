using Controllers.AlbaServer;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;

namespace PowerShell
{
    [Cmdlet(VerbsData.Out,"Address")]
    [OutputType(typeof(AlbaAddressImport))]
    public class OutAddress : PSCmdlet
    {
        string content = string.Empty;

        [Parameter(
            ValueFromPipelineByPropertyName = true)]
        [ValidateSet("TSV", "CSV")]
        public string Format { get; set; } = "TSV";

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public string Path { get; set; }

        private List<AlbaAddressImport> addresses = new List<AlbaAddressImport>();
        
        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin Saving Addresses...");
            addresses = new List<AlbaAddressImport>();
            //var tw = new TextWriter()
            //var writer = new CsvWriter()
            //using (var reader = new StreamReader(Path))
            //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //{
            //    csv.Configuration.Delimiter = Format == "TSV" ? "\t" : ",";
            //    csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            //    csv.Configuration.BadDataFound = null;
            //    var addresses = csv.GetRecords<AlbaAddressImport>();
            //    WriteVerbose("Start looping addresses...");
            //    foreach (var address in addresses)
            //    {
            //        Addresses.Add(address);
            //    }

            //    WriteVerbose($"{addresses.Count()} addresses loaded.");
            //}
        }

        protected override void ProcessRecord()
        {
            //WriteObject(Addresses);
            addresses.Add(Address);
        }

        protected override void EndProcessing()
        {
            using (var writer = new StreamWriter(Path))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.WriteRecords(addresses);
            }

            WriteVerbose("Done");
        }
    }
}
