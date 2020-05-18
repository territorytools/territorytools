namespace TerritoryTools.Entities.AddressParsers
{
    public class ParsedAddress
    {
        public AddressPartResult Number { get; set; } 
            = new AddressPartResult();

        public AddressPartResult NumberFraction { get; set; } 
            = new AddressPartResult();

        public AddressPartResult DirectionalPrefix { get; set; } 
            = new AddressPartResult();

        public AddressPartResult StreetName { get; set; }
            = new AddressPartResult();

        public AddressPartResult StreetType { get; set; } 
            = new AddressPartResult();

        public AddressPartResult DirectionalSuffix { get; set; } 
            = new AddressPartResult();

        public AddressPartResult UnitType { get; set; } 
            = new AddressPartResult();

        public AddressPartResult UnitNumber { get; set; } 
            = new AddressPartResult();

        public AddressPartResult City { get; set; } 
            = new AddressPartResult();

        public AddressPartResult State { get; set; } 
            = new AddressPartResult();

        public AddressPartResult PostalCode { get; set; } 
            = new AddressPartResult();

        public AddressPartResult PostalCodeExt { get; set; } 
            = new AddressPartResult();
    }
}
