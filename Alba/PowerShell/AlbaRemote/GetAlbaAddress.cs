using Controllers.AlbaServer;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaAddress")]
    [OutputType(typeof(AlbaAddressExport))]
    public class GetAlbaAddress : PSCmdlet
    {
        List<string> errors = new List<string>();

        [Parameter]
        public int TerritoryId { get; set; }

        [Parameter]
        public string Search { get; set; } = "";

        [Parameter]
        public AlbaConnection Connection { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Connection == null)
                {
                    Connection = SessionState
                        .PSVariable
                        .Get(nameof(Names.CurrentAlbaConnection))?
                        .Value as AlbaConnection
                        ?? throw new MissingConnectionException();
                }

                var resultString = Connection.DownloadString(
                    RelativeUrlBuilder.ExportAddresses(
                        accountId: Connection.AccountId,
                        territoryId: TerritoryId,
                        searchText: Search));

                string text = AddressExportParser.Parse(resultString);

                using (var reader = new StringReader(text))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = "\t";
                    csv.Configuration.BadDataFound = (rc) => {
                        WriteVerbose($"CSV Address Parsing Error: {rc.RawRecord} ");
                    };

                    var records = csv.GetRecords<AlbaAddressExport>().ToList();
                    foreach (var record in records)
                    {
                        WriteObject(record);
                    }
                }

            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
