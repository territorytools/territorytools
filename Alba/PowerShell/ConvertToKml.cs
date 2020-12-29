using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Xml.Serialization;
using TerritoryTools.Alba.Controllers.Kml;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.ConvertTo,"Kml")]
    [OutputType(typeof(GoogleMapsKml))]
    public class ConvertToKml : PSCmdlet
    {
        AlbaTerritoryBorderToKmlConverter converter;
        readonly List<AlbaTerritoryBorder> territories = new List<AlbaTerritoryBorder>();

        [Parameter(
          Mandatory = true,
          Position = 0,
          ValueFromPipeline = true,
          ValueFromPipelineByPropertyName = true)]
        public AlbaTerritoryBorder Input { get; set; }

        protected override void BeginProcessing()
        {
            converter = new AlbaTerritoryBorderToKmlConverter();

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            try
            {
                territories.Add(Input);
            }
            catch(Exception e)
            {
                throw new Exception($"Error converting AlbaTerritoryBorder into KML: {e.StackTrace}", e);
            }
        }

        protected override void EndProcessing()
        {
            var kml = converter.KmlFrom(territories);

            var serializer = new XmlSerializer(typeof(GoogleMapsKml));

            using (var textWriter = new StringWriterUtf8())
            {
                serializer.Serialize(textWriter, kml);
                WriteObject(textWriter.ToString());
            }

            base.EndProcessing();
        }
    }

    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
