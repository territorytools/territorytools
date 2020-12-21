using Controllers.UseCases;
using System;
using System.Collections.Generic;
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

        [Parameter]
        public int UserId { get; set; }
        
        [Parameter]
        public string User { get; set; }

        [Parameter]
        public List<AlbaUser> Users { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if(UserId == 0 && string.IsNullOrWhiteSpace(User))
                {
                    throw new ArgumentException("UserId or User required");
                }

                if (!string.IsNullOrWhiteSpace(User) 
                    && (Users == null || Users.Count == 0))
                {
                    throw new ArgumentException("UserId or User required");
                }

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
