using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Edit, nameof(AlbaTerritoryBorder))]
    [OutputType(typeof(AlbaTerritoryBorder))]
    public class EditAlbaTerritoryBorder : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory = true)]
        public AlbaTerritoryBorder Territory { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                CheckArguments();

                string uri = RelativeUrlBuilder.SaveTerritoryWithBorder(Territory);
                
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
            if (Territory.Id == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Territory.Id),
                    "Territory ID cannot be zero");
            }

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
