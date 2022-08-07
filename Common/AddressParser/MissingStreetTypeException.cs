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
                return $"Missing Street Type: {AddressToParse}";
            }
        }
    }
}
