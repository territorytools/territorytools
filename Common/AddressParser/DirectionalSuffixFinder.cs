using System.Linq;

namespace TerritoryTools.Entities.AddressParsers
{
    public class DirectionalSuffixFinder : DirectionalFinder
    {
        public DirectionalSuffixFinder(AddressParseContainer container)
            : base(container)
        {
        }

        protected override bool IsAPossibleMatch(AddressPartResult match)
        {
            SetInstanceVariablesFromContainer();

            bool patternMatches = MatchesDirection(match.Value);
            bool afterStreetType = match.IsAfter(streetType);
            bool beforeState = state.IsNotSet() || match.IsAtLeastNBefore(2, state);
            bool beforePostalCode = postalCode.IsNotSet() || match.IsBefore(postalCode);
            bool beforeUnitType = unitType.IsNotSet() || match.IsBefore(unitType);

            if (patternMatches && afterStreetType && beforeState && beforePostalCode && beforeUnitType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void FindMatch()
        {
            var match = AlwaysPickTheFirstDirectionalForSuffix();

            container.ParsedAddress.DirectionalSuffix = match;
        }

        private AddressPartResult AlwaysPickTheFirstDirectionalForSuffix()
        {
            return possibleMatches.First();
        }
    }
}
