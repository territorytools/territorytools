using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public class DirectionalPrefixFinder : DirectionalFinder
    {
        public DirectionalPrefixFinder(AddressParseContainer container)
            : base(container)
        {
        }

        protected override bool IsAPossibleMatch(AddressPartResult match)
        {
            var streetType = parsedAddress.StreetType;

            var patternMatches = MatchesDirection(match.Value);
            var twoBeforeStreetType = match.IsAtLeastNBefore(2, streetType);

            if (patternMatches && twoBeforeStreetType)
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
            var match = possibleMatches.Last(); 

            container.ParsedAddress.DirectionalPrefix = match;
        }
    }
}
