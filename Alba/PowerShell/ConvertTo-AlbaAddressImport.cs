using Controllers.AlbaServer;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PowerShell
{
    [Cmdlet("ConvertTo","AlbaAddressImport")]
    [OutputType(typeof(AlbaAddressImport))]
    public class ConvertToAlbaAddressImport : PSCmdlet
    {
        [Parameter(
          Mandatory = true,
          Position = 0,
          ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true)]
        public PSObject Input { get; set; }
        
        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            var address = new AlbaAddressImport
            {
                //Address_ID = (int?)Input.Properties["Address_Id"]?.Value
                Name = Input.Properties["Name"].Value?.ToString(),
                Address = Input.Properties["Address"].Value?.ToString(),
                Suite = Input.Properties["Suite"].Value?.ToString(),
                City = Input.Properties["City"].Value?.ToString(),
                Province = Input.Properties["Province"].Value?.ToString(),
                Postal_code = Input.Properties["Postal_code"].Value?.ToString(),
            };

            WriteObject(address);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
