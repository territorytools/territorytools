namespace TerritoryTools.Entities.AddressParsers
{
    public class MissingStreetNumberException : AddressParsingException
    {
        public MissingStreetNumberException(string addressToParse)
            : base(addressToParse)
        {
        }
        public override string Message
        {
            get
            {
                return "Missing Street Number: " + AddressToParse;
            }
        }
    }
}
