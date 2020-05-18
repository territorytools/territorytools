namespace TerritoryTools.Entities.AddressParsers
{
    public abstract class Finder
    {
        public Finder(AddressParseContainer container)
        {
            this.container = container;
            parsedAddress = container.ParsedAddress;
        }

        protected AddressParseContainer container;
        protected ParsedAddress parsedAddress;

        protected AddressPartResult number;
        protected AddressPartResult numberFraction;
        protected AddressPartResult directionalPrefix;
        protected AddressPartResult streetName;
        protected AddressPartResult streetType;
        protected AddressPartResult directionalSuffix;
        protected AddressPartResult unitType;
        protected AddressPartResult unitNumber;
        protected AddressPartResult city;        
        protected AddressPartResult state;
        protected AddressPartResult postalCode;
        protected AddressPartResult postalCodeExt;

        public virtual void Find()
        {
            FindAllPossibleMatches();

            if (PossibleMatchesWereFound())
                FindMatch();
        }

        private void FindAllPossibleMatches()
        {
            foreach (var result in container.AddressPartResults)
                FindPossibleMatch(result);
        }

        protected abstract bool PossibleMatchesWereFound();

        protected abstract void FindMatch();

        protected abstract void FindPossibleMatch(AddressPartResult result);

        protected void SetInstanceVariablesFromContainer()
        {
            number            = parsedAddress.Number;
            numberFraction    = parsedAddress.NumberFraction;
            directionalPrefix = parsedAddress.DirectionalPrefix;
            streetName        = parsedAddress.StreetName;
            streetType        = parsedAddress.StreetType;
            directionalSuffix = parsedAddress.DirectionalSuffix;
            unitType          = parsedAddress.UnitType;
            unitNumber        = parsedAddress.UnitNumber;
            city              = parsedAddress.City;
            state             = parsedAddress.State;
            postalCode        = parsedAddress.PostalCode;
            postalCodeExt     = parsedAddress.PostalCodeExt;
        }
    }
}
