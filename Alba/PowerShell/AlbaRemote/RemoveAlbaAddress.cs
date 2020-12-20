using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "AlbaAddress")]
    public class RemoveAlbaAddress : AlbaConnectedCmdlet
    {
        [Parameter(
           Mandatory = false,
           Position = 0,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true)]
        public int AddressId { get; set; }

        // This Cmdlet is a very destructive, the Force parameter hopefully 
        // will make that clear.
        [Parameter]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (!Force.IsPresent)
                {
                    throw new Exception("Since this is a destructive Cmdlet you must supply the -Force parameter to use this Cmdlet.");
                }

                string url = RelativeUrlBuilder.DeleteAddress(AddressId);
                string result = Connection.DownloadString(url);

                WriteVerbose($"Delete address id {AddressId} unparsed result: {result}");
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
