using Controllers.UseCases;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Set,"AlbaTerritoryUser")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class SetAlbaTerritoryUser : PSCmdlet
    {
        [Parameter(Mandatory=true)]
        public int TerritoryId { get; set; }

        [Parameter(Mandatory = true)]
        public int UserId { get; set; }

        [Parameter]
        public AlbaConnection Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Connection == null)
                {
                    Connection = SessionState
                        .PSVariable
                        .Get(nameof(Names.CurrentAlbaConnection))?
                        .Value as AlbaConnection
                        ?? throw new MissingConnectionException();
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
