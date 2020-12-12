using Controllers.AlbaServer;
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

        [Parameter]
        public int TerritoryId { get; set; }

        [Parameter]
        public string Search { get; set; } = "";

        [Parameter(Mandatory = true)]
        public AuthorizationClient Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if(Connection.AccountId == 0)
                {
                    throw new ArgumentException("Account ID cannot be zero");
                }

                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.ExportAddresses(
                        accountId: Connection.AccountId,
                        territoryId: TerritoryId,
                        searchText: Search));

                string text = AddressExportParser.Parse(resultString);

                foreach (string line in text.Split('\n'))
                {
                    WriteObject(line);
                }
            }
            catch(Exception e)
            {
                errors.Add(e.Message);
            }
        }
    }
}
