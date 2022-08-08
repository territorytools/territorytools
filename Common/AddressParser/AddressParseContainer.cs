using System.Collections.Generic;

namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressParseContainer
    {
        public AddressParseContainer(string completeAddressToParse)
        {
            this.CompleteAddressToParse = completeAddressToParse;
            this.AddressParts = new List<string>();
            this.AddressPartResults = new List<AddressPartResult>();
            this.Address = new Address();
            this.ParsedAddress = new ParsedAddress();
        }

        public Address Address { get; set; }

        public string CompleteAddressToParse { get; set; }

        public List<string> AddressParts { get; set; }

        public List<AddressPartResult> AddressPartResults { get; set; }

        public ParsedAddress ParsedAddress { get; set; }

        public override string ToString()
        {
            return CompleteAddressToParse;
        }
    }
}
