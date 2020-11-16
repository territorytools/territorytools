using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Normalized")]
    [OutputType(typeof(Normalized))]
    public class GetDuplicates : PSCmdlet
    {
        Parser parser;
        List<Normalized> normalized;

        [Parameter(
           Mandatory = true,
           Position = 0,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true)]
        public string Address { get; set; }
        //public AlbaAddressImport Address { get; set; }

        [Parameter]
        public List<string> Cities { get; set; }

        protected override void BeginProcessing()
        {
            var streetTypes = StreetType.Split(StreetType.Defaults);
            var prefixStreetTypes = StreetType.Split(StreetType.PrefixDefaults);
            parser = new Parser(Cities, streetTypes, prefixStreetTypes);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            Address parsed;
            var normalized = new Normalized();

            try
            {
                parsed = parser.Parse(Address);
                normalized = new Normalized
                {
                    Original = Address,
                    StreetNamePrefix = parsed.Street.Name.NamePrefix,
                    StreetNumber = parsed.Street.Number,
                    StreetNumberFraction = parsed.Street.NumberFraction,
                    DirectionalPrefix = parsed.Street.Name.DirectionalPrefix,
                    StreetTypePrefix = parsed.Street.Name.StreetTypePrefix,
                    StreetName = parsed.Street.Name.Name,
                    StreetType = parsed.Street.Name.StreetType,
                    DirectionalSuffix = parsed.Street.Name.DirectionalSuffix,
                    UnitType = parsed.Unit.Type,
                    UnitNumber = parsed.Unit.Number,
                    City = parsed.City.Name,
                    Region = parsed.Region.Code,
                    PostalCode = parsed.Postal.Code,
                    PostalCodeExtra = parsed.Postal.Extra
                };
            }
            catch(Exception)
            {
                normalized.Original = $"*{Address}";
            }
               

            WriteObject(normalized);
        }
    }

    public class Normalized
    {
        public string Original { get; set; }
        public string StreetNamePrefix { get; set; }
        public string StreetNumber { get; set; }
        public string StreetNumberFraction { get; set; }
        public string DirectionalPrefix { get; set; }
        public string StreetTypePrefix { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string DirectionalSuffix { get; set; }
        public string UnitType { get; set; }
        public string UnitNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeExtra { get; set; }
    }
}
