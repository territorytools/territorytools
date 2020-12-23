using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Clear,"AlbaTerritoryUser")]
    public class ClearAlbaTerritoryUser : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory=true)]
        public int TerritoryId { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string url = RelativeUrlBuilder.UnassignTerritory(TerritoryId);
                var json = Connection.DownloadString(url);

                // TODO: Check the json to see if it worked
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
