using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Skip,"DuplicateAddresses")]
    [OutputType(typeof(Parsed))]
    public class SkipDuplicateAddresses : PSCmdlet
    {
       
        Parser parser;
        List<ParsedAddress> parsedMasterList;
        List<string> errors = new List<string>();

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

        [Parameter]
        public SwitchParameter SkipNonDuplicatesInstead { get; set; }

        protected override void BeginProcessing()
        {
            var validRegions = Region.Split(Region.Defaults);
            var streetTypes = StreetType.Split(StreetType.Defaults);
            var prefixStreetTypes = StreetType.Split(StreetType.PrefixDefaults);
            var mapStreetTypes = StreetType.Map(StreetType.Defaults);
            parser = new Parser(validRegions, Cities, streetTypes, mapStreetTypes, prefixStreetTypes);
            parser.Normalize = true;

            WriteVerbose($"Parser Loaded Cities: {Cities.Count} Street Types:{streetTypes.Count}");

            parsedMasterList = new List<ParsedAddress>();
            errors = new List<string>();
            foreach (var master in MasterList)
            {
                string text = $"{master.Address}, {master.Suite}, {master.City}, {master.Province} {master.Postal_code}";
                var parsed = parser.Parse(text);
                parsedMasterList.Add(new ParsedAddress() { Address = parsed, AlbaAddressImport = master });
            }

            WriteVerbose($"parsedMasterList: {parsedMasterList.Count}");
        }

        protected override void ProcessRecord()
        {
            try
            {
                string text = $"{Address.Address}, {Address.Suite}, {Address.City}, {Address.Province} {Address.Postal_code}";
                var parsed = parser.Parse(text);
                if (!string.IsNullOrWhiteSpace(parsed.FailedAddress))
                {
                    errors.Add($"{parsed.ErrorMessage}: {parsed.FailedAddress} ");
                }

                bool duplicatesFound = false;
                var duplicates = new List<Parsed>();
                foreach(var master in parsedMasterList)
                {
                    if (master.Address.SameAs(parsed))
                    {
                        duplicatesFound = true;
                        break;
                    }
                }

                if ((!duplicatesFound && !SkipNonDuplicatesInstead.IsPresent)
                    || (duplicatesFound && SkipNonDuplicatesInstead.IsPresent))
                {
                    WriteObject(Address);
                }
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }

    public class Parsed
    {
        public string Text { get; set; }
        public AlbaAddressImport AlbaAddressImport { get; set; }
        public Address Address { get; set; }
        public List<AlbaAddressImport> Duplicates { get; set; } = new List<AlbaAddressImport>();
    }
}
