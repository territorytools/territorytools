using Controllers.UseCases;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "AlbaTerritory")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class GetAlbaTerritory : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public AlbaConnection Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.GetAllTerritories());

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
