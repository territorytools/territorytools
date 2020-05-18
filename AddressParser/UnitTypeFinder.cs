using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public class UnitTypeFinder : Finder
    {
        public UnitTypeFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        private List<AddressPartResult> possibleMatches;

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count == 1;
        }

        protected override void FindMatch()
        {
            var unitType = possibleMatches.Single();

            if (IsAfterStreetType(unitType))
            {
                container.ParsedAddress.UnitType = unitType;
            }
        }

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAfterStreetType(result)
                && IsAPossibleMatch(result.Value))
            {
                possibleMatches.Add(result);
            }
        }

        private bool IsAfterStreetType(AddressPartResult part)
        {
            return part.IsAfter(parsedAddress.StreetType);
        }

        private bool IsAPossibleMatch(string part)
        {
            return MatchesUnitType(part);
        }

        public bool MatchesUnitType(string value)
        {
            return Regex.IsMatch(
                value,
                @"^((#|APT|STE|UNIT|APARTMENT|BUILDING|BLDG))?$",
                RegexOptions.IgnoreCase);
        }
    }
}
