using System;
using System.IO;
using System.Management.Automation;
using System.Xml.Serialization;
using TerritoryTools.Alba.Controllers.Kml;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.ConvertTo, nameof(AlbaTerritoryBorder))]
    [OutputType(typeof(AlbaTerritoryBorder))]
    public class ConvertToAlbaTerritoryBorder : PSCmdlet
    {
        KmlToAlbaTerritoryBorderConverter converter = new KmlToAlbaTerritoryBorderConverter();

        [Parameter(
          Mandatory = true,
          Position = 0,
          ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true)]
        public string Input { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GoogleMapsKml));

                using (var stream = new StringReader(Input))
                {
                    var kml = serializer.Deserialize(stream) as GoogleMapsKml;
                    var territories = converter.TerritoryListFrom(kml);

                    foreach (var territory in territories)
                    {
                        WriteObject(territory);
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception($"Error converting KML into AlbaTerritoryBorder, if you are using Get-Content, be sure to add the -Raw parameter: {e.Message} at {e.StackTrace}", e);
            }

            base.EndProcessing();
        }
    }
}
