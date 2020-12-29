using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Add, nameof(AlbaTerritoryBorder))]
    [OutputType(typeof(string))]
    public class AddAlbaTerritoryBorder : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory = true)]
        public AlbaTerritoryBorder Territory { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                CheckArguments();

                string uri = RelativeUrlBuilder.AddTerritoryWithBorder(Territory);
                
                WriteVerbose(uri);

                var resultString = Connection.DownloadString(uri);

                WriteObject(resultString);
            }
            catch (Exception e)
            {
                WriteError(
                    new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }

        void CheckArguments()
        {
            if (Territory.Border.Vertices.Count < 3)
            {
                throw new ArgumentOutOfRangeException(
                   nameof(Territory.Border),
                   "There are not enough territory border vertices, there must be at least three");
            }

            if (string.IsNullOrWhiteSpace(Territory.Number))
            {
                throw new ArgumentOutOfRangeException(
                   nameof(Territory.Number),
                   "Territory number");
            }
        }
    }
}
