using Controllers.UseCases;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaUser")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class GetAlbaUser : PSCmdlet
    {
        [Parameter]
        public AlbaConnection Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Connection == null)
                {
                    Connection = SessionState.PSVariable
                        .Get("CurrentAlbaConnection")?.Value as AlbaConnection;
                }

                if (Connection == null)
                {
                    throw new ArgumentNullException(
                        paramName: nameof(Connection), 
                        message: "Use Get-AlbaConnection to set a default connection");
                }

                string url = RelativeUrlBuilder.GetUserManagementPage();
                var json = Connection.DownloadString(url);
                string html = AlbaJsonResultParser.ParseDataHtml(json, "users");
                var users = DownloadUserManagementData.GetUsers(html);

                foreach (var user in users)
                {
                    WriteObject(user);
                }
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
