using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "AlbaTerritory")]
    [OutputType(typeof(Assignment))]
    public class GetAlbaTerritory : AlbaConnectedCmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                var assignmentsResultString = Connection.DownloadString(
                   RelativeUrlBuilder.GetTerritoryAssignments());

                string assignmentsHtml = TerritoryAssignmentParser
                    .Parse(assignmentsResultString);

                var assignments = new DownloadTerritoryAssignments(Connection)
                    .GetAssignments(assignmentsHtml);

                foreach (var assignment in assignments)
                {
                    WriteObject(assignment);
                }
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
