using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"OriginalAddress")]
    [OutputType(typeof(DuplicatedAddress))]
    public class GetOriginalAddress : PSCmdlet
    {
       
        Parser parser;
        List<ParsedAddress> parsedMasterList;

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public List<AlbaAddressImport> MasterList { get; set; }

        [Parameter]
        public List<string> Cities { get; set; }

        [Parameter]
        public SwitchParameter IncludeSelf { get; set; }

        protected override void BeginProcessing()
        {
            var validRegions = Region.Split(Region.Defaults);
            var streetTypes = StreetType.Split(StreetType.Defaults);
            var prefixStreetTypes = StreetType.Split(StreetType.PrefixDefaults);
            var mapStreetTypes = StreetType.Map(StreetType.Defaults);
            parser = new Parser(validRegions, Cities, streetTypes, mapStreetTypes, prefixStreetTypes);
            parser.Normalize = true;

            WriteVerbose($"Loaded Cities: {Cities.Count}");
            WriteVerbose($"Loaded Street Types:{streetTypes.Count}");

            parsedMasterList = new List<ParsedAddress>();
            foreach (var master in MasterList)
            {
                string text = $"{master.Address}, {master.Suite}, {master.City}, {master.Province} {master.Postal_code}";
                var parsed = parser.Parse(text);
                parsedMasterList.Add(new ParsedAddress() { Address = parsed, AlbaAddressImport = master });
            }

            WriteVerbose($"Loaded Master List Addresses: {parsedMasterList.Count}");

            WriteVerbose("Sorting Master List of Addresses by Address_ID so the lowest, or oldest, record is treated as the original.");
            parsedMasterList = parsedMasterList.OrderBy(a => a.AlbaAddressImport.Address_ID).ToList();
        }

        protected override void ProcessRecord()
        {
            try
            {
                ProcessAddress();
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }

        private void ProcessAddress()
        {
            string text = $"{Address.Address}, {Address.Suite}, {Address.City}, {Address.Province} {Address.Postal_code}";
            var parsed = parser.Parse(text);
            if (!string.IsNullOrWhiteSpace(parsed.FailedAddress))
            {
                throw new Exception("Parsing error: " + parsed.FailedAddress);
            }

            foreach (var master in parsedMasterList)
            {
                if (master.Address.SameAs(parsed) 
                    && master.AlbaAddressImport.Address_ID != Address.Address_ID
                    && master.AlbaAddressImport.Address_ID != 0)
                {
                    var duplicate = new DuplicatedAddress
                    {
                        Original = master.AlbaAddressImport,
                        Duplicate = Address
                    };

                    WriteObject(duplicate);
                    break;
                }
            }
        }
    }

    public class DuplicatedAddress
    {
        public AlbaAddressImport Duplicate { get; set; }
        public AlbaAddressImport Original { get; set; }
    }
}
