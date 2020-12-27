using System;
using System.Management.Automation;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet("Invoke", "AlbaWebRequest")]
    public class InvokeAlbaWebRequest : AlbaConnectedCmdlet
    {
        [Parameter(
           Mandatory = true,
           Position = 0,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true)]
        public string RelativeUri { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string result = Connection.DownloadString(RelativeUri);

                WriteObject(result);
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
