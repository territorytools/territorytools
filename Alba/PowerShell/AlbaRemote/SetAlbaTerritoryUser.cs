using Controllers.UseCases;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Set,"AlbaTerritoryUser")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class SetAlbaTerritoryUser : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory=true)]
        public int TerritoryId { get; set; }

        [Parameter(Mandatory = true)]
        public int UserId { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string url = RelativeUrlBuilder.AssignTerritory(
                    territoryId: TerritoryId,
                    userId: UserId,
                    DateTime.Now);

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
