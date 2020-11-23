using Controllers.AlbaServer;
using CsvHelper;
using System;
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
            try
            {
                WriteObject(Convert(Input));
            }
            catch(Exception)// e)
            {
                throw; // new Exception($"Error converting PSObject to AlbaAddressImport: {e.StackTrace}", e);
            }
        }

        private AlbaAddressImport Convert(PSObject input)
        {
            int.TryParse(input.Properties["Address_ID"]?.Value?.ToString(), out int aid);
            int.TryParse(input.Properties["Territory_ID"]?.Value?.ToString(), out int tid);
            double.TryParse(input.Properties["Latitude"]?.Value?.ToString(), out double latitude);
            double.TryParse(input.Properties["Longitude"]?.Value?.ToString(), out double longitude);

            var address = new AlbaAddressImport
            {
                Address_ID = aid == 0 ? null : (int?)aid,
                Territory_ID = tid == 0 ? null : (int?)tid,
                Language = input.Properties["Language"]?.Value?.ToString(),
                Status = input.Properties["Status"]?.Value?.ToString(),
                Name = input.Properties["Name"]?.Value?.ToString(),
                Address = input.Properties["Address"]?.Value?.ToString(),
                Suite = input.Properties["Suite"]?.Value?.ToString(),
                City = input.Properties["City"]?.Value?.ToString(),
                Province = input.Properties["Province"]?.Value?.ToString(),
                Postal_code = input.Properties["Postal_code"]?.Value?.ToString(),
                Country = input.Properties["Country"]?.Value?.ToString(),
                Latitude = latitude == 0 ? null : (double?)latitude,
                Longitude = longitude == 0 ? null : (double?)longitude,
                Telephone = input.Properties["Telephone"]?.Value?.ToString(),
                Notes = input.Properties["Notes"]?.Value?.ToString(),
                Notes_private = input.Properties["Notes_private"]?.Value?.ToString(),
            };

            return address;
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
