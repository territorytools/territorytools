using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;
//using TerritoryTools.Entities;

namespace PowerShell
{
    [Cmdlet(VerbsCommon.Get,"Duplicates")]
    [OutputType(typeof(DuplicateAddress))]
    public class GetDuplicates : PSCmdlet
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
            var streetTypes = StreetType.Split(StreetType.Defaults);
            var prefixStreetTypes = StreetType.Split(StreetType.PrefixDefaults);
            parser = new Parser(Cities, streetTypes, prefixStreetTypes);

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
                //WriteVerbose(parsed.ToString());
                if (!string.IsNullOrWhiteSpace(parsed.FailedAddress))
                {
                    errors.Add($"{parsed.ErrorMessage}: {parsed.FailedAddress} ");
                }

                var duplicates = new List<ParsedAddress>();
                foreach(var master in parsedMasterList)
                {
                    if (master.AlbaAddressImport.Address_ID != Address.Address_ID
                        && master.Address.SameAs(parsed))
                    {
                        //WriteVerbose($"    {parsed} == {master.Address}");
                        duplicates.Add(master);
                    }
                }

                //WriteVerbose($"Duplicates: {duplicates.Count}");

                if (duplicates.Count > 0)
                {
                    //foreach (var dup in duplicates)
                    //{
                    //    if(IncludeSelf)
                    //    {
                    //        WriteObject(Address);
                    //    }

                    //    WriteObject(dup.AlbaAddressImport);
                    //}
                    var a = new DuplicateAddress(Address.Address_ID, Address);
                    a.DuplicationStatus = "Original";
                    WriteObject(a);

                    foreach (var dup in duplicates)
                    {
                        var da = new DuplicateAddress(Address.Address_ID, dup.AlbaAddressImport);
                        da.DuplicationStatus = "Duplicate";
                        WriteObject(da);
                    }
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

    public class ParsedAddress
    {
        public string Text { get; set; }
        public AlbaAddressImport AlbaAddressImport { get; set; }
        public Address Address { get; set; }
        public List<AlbaAddressImport> Duplicates { get; set; } = new List<AlbaAddressImport>();
    }

    public class DuplicateAddress : AlbaAddressImport
    {
        public DuplicateAddress(int? duplicateOfAddressId, AlbaAddressImport copy)
        {
            DuplicateOf = duplicateOfAddressId;
            Address_ID = copy.Address_ID;
            Territory_ID = copy.Territory_ID;
            Language = copy.Language;
            Status = copy.Status;
            Name = copy.Name;
            Suite = copy.Suite;
            Address = copy.Address;
            City = copy.City;
            Province = copy.Province;
            Postal_code = copy.Postal_code;
            Country = copy.Country;
            Latitude = copy.Latitude;
            Longitude = copy.Longitude;
            Telephone = copy.Telephone;
            Notes = copy.Notes;
            Notes_private = copy.Notes_private;
        }

        public int? DuplicateOf { get; set; }
        public string DuplicationStatus { get; set; }
    }
}
