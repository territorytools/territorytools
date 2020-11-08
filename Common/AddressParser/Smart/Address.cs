namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Address
    {
        public Street Street { get; set; } = new Street();
        public Unit Unit { get; set; } = new Unit();
        public City City { get; set; } = new City();
        public Region Region { get; set; } = new Region();
        public Postal Postal { get; set; } = new Postal();
    }
}
