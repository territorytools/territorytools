using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, nameof(AlbaTerritoryBorder))]
    [OutputType(typeof(AlbaTerritoryBorder))]
    public class GetAlbaTerritoryWithBorder : AlbaConnectedCmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.GetAllTerritoriesWithBorders());

                var territories = TerritoryBorderResultParser.Parse(resultString);

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
