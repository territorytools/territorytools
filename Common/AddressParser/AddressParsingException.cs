using System;

namespace TerritoryTools.Entities
{
    public class AddressParsingException : Exception
    {
        public AddressParsingException(string addressToParse)
            : base(addressToParse)
        {
            AddressToParse = addressToParse;
        }

        public string AddressToParse { get; set; }

        public override string Message
        {
            get
            {
                return "Address Parsing Exception: " + this.AddressToParse;
            }
        }
    }
}
