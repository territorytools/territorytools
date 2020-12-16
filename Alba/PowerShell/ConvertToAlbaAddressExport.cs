using Controllers.AlbaServer;
using System;
using System.Management.Automation;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.ConvertTo,nameof(AlbaAddressExport))]
    [OutputType(typeof(AlbaAddressExport))]
    public class ConvertToAlbaAddressExport : PSCmdlet
    {
        [Parameter(
          Mandatory = true,
          Position = 0,
          ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true)]
        public PSObject Input { get; set; }
        
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(Convert(Input));
            }
            catch(Exception e)
            {
                throw new Exception($"Error converting PSObject to AlbaAddressImport: {e.StackTrace}", e);
            }
        }

        private AlbaAddressExport Convert(PSObject input)
        {
            int.TryParse(input.Properties["Address_ID"]?.Value?.ToString(), out int aid);
            int.TryParse(input.Properties["Territory_ID"]?.Value?.ToString(), out int tid);
            double.TryParse(input.Properties["Latitude"]?.Value?.ToString(), out double latitude);
            double.TryParse(input.Properties["Longitude"]?.Value?.ToString(), out double longitude);
            //DateTime.TryParse(input.Properties["Created"]?.Value?.ToString(), out DateTime created);
            //DateTime.TryParse(input.Properties["Modified"]?.Value?.ToString(), out DateTime modified);
            //DateTime.TryParse(input.Properties["Contacted"]?.Value?.ToString(), out DateTime contacted);

            var address = new AlbaAddressExport
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
                Account = input.Properties["Account"]?.Value?.ToString(),
                Geocoded = input.Properties["Geocoded"]?.Value?.ToString(),
                Owner = input.Properties["Owner"]?.Value?.ToString(),
                Territory_number = input.Properties["Territory_number"]?.Value?.ToString(),
                Territory_description = input.Properties["Territory_description"]?.Value?.ToString(),
                Created = input.Properties["Created"]?.Value?.ToString(),
                Modified = input.Properties["Modified"]?.Value?.ToString(),
                Contacted = input.Properties["Contacted"]?.Value?.ToString()
            };

            return address;
        }
    }
}
