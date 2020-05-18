using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Entities.AddressParsers
{
    public class StreetTypeStreetNameFinder : Finder
    {
        private List<StreetType> streetTypes;
        private List<AddressPartResult> possibilities 
            = new List<AddressPartResult>();

        public StreetTypeStreetNameFinder(
            AddressParseContainer container, 
            IEnumerable<StreetType> streetTypes) : base(container)
        {
            this.streetTypes = streetTypes.ToList();
        }

        protected override void FindMatch()
        {
            if (parsedAddress.StreetType.IsNotSet()
                && PossibleMatchesWereFound())
            {
                parsedAddress.StreetName = possibilities.First();
                parsedAddress.StreetType = new AddressPartResult()
                {
                    Value = string.Empty,
                    Index = parsedAddress.StreetName.Index
                };
            }
                
        }

        protected override void FindPossibleMatch(AddressPartResult part)
        {
            if (EndsWithStreetType(part.Value))
                possibilities.Add(part);
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibilities.Count > 0;
        }

        private bool EndsWithStreetType(string value)
        {
            return streetTypes
                .Exists(s => value.EndsWith(
                    s.Full,
                    StringComparison.CurrentCultureIgnoreCase)
                && value.Length > s.Full.Length);
        }
    }
}
