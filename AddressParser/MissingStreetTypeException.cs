using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerritoryTools.Entities.AddressParsers
{
    public class MissingStreetTypeException : AddressParsingException
    {
        public MissingStreetTypeException(string addressToParse)
            : base(addressToParse)
        {
        }
        public override string Message
        {
            get
            {
                return "Missing Street Type: " + this.AddressToParse;
            }
        }
    }
}
