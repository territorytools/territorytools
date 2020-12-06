using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "Address")]
    public class RemoveAddress : PSCmdlet
    {
        [Parameter]
        public AuthorizationClient Connection { get; set; }

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

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
          
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if(!Force.IsPresent)
            {
                throw new System.Exception("Since this is a destructive Cmdlet you must supply the -Force parameter to use this Cmdlet.");
            }

            string url = RelativeUrlBuilder.DeleteAddress(AddressId);
            string result = Connection.DownloadString(url);
            WriteVerbose($"Delete address id {AddressId} unparsed result: {result}");
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {

        }
    }
}
