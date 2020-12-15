using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "AlbaTerritoryBorder")]
    [OutputType(typeof(Territory))]
    public class GetAlbaTerritoryWithBorder : PSCmdlet
    {
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

                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.GetAllTerritoriesWithBorders());

                var territories = TerritoryResultParser.Parse(resultString);

                foreach (var territory in territories)
                {
                    WriteObject(territory);
                }
            }
            catch(Exception e)
            {
                WriteError(
                    new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
