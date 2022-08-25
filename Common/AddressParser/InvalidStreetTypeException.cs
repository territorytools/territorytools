namespace TerritoryTools.Entities.AddressParsers
{
    public class InvalidStreetTypeException : AddressParsingException
    {
        public InvalidStreetTypeException(string streetType, string addressToParse)
            : base(addressToParse)
        {
            StreetType = streetType;
        }

        public string StreetType { get; set; }

        public override string Message
        {
            get
            {
                return $"Invalid Street Type: '{StreetType}' in address: {AddressToParse}";
            }
        }
    }
}
