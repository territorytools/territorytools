using System.Collections.Generic;

namespace TerritoryTools.Entities.AddressParsers
{
    public class NonPhysicalStreetTypeFinder : PrefixStreetTypeFinder
    {
        public NonPhysicalStreetTypeFinder(AddressParseContainer container) 
            : base(container)
        {
        }

        protected override void SetStreetTypeTo(AddressPartResult result)
        {
            base.SetStreetTypeTo(result);

            container.Address.IsNotPhysical = true;
        }
    
        protected override List<string> GetComletePrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "POB",
                    "P.O.B.",
                };

            return streetTypes;
        }

        protected override List<string> GetFirstPartPrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "PO",
                    "P.O.",
                };

            return streetTypes;
        }

        protected override List<string> GetSecondPartPrefixes()
        {
            var streetTypes = new List<string>()
                {
                    "Box",
                };

            return streetTypes;
        }
    }
}
