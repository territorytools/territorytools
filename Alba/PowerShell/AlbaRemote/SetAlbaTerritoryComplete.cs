using Controllers.UseCases;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "AlbaTerritoryComplete")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class SetAlbaTerritoryComplete : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory=true, Position=0, ValueFromPipeline=true)]
        public int TerritoryId { get; set; }

        [Parameter(Position = 1)]
        public DateTime? CompletedDate { get; set; }
        
        protected override void ProcessRecord()
        {
            try
            {
                if(CompletedDate == null)
                {
                    CompletedDate = DateTime.Now;
                }

                string url = RelativeUrlBuilder.SetTerritoryCompleted(
                    territoryId: TerritoryId,
                    completed: (DateTime)CompletedDate);

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
