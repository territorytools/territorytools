using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet("Normalize", "AlbaAddressImport")]
    [OutputType(typeof(AlbaAddressImport))]
    public class NormalizeAlbaAddressImport : PSCmdlet
    {
        Parser parser;

        [Parameter(
           Mandatory = true,
           Position = 0,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public List<string> Cities { get; set; }

        protected override void BeginProcessing()
        {
            var validRegions = Region.Split(Region.Defaults);
            var streetTypes = StreetType.Split(StreetType.Defaults);
            var mapStreetTypes = StreetType.Map(StreetType.Defaults);
            var prefixStreetTypes = StreetType.Split(StreetType.PrefixDefaults);
            parser = new Parser(validRegions, Cities, streetTypes, mapStreetTypes, prefixStreetTypes);
        }

        protected override void ProcessRecord()
        {
            var normalized = new AlbaAddressImport();

            try
            {
                parser.Normalize = true;
                WriteVerbose(Address.ToAddressString());
                Address parsed = parser.Parse(Address.ToAddressString());
                
                normalized = new AlbaAddressImport
                {
                    Address_ID  = Address.Address_ID,
                    Territory_ID = Address.Territory_ID,
                    Status = Address.Status, 
                    Name = Address.Name,
                    Address = parsed.Street.ToString(),
                    Suite = parsed.Unit.ToString(),
                    City = parsed.City.Name,
                    Province = parsed.Region.Code,
                    Postal_code = parsed.Postal.ToString(),
                    Country = Address.Country,
                    Telephone = Address.Telephone,
                    Language = Address.Language,
                    Latitude = Address.Latitude,
                    Longitude = Address.Longitude,
                    Notes = Address.Notes,
                    Notes_private = Address.Notes_private,
                };
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }

            WriteObject(normalized);
        }
    }
}
