using Controllers.AlbaServer;
using System;
using System.Management.Automation;
using System.Threading;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.AzureMaps;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Set,"Coordinates")]
    [OutputType(typeof(AlbaAddressImport))]
    public class SetCoordinates : PSCmdlet
    {
        [Parameter]
        public string Key { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        [Parameter]
        public AlbaConnection Connection { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        // Free account limited to 50 per second, or every 20ms
        [Parameter]
        public int DelayMs { get; set; } = 40;

        AzureMapsClient client;

        protected override void BeginProcessing()
        {
            client = AzureMapsmGeocodeAddress.AzureMapsClientFrom(Key);
        }

        protected override void ProcessRecord()
        {
            try
            {
                Thread.Sleep(DelayMs);

                var geocodedAddress = new AzureMapsmGeocodeAddress(client)
                    .Geocode(Address);

                WriteObject(geocodedAddress);
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
