using Controllers.AlbaServer;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Kml;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.ConvertTo,"Kml")]
    [OutputType(typeof(GoogleMapsKml))]
    public class ConvertToKml : PSCmdlet
    {
        [Parameter(
          Mandatory = true,
          Position = 0,
          ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true)]
        public AlbaTerritoryBorder Input { get; set; }
        
        protected override void ProcessRecord()
        {
            try
            {
                var kml = TerritoryDetailToKmlConverter
                    .Convert(TerritoryResultParser.TerritoryFrom(Input));

                WriteObject(kml);
            }
            catch(Exception e)
            {
                throw new Exception($"Error converting PSObject to AlbaAddressImport: {e.StackTrace}", e);
            }
        }
    }
}
