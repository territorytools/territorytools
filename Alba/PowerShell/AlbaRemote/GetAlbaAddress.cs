using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaAddress")]
    public class GetAlbaAddress : PSCmdlet
    {
        List<string> errors = new List<string>();

        [Parameter(Mandatory = true)]
        public int AccountId { get; set; }

        [Parameter]
        public int TerritoryId { get; set; }

        [Parameter(Mandatory = true)]
        public AuthorizationClient Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.ExportAddresses(
                        accountId: AccountId,
                        territoryId: TerritoryId));

                string text = AddressExportParser.Parse(resultString);

                WriteObject(text);
            }
            catch(Exception e)
            {
                errors.Add(e.Message);
            }
        }
    }
}
