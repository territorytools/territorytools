using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaUser")]
    [OutputType(typeof(AlbaHtmlUser))]
    public class GetAlbaUser : PSCmdlet
    {
        List<string> errors = new List<string>();

        [Parameter(Mandatory = true)]
        public AlbaConnection Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
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
