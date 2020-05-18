using System.Collections.Generic;

namespace TerritoryTools.Entities.AddressParsers
{
    public class PhysicalPrefixStreetTypeFinder : PrefixStreetTypeFinder
    {
        public PhysicalPrefixStreetTypeFinder(AddressParseContainer container) 
            : base(container)
        {
        }

        protected override List<string> GetComletePrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "SR",
                    "S.R.",
                    "HIGHWAY",
                    "HWY"
                };

            return streetTypes;
        }

        protected override List<string> GetFirstPartPrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "State",
                    "S.",
                    "S",
                };

            return streetTypes;
        }

        protected override List<string> GetSecondPartPrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "Route",
                    "R.",
                    "R",
                };

            return streetTypes;
        }
    }
}
