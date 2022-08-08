using System.Collections.Generic;

namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressParseContainer
    {
        public AddressParseContainer(string completeAddressToParse)
        {
            CompleteAddressToParse = completeAddressToParse;
            AddressParts = new List<string>();
            AddressPartResults = new List<AddressPartResult>();
            Address = new Address();
            ParsedAddress = new ParsedAddress();
        }

        public Address Address { get; set; }

        public string CompleteAddressToParse { get; set; }

        public List<string> AddressParts { get; set; }
        public List<int> AddressPartsGroupIndex { get; set; } = new List<int>();
        public List<int> AddressPartsPartIndex { get; set; } = new List<int>();

        public List<List<string>> AddressPartsGrouped { get; set; } = new List<List<string>>();
        public List<string> AddressGroups { get; set; } = new List<string>();

        public List<AddressPartResult> AddressPartResults { get; set; }

        public ParsedAddress ParsedAddress { get; set; }

        public override string ToString()
        {
            return CompleteAddressToParse;
        }
    }
}
