using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Skip,"Duplicates")]
    [OutputType(typeof(Parsed))]
    public class SkipDuplicates : PSCmdlet
    {
       
        Parser parser;
        List<ParsedAddress> parsedMasterList; // = new List<Address>();
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

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
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
                //WriteVerbose($"MasterIn: {text}");
                var parsed = parser.Parse(text);
                //WriteVerbose($"MasterOut: {parsed}");
                parsedMasterList.Add(new ParsedAddress() { Address = parsed, AlbaAddressImport = master });
            }

            WriteVerbose($"parsedMasterList: {parsedMasterList.Count}");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            try
            {
                // TODO: Use the AlbaAddressInput to pass the data, only parse for comparison

                string text = $"{Address.Address}, {Address.Suite}, {Address.City}, {Address.Province} {Address.Postal_code}";
                var parsed = parser.Parse(text);
                WriteVerbose($"Parsing text: {text} -> {parsed}");
                if (!string.IsNullOrWhiteSpace(parsed.FailedAddress))
                {
                    errors.Add($"{parsed.ErrorMessage}: {parsed.FailedAddress} ");
                }

                bool duplicatesFound = false;
                var duplicates = new List<Parsed>();
                foreach(var master in parsedMasterList)
                {
                    //WriteVerbose($"Checking Master: {master.Address.ToString()}");
                    if (master.Address.SameAs(parsed))
                    {
                        WriteVerbose($"DUPLICATE: {Address.ToString()}");
                        duplicatesFound = true;
                        break;
                    }
                }

                if (!duplicatesFound)
                {
                    //WriteVerbose($"No duplicates found for: {Address.ToString()}");
                    //var p = new Parsed { Address = parsed, AlbaAddressImport = Address };
                    WriteObject(Address);
                }
            }
            catch(Exception)
            {
                //Skip
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("ERRORS:");
            foreach(string error in errors)
            {
                WriteVerbose(error);
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
