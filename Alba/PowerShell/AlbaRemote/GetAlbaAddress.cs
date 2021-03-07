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
    public class GetAlbaAddress : AlbaConnectedCmdlet
    {
        [Parameter]
        public int TerritoryId { get; set; }

        [Parameter]
        public int AddressId { get; set; }

        [Parameter]
        public string Search { get; set; } = "";

        protected override void ProcessRecord()
        {
            try
            {
                var records = new List<AlbaAddressExport>();
                if (AddressId != 0)
                {
                    var list = GetAddressList(Search, TerritoryId);
                    records = list.Where(a => a.Address_ID == AddressId)
                        .ToList();
                }
                else
                {
                    records = GetAddressList(Search, TerritoryId);
                }

                foreach (var record in records)
                {
                    WriteObject(record);
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }

        List<AlbaAddressExport> GetAddressList(string search, int territoryId)
        {
            var resultString = Connection.DownloadString(
                RelativeUrlBuilder.ExportAddresses(
                    accountId: Connection.AccountId,
                    territoryId: territoryId,
                    searchText: search));

            string text = AddressExportParser.Parse(resultString);

            using (var reader = new StringReader(text))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.BadDataFound = (rc) =>
                {
                    WriteVerbose($"CSV Address Parsing Error: {rc.RawRecord} ");
                };

                return csv.GetRecords<AlbaAddressExport>().ToList();
            }
        }
    }
}
