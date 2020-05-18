namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressPartResult
    {
        public string Value { get; set; }

        public int Index { get; set; }

        public override string ToString()
        {
            return Index + ": " + Value ?? "Null";
        }
    }
}
